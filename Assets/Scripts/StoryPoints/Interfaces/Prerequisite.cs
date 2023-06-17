using System.Linq;

namespace StoryPoints.Interfaces {
    public static class Prerequisite {

        public static bool Evaluate(string prerequisite, int[] input) {
            var subSentences = prerequisite.Split('|');
            return subSentences
                .Any(subSentence => Matches(subSentence, input));
        }

        private static bool Matches(string subSentence, int[] input) {
            subSentence = subSentence.Trim('(', ')');
            var literals = subSentence.Split('&');
            foreach (var literal in literals) {
                if (literal.StartsWith('!')) {
                    int.TryParse(literal[1..], out var id);
                    if (input.Contains(id)) {
                        return false;
                    }
                }
                else {
                    int.TryParse(literal, out var id);
                    if (!input.Contains(id)) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}