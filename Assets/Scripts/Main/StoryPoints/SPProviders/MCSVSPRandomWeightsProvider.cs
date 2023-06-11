using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Main.StoryPoints.Interfaces;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.StoryPoints.SPProviders {
    public class MCSVSPRandomWeightsProvider : CSVSPProvider {

        [SerializeField, Range(0, 7)] private int numberOfDecidingTraits;
        protected override CSVHeader Header { 
            get => base.Header as CSVHeaderWithActions;
            set => base.Header = value;
        }

        protected override StoryPointData? TryParse(List<Dictionary<string, object>> entries) {
            if (entries == null || entries.Count == 0) {
                return null;
            }
            var newSPData = new StoryPointData {
                id = (int) entries[0][Header.id],
                description = (string) entries[0][Header.description],
                turnsToEvaluation = (int) entries[0][Header.turnsToEvaluation],
                prerequisites = (string) entries[0][Header.prerequisites]
            };

            var decidingTraits = GetDecidingTraits(entries);
            if (decidingTraits == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.title}");
            }

            newSPData.decidingTraits = decidingTraits;
            newSPData.title = (string) entries[0][((CSVHeaderWithActions) Header).title];
            return newSPData;
        }

        protected override DecidingTraits GetDecidingTraits(List<Dictionary<string, object>> entries) {
            var deciders = new DecidingTraits();
            // deciding traits are chosen randomly
            entries = entries.OrderBy(_ => Random.value).Take(numberOfDecidingTraits).ToList();
            foreach (var entry in entries) {
                var deciderAction = (string) entry[((CSVHeaderWithActions) Header).actions];
                if (deciderAction == null) {
                    return null;
                }

                var deciderEffects = GetTraitDecisionEffects(entry);

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

        private TraitDecisionEffects GetTraitDecisionEffects(IReadOnlyDictionary<string, object> entry) {
            var deciderEffects = new TraitDecisionEffects {
                Action = (string) entry[((CSVHeaderWithActions) Header).actions],
                Outcome = (string) entry[Header.outcomes],
                BoardEffect = new Dictionary<ETrait, int>()
            };
            EnumUtil.GetValues<ETrait>()
                .ToList()
                .ForEach(t =>
                    deciderEffects.BoardEffect.Add(t, Random.Range(-1, 2)));
            deciderEffects.DecidingTrait = EnumUtil.GetValues<ETrait>()
                .First(t => t.ToString().Equals(entry[Header.decidingTraits]));
            return deciderEffects;
        }

        protected override void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            Header = new CSVHeaderWithActions {
                id = headerAsArray[0],
                title = headerAsArray[1],
                description = headerAsArray[2],
                decidingTraits = headerAsArray[3],
                // skip 4, it's just for validation
                actions = headerAsArray[5],
                outcomes = headerAsArray[6],
                outcomeID = headerAsArray[7],
                turnsToEvaluation = headerAsArray[8],
                prerequisites = headerAsArray[9]
            };
        }

        protected class CSVHeaderWithActions : CSVHeader {
            public string title;
            public string actions;
        }
    }
}