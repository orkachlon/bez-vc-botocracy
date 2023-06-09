using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils;
using Main.GameModifications;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.StoryPoints.SPProviders {
    public class CSVSPProvider : MonoBehaviour, ISPProvider {

        [SerializeField] private TextAsset StoryPointsCSV;
        
        [Header("Event Managers"), SerializeField] private SEventManager modificationsEventManager;

        private const int RowsPerSP = 6;

        private IEnumerator<Dictionary<string, object>> _spEnumerator;
        private (
            string id,
            string description,
            string decidingTraits,
            string cmmndr,
            string ntrpnr,
            string mdtr,
            string dfndr,
            string ntrpst,
            string lgstcn,
            string outcomes,
            string outcomeID,
            string turnsToEvaluation,
            string reward,
            string prerequisites
            ) _header;
        
        private bool _isInfinite;

        public int Count { get; private set; }

        private void Awake() {
            InitHeader();
            Reset();
            
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteSP, OnInfiniteStoryPoints);
        }

        private void OnDestroy() {
            modificationsEventManager.Unregister(GameModificationEvents.OnInfiniteSP, OnInfiniteStoryPoints);
        }

        public StoryPointData Next() {
            if (IsEmpty()) {
                throw new IndexOutOfRangeException("No more events in queue!");
            }

            if (Count == 0 && _isInfinite) {
                Reset();
            }
            var currentStoryEntries = new List<Dictionary<string, object>>();

            var i = 0;
            while (i < RowsPerSP && _spEnumerator.MoveNext()) {
                currentStoryEntries.Add(_spEnumerator.Current);
                i++;
            }

            var nextSP = TryParse(currentStoryEntries);
            Count--;

            return nextSP;
        }

        public bool IsEmpty() {
            return !_isInfinite && Count == 0;
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
        }

        private StoryPointData TryParse(List<Dictionary<string, object>> entries) {
            var newSPData = new StoryPointData {
                id = (int) entries[0][_header.id],
                description = (string) entries[0][_header.description],
                reward = (int) entries[0][_header.reward],
                turnsToEvaluation = (int) entries[0][_header.turnsToEvaluation]
            };

            var decidingTraits = GetDecidingTraits(entries);
            if (decidingTraits == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.description}");
            }

            newSPData.decidingTraits = decidingTraits;

            // doesn't work yet. Delayed to later date
            var prerequisites = GetPrerequisites(entries);
            if (prerequisites != null) {
                newSPData.prerequisites = prerequisites;
            }

            return newSPData;
        }

        private int[] GetPrerequisites(List<Dictionary<string,object>> entries) {
            var preString = (string) entries[0][_header.prerequisites];
            if (string.IsNullOrEmpty(preString)) {
                return null;
            }

            return null;
        }

        private DecidingTraits GetDecidingTraits(List<Dictionary<string, object>> entries) {
            var deciders = new DecidingTraits();
            foreach (var entry in entries) {
                if ((string) entry[_header.outcomes] == "-") {
                    continue;
                }

                var deciderEffects = GetDeciderEffects(entry);
                if (deciderEffects == null) {
                    return null;
                }

                var traitString = (string) entry[_header.decidingTraits];
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
                Outcome = (string) entry[_header.outcomes],
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
        //         var traitString = (string) entry[_header.traits];
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

        private void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            _header.id = headerAsArray[0];
            _header.description = headerAsArray[1];
            _header.decidingTraits = headerAsArray[2];
            _header.cmmndr = headerAsArray[3];
            _header.ntrpnr = headerAsArray[4];
            _header.mdtr = headerAsArray[5];
            _header.dfndr = headerAsArray[6];
            _header.ntrpst = headerAsArray[7];
            _header.lgstcn = headerAsArray[8];
            _header.outcomes = headerAsArray[9];
            _header.outcomeID = headerAsArray[10];
            _header.turnsToEvaluation = headerAsArray[11];
            _header.reward = headerAsArray[12];
            _header.prerequisites = headerAsArray[13];
        }

        #region EventHandlers

        private void OnInfiniteStoryPoints(EventArgs eventArgs) {
            if (eventArgs is not IsInfiniteStoryPointsEventArgs infiniteSPArgs) {
                return;
            }

            _isInfinite = infiniteSPArgs.IsInfinite;
        }

        #endregion

    }
}