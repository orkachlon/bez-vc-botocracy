using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using JetBrains.Annotations;
using StoryPoints.Interfaces;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StoryPoints.SPProviders {
    public class CSVSPProvider : MonoBehaviour, ISPProvider {

        [SerializeField] protected int memorySize;
        [SerializeField] protected TextAsset StoryPointsCSV;
        
        [Header("Event Managers"), SerializeField] protected SEventManager modificationsEventManager;

        private const int RowsPerSP = 6;

        private List<Dictionary<string, object>> _allEntries = new();
        private Dictionary<int, StoryPointData> _allSPs = new();
        private HashSet<int> _outcomeIDs = new();
        private Queue<int> _usedSPs;
        
        protected virtual CSVHeader Header { get; set; }
        
        public int Count { get; private set; }


        #region UnityMethods

        private void Awake() {
            InitHeader();
            GetAllSPsFromFile();
            _usedSPs = new Queue<int>(memorySize);
            // ResetProvider();
            // ClearOutcomes();
        }

        #endregion

        [CanBeNull]
        public virtual StoryPointData? Next() {
            // check that 
            if (_allSPs.Count <= 0) {
                throw new IndexOutOfRangeException("No more events in queue!");
            }

            var nextSP = GetNextSP();
            return nextSP;
        }

        private StoryPointData? GetNextSP() {
            var sp = _allSPs.Values
                .Where(IsValidSP)
                .OrderBy(_ => Random.value)
                .First();
            RecordSP(sp);
            return sp;
        }

        private void RecordSP(StoryPointData sp) {
            if (_usedSPs.Count == memorySize) {
                _usedSPs.TryDequeue(out _);
            }
            _usedSPs.Enqueue(sp.id);
        }

        private bool IsValidSP(StoryPointData sp) {
            return !_usedSPs.Contains(sp.id) && Prerequisite.Evaluate(sp.prerequisites, _outcomeIDs);
        }
        
        private void GetAllSPsFromFile() {
            ReadAllEntriesFromFile();
            ParseAllSPs();
        }

        private void ReadAllEntriesFromFile() {
            _allEntries?.Clear();
            _allEntries = CSVReader.Read(StoryPointsCSV);
            if (_allEntries == null || _allEntries.Count % RowsPerSP != 0) {
                Count = 0;
            }
            else {
                Count = _allEntries.Count / RowsPerSP;
            }
        }

        private void ParseAllSPs() {
            if (Count == 0) {
                return;
            }
            _allSPs = new Dictionary<int, StoryPointData>();
            for (var i = 0; i < _allEntries.Count; i += 6) {
                var trySP = TryParse(_allEntries.GetRange(i, 6));
                if (trySP.HasValue) {
                    _allSPs[trySP.Value.id] = trySP.Value;
                }
            }
        }

        public void AddOutcome(int outcomeID) {
            if (_outcomeIDs == null) { 
                _outcomeIDs = new HashSet<int> { outcomeID };
                return;
            }
            _outcomeIDs.Add(outcomeID);
        }

        public void RemoveOutcome(int outcomeID) {
            if (_outcomeIDs == null || !_outcomeIDs.Contains(outcomeID)) {
                return;
            }
            _outcomeIDs.Remove(outcomeID);
        }

        protected virtual StoryPointData? TryParse(List<Dictionary<string, object>> entries) {
            if (entries == null || entries.Count == 0) {
                return null;
            }
            var newSPData = new StoryPointData {
                id = (int) entries[0][Header.ID],
                description = (string) entries[0][Header.Description],
                reward = (int) entries[0][Header.Reward],
                turnsToEvaluation = (int) entries[0][Header.TurnsToEvaluation],
                prerequisites = (string) entries[0][Header.Prerequisites]
            };

            var decidingTraits = GetDecidingTraits(entries);
            newSPData.decidingTraits = decidingTraits ?? throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.description}");
            return newSPData;
        }

        protected virtual DecidingTraits GetDecidingTraits(List<Dictionary<string, object>> entries) {
            var deciders = new DecidingTraits();
            foreach (var entry in entries) {
                if ((int) entry[Header.OutcomeID] < 0) {
                    continue;
                }

                var deciderEffects = GetDeciderEffects(entry);
                if (deciderEffects == null) {
                    return null;
                }

                var traitString = (string) entry[Header.DecidingTraits];
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
                OutcomeID = (int) entry[Header.OutcomeID],
                Outcome = (string) entry[Header.Outcomes],
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

        protected virtual  void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            Header = new CSVHeader {
                ID = headerAsArray[0],
                Description = headerAsArray[1],
                DecidingTraits = headerAsArray[2],
                Cmmndr = headerAsArray[3],
                Ntrpnr = headerAsArray[4],
                Mdtr = headerAsArray[5],
                Dfndr = headerAsArray[6],
                Ntrpst = headerAsArray[7],
                Lgstcn = headerAsArray[8],
                Outcomes = headerAsArray[9],
                OutcomeID = headerAsArray[10],
                TurnsToEvaluation = headerAsArray[11],
                Reward = headerAsArray[12],
                Prerequisites = headerAsArray[13]
            };
        }

        protected class CSVHeader {
            public string ID;
            public string Description;
            public string DecidingTraits;
            public string Cmmndr;
            public string Ntrpnr;
            public string Mdtr;
            public string Dfndr;
            public string Ntrpst;
            public string Lgstcn;
            public string Outcomes;
            public string OutcomeID;
            public string TurnsToEvaluation;
            public string Reward;
            public string Prerequisites;
        }

    }
}