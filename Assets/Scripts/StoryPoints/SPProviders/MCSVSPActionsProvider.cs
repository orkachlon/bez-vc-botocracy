using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using StoryPoints.Types;
using Types.Trait;

namespace StoryPoints.SPProviders {
    public class MCSVSPActionsProvider : MCSVSPRandomWeightsProvider {
        
        protected override TraitDecisionEffects GetTraitDecisionEffects(IReadOnlyDictionary<string, object> entry) {
            var deciderEffects = new TraitDecisionEffects {
                decision = (string) entry[((CSVHeaderWithActions) Header).Actions],
                outcomeID = (int) entry[Header.OutcomeID],
                outcome = (string) entry[Header.Outcomes],
                outcomeModification = (string) entry[((CSVHeaderWithActions) Header).OutcomeModifications],
                boardEffect = new Dictionary<ETrait, int>()
            };
            EnumUtil.GetValues<ETrait>()
                .ToList()
                .ForEach(t =>
                    deciderEffects.boardEffect.Add(t, (int) entry[t.ToString()]));
            deciderEffects.decidingTrait = EnumUtil.GetValues<ETrait>()
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
                Cmmndr = headerAsArray[4],
                Ntrpnr = headerAsArray[5],
                Mdtr = headerAsArray[6],
                Dfndr = headerAsArray[7],
                Ntrpst = headerAsArray[8],
                Lgstcn = headerAsArray[9],
                // skip 10, it's just for validation
                Actions = headerAsArray[11],
                Outcomes = headerAsArray[12],
                OutcomeID = headerAsArray[13],
                TurnsToEvaluation = headerAsArray[14],
                Prerequisites = headerAsArray[15],
                OutcomeModifications = headerAsArray[16]
            };
        }
    }
}