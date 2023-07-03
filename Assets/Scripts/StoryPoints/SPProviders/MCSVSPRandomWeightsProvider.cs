using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Utils;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StoryPoints.SPProviders {
    public class MCSVSPRandomWeightsProvider : CSVSPProvider {

        [SerializeField, Range(0, 6)] private int numberOfDecidingTraits;
        [SerializeField] private string spImagesPath;

        protected override CSVHeader Header { 
            get => base.Header as CSVHeaderWithActions;
            set => base.Header = value;
        }

        protected override StoryPointData? TryParse(List<Dictionary<string, object>> entries) {
            if (entries == null || entries.Count == 0) {
                return null;
            }
            var newSPData = new StoryPointData {
                id = (int) entries[0][Header.ID],
                title = (string) entries[0][((CSVHeaderWithActions) Header).Title],
                description = (string) entries[0][Header.Description],
                turnsToEvaluation = (int) entries[0][Header.TurnsToEvaluation],
                prerequisites = (string) entries[0][Header.Prerequisites]
            };

            var decidingTraits = GetDecidingTraits(entries);
            if (decidingTraits == null) {
                throw new Exception($"SP data couldn't be read correctly! SP: {newSPData.title}");
            }
            newSPData.decidingTraits = decidingTraits;

            var image = GetImage(newSPData.id);
            if (image == null) {
                //throw new Exception($"SP image missing! SP: {newSPData.title}");
                MLogger.LogEditorWarning($"SP \"{newSPData.title}\" image not found!");
            } else {
                newSPData.image = image;
            }

            return newSPData;
        }

        private Sprite GetImage(int id) {
            return Resources.Load<Sprite>(Path.Join(spImagesPath, id.ToString()));
        }

        protected override DecidingTraits GetDecidingTraits(List<Dictionary<string, object>> entries) {
            var deciders = new DecidingTraits();
            // deciding traits are chosen randomly (filter traits that can't decide)
            entries = entries
                .Where(e => (string) e[Header.Outcomes] != "-")
                .OrderBy(_ => Random.value)
                .Take(numberOfDecidingTraits)
                .ToList();
            foreach (var entry in entries) {
                var deciderAction = (string) entry[((CSVHeaderWithActions) Header).Actions];
                if (deciderAction == null) {
                    return null;
                }

                var deciderEffects = GetTraitDecisionEffects(entry);

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

        protected virtual TraitDecisionEffects GetTraitDecisionEffects(IReadOnlyDictionary<string, object> entry) {
            var deciderEffects = new TraitDecisionEffects {
                Decision = (string) entry[((CSVHeaderWithActions) Header).Actions],
                OutcomeID = (int) entry[Header.OutcomeID],
                Outcome = (string) entry[Header.Outcomes],
                BoardEffect = new Dictionary<ETrait, int>()
            };
            EnumUtil.GetValues<ETrait>()
                .ToList()
                .ForEach(t =>
                    deciderEffects.BoardEffect.Add(t, Random.Range(-1, 2)));
            deciderEffects.DecidingTrait = EnumUtil.GetValues<ETrait>()
                .First(t => t.ToString().Equals(entry[Header.DecidingTraits]));
            return deciderEffects;
        }

        protected override void InitHeader() {
            var headerAsArray = CSVReader.GetHeader(StoryPointsCSV);
            Header = new CSVHeaderWithActions {
                ID = headerAsArray[0],
                Title = headerAsArray[1],
                Description = headerAsArray[2],
                DecidingTraits = headerAsArray[3],
                // skip 4, it's just for validation
                Actions = headerAsArray[5],
                Outcomes = headerAsArray[6],
                OutcomeID = headerAsArray[7],
                TurnsToEvaluation = headerAsArray[8],
                Prerequisites = headerAsArray[9]
            };
        }

        protected class CSVHeaderWithActions : CSVHeader {
            public string Title;
            public string Actions;
            public string OutcomeModifications;
        }
    }
}