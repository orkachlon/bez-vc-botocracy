using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils;
using Main.GameModifications;
using Main.GameStats;
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
            string traits, 
            string economy, 
            string welfare, 
            string defense, 
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
                description = (string) entries[0][_header.description],
                reward = (int) entries[0][_header.reward],
                turnsToEvaluation = (int) entries[0][_header.turnsToEvaluation]
            };
            var statsToTraits = GetStatToTraitWeights(entries);
            if (statsToTraits == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.description}");
            }

            newSPData.statEffects = statsToTraits;

            var outcomes = GetTraitsToOutcomes(entries);
            if (outcomes == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.description}");
            }

            newSPData.outcomes = outcomes;

            return newSPData;
        }

        private TraitsToOutcomes GetTraitsToOutcomes(List<Dictionary<string, object>> entries) {
            var traitOutcomes = new TraitsToOutcomes();
            foreach (var entry in entries) {
                var traitString = (string) entry[_header.traits];
                if (Enum.TryParse<ETraitType>(traitString, out var trait)) {
                    traitOutcomes[trait] = (string) entry[_header.outcomes];
                } else {
                    return null;
                }
            }
            
            return traitOutcomes;
        }

        private StatToTraitWeights GetStatToTraitWeights(List<Dictionary<string, object>> entries) {
            var statsToTraits = new StatToTraitWeights();
            foreach (var stat in EnumUtil.GetValues<EStatType>()) {
                var traitWeights = GetTraitWeightsForStat(entries, stat);
                if (traitWeights == null) {
                    return null;
                }

                statsToTraits[stat] = traitWeights;
            }

            return statsToTraits;
        }

        private TraitWeights GetTraitWeightsForStat(List<Dictionary<string, object>> entries, EStatType stat) {
            var weights = new TraitWeights();
            foreach (var entry in entries) {
                var traitString = (string) entry[_header.traits];
                var weight = (int) entry[stat.ToString()];
                if (Enum.TryParse<ETraitType>(traitString, out var trait)) {
                    weights[trait] = weight;
                } else {
                    return null;
                }
            }

            return weights;
        }

        private void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            _header.id = headerAsArray[0];
            _header.description = headerAsArray[1];
            _header.traits = headerAsArray[2];
            _header.economy = headerAsArray[3];
            _header.welfare = headerAsArray[4];
            _header.defense = headerAsArray[5];
            _header.outcomes = headerAsArray[6];
            _header.outcomeID = headerAsArray[7];
            _header.turnsToEvaluation = headerAsArray[8];
            _header.reward = headerAsArray[9];
            _header.prerequisites = headerAsArray[10];
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