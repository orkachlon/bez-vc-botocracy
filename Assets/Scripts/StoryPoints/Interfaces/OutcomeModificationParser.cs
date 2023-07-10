using System.Linq;
using Types.StoryPoint;

namespace StoryPoints.Interfaces {
    public static class OutcomeModificationParser {


        public static void ModifyOutcomes(ISPProvider outcomes, string modification) {
            if (!modification.StartsWith('R')) {
                return;
            }
            var ids = modification
                .TrimStart('R')
                .Trim('(', ')')
                .Split(',')
                .Select(int.Parse);

            foreach (var id in ids) {
                outcomes.RemoveOutcome(id);
            }
        }

    }
}