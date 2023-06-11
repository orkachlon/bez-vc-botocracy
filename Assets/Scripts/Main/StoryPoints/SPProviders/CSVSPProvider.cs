using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using JetBrains.Annotations;
using Main.GameModifications;
using Main.StoryPoints.Interfaces;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.StoryPoints.SPProviders {
    public class CSVSPProvider : MonoBehaviour, ISPProvider {

        [SerializeField] protected TextAsset StoryPointsCSV;
        
        [Header("Event Managers"), SerializeField] protected SEventManager modificationsEventManager;

        private const int RowsPerSP = 6;

        private IEnumerator<Dictionary<string, object>> _spEnumerator;
        protected virtual CSVHeader Header { get; set; }

        private bool _isInfinite;

        public int Count { get; private set; }

        private int[] _outcomeIDs;

        // we store SPs which haven't met their prerequisites yet to try them again later.
        private readonly HashSet<StoryPointData> _unusedSPs = new HashSet<StoryPointData>();

        #region UnityMethods

        private void Awake() {
            InitHeader();
            Reset();
        }

        private void OnEnable() {
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteSP, OnInfiniteStoryPoints);
        }

        private void OnDisable() {
            modificationsEventManager.Unregister(GameModificationEvents.OnInfiniteSP, OnInfiniteStoryPoints);
        }

        #endregion

        [CanBeNull]
        public virtual StoryPointData? Next() {
            if (IsEmpty()) {
                throw new IndexOutOfRangeException("No more events in queue!");
            }

            if (Count == 0 && _isInfinite) {
                Reset();
            }

            // try to get event from previously unused
            if (_unusedSPs.Any(sp => Prerequisite.Evaluate(sp.prerequisites, _outcomeIDs))) {
                Count--;
                return _unusedSPs.First(sp => Prerequisite.Evaluate(sp.prerequisites, _outcomeIDs));
            }
            
            // read from file - I think that we can be sure we have more to read from file because we asked IsEmpty
            var currentStoryEntries = ReadNextStoryEntriesFromFile();
            var nextSP = TryParse(currentStoryEntries);

            // if this SP's prerequisites aren't answered yet, and we haven't reached EOF. 
            while (nextSP.HasValue && !string.IsNullOrEmpty(nextSP.Value.prerequisites) && !Prerequisite.Evaluate(nextSP.Value.prerequisites, _outcomeIDs)) {
                // save it for later
                _unusedSPs.Add(nextSP.Value);
                // read the next one from file
                currentStoryEntries = ReadNextStoryEntriesFromFile();
                nextSP = TryParse(currentStoryEntries);
            }

            if (nextSP.HasValue) {
                Count--;
            }
            return nextSP;
        }

        private List<Dictionary<string, object>> ReadNextStoryEntriesFromFile() {
            var currentStoryEntries = new List<Dictionary<string, object>>();

            var i = 0;
            while (i < RowsPerSP && _spEnumerator.MoveNext()) {
                currentStoryEntries.Add(_spEnumerator.Current);
                i++;
            }

            return currentStoryEntries;
        }

        public bool IsEmpty() {
            return !_isInfinite && Count == 0 && _unusedSPs.All(sp => !Prerequisite.Evaluate(sp.prerequisites, _outcomeIDs));
        }

        public void Reset() {
            _spEnumerator?.Dispose();
            _spEnumerator = CSVReader.ReadIterative(StoryPointsCSV).GetEnumerator();
            var allSPs = CSVReader.Read(StoryPointsCSV);
            if (allSPs == null || allSPs.Count % RowsPerSP != 0) {
                Count = 0;
            }
            else {
                Count = allSPs.Count / RowsPerSP;
            }
            _outcomeIDs = new int[] { };
        }

        public void AddOutcome(int outcomeID) {
             _outcomeIDs ??= _outcomeIDs.Append(outcomeID) as int[];
        }

        protected virtual StoryPointData? TryParse(List<Dictionary<string, object>> entries) {
            if (entries == null || entries.Count == 0) {
                return null;
            }
            var newSPData = new StoryPointData {
                id = (int) entries[0][Header.id],
                description = (string) entries[0][Header.description],
                reward = (int) entries[0][Header.reward],
                turnsToEvaluation = (int) entries[0][Header.turnsToEvaluation],
                prerequisites = (string) entries[0][Header.prerequisites]
            };

            var decidingTraits = GetDecidingTraits(entries);
            if (decidingTraits == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.description}");
            }

            newSPData.decidingTraits = decidingTraits;
            return newSPData;
        }

        protected virtual DecidingTraits GetDecidingTraits(List<Dictionary<string, object>> entries) {
            var deciders = new DecidingTraits();
            foreach (var entry in entries) {
                if ((string) entry[Header.outcomes] == "-") {
                    continue;
                }

                var deciderEffects = GetDeciderEffects(entry);
                if (deciderEffects == null) {
                    return null;
                }

                var traitString = (string) entry[Header.decidingTraits];
                if (Enum.TryParse<ETrait>(traitString, out var trait)) {
                    deciders[trait] = deciderEffects;
                }
                else {
                    return null;
                }
            }

            return deciders;
        }

        private TraitDecisionEffects GetDeciderEffects(IReadOnlyDictionary<string, object> entry) {
            var effects = new TraitDecisionEffects {
                Outcome = (string) entry[Header.outcomes],
                BoardEffect = new Dictionary<ETrait, int>()
            };
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                if (!entry.ContainsKey(trait.ToString())) {
                    return null;
                }

                effects.BoardEffect[trait] = (int) entry[trait.ToString()];
            }

            return effects;
        }

        // private StatToTraitWeights GetStatToTraitWeights(List<Dictionary<string, object>> entries) {
        //     var statsToTraits = new StatToTraitWeights();
        //     foreach (var stat in EnumUtil.GetValues<EStatType>()) {
        //         var traitWeights = GetTraitWeightsForStat(entries, stat);
        //         if (traitWeights == null) {
        //             return null;
        //         }
        //
        //         statsToTraits[stat] = traitWeights;
        //     }
        //
        //     return statsToTraits;
        // }

        // private TraitWeights GetTraitWeightsForStat(List<Dictionary<string, object>> entries, EStatType stat) {
        //     var weights = new TraitWeights();
        //     foreach (var entry in entries) {
        //         var traitString = (string) entry[Header.traits];
        //         var weight = (int) entry[stat.ToString()];
        //         if (Enum.TryParse<ETrait>(traitString, out var trait)) {
        //             weights[trait] = weight;
        //         } else {
        //             return null;
        //         }
        //     }
        //
        //     return weights;
        // }

        protected virtual  void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            Header = new CSVHeader {
                id = headerAsArray[0],
                description = headerAsArray[1],
                decidingTraits = headerAsArray[2],
                cmmndr = headerAsArray[3],
                ntrpnr = headerAsArray[4],
                mdtr = headerAsArray[5],
                dfndr = headerAsArray[6],
                ntrpst = headerAsArray[7],
                lgstcn = headerAsArray[8],
                outcomes = headerAsArray[9],
                outcomeID = headerAsArray[10],
                turnsToEvaluation = headerAsArray[11],
                reward = headerAsArray[12],
                prerequisites = headerAsArray[13]
            };
        }

        #region EventHandlers

        private void OnInfiniteStoryPoints(EventArgs eventArgs) {
            if (eventArgs is not IsInfiniteStoryPointsEventArgs infiniteSPArgs) {
                return;
            }

            _isInfinite = infiniteSPArgs.IsInfinite;
        }

        #endregion

        protected class CSVHeader {
            public string id;
            public string description;
            public string decidingTraits;
            public string cmmndr;
            public string ntrpnr;
            public string mdtr;
            public string dfndr;
            public string ntrpst;
            public string lgstcn;
            public string outcomes;
            public string outcomeID;
            public string turnsToEvaluation;
            public string reward;
            public string prerequisites;
        }

    }
}