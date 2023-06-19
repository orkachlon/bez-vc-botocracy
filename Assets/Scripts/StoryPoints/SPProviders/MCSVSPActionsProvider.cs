using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Types.StoryPoint;
using Types.Trait;

namespace StoryPoints.SPProviders {
    public class MCSVSPActionsProvider : MCSVSPRandomWeightsProvider {
        
        protected override TraitDecisionEffects GetTraitDecisionEffects(IReadOnlyDictionary<string, object> entry) {
            var deciderEffects = new TraitDecisionEffects {
                Decision = (string) entry[((CSVHeaderWithActions) Header).actions],
                Outcome = (string) entry[Header.outcomes],
                BoardEffect = new Dictionary<ETrait, int>()
            };
            EnumUtil.GetValues<ETrait>()
                .ToList()
                .ForEach(t =>
                    deciderEffects.BoardEffect.Add(t, (int) entry[t.ToString()]));
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
                cmmndr = headerAsArray[4],
                ntrpnr = headerAsArray[5],
                mdtr = headerAsArray[6],
                dfndr = headerAsArray[7],
                ntrpst = headerAsArray[8],
                lgstcn = headerAsArray[9],
                // skip 10, it's just for validation
                actions = headerAsArray[11],
                outcomes = headerAsArray[12],
                outcomeID = headerAsArray[13],
                turnsToEvaluation = headerAsArray[14],
                prerequisites = headerAsArray[15]
            };
        }
    }
}