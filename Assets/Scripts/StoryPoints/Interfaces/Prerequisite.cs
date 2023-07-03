using Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace StoryPoints.Interfaces {
    public static class Prerequisite {

        public static bool Evaluate(string prerequisite, IEnumerable<int> input) {
            if (string.IsNullOrEmpty(prerequisite)) {
                return true;
            }
            if (prerequisite[0] == 'C') {
                var subSentences = prerequisite[1..].Split('|');
                return subSentences
                    .Any(subSentence => MatchesCNF(subSentence, input));
            } else if (prerequisite[0] == 'D') {
                var subSentences = prerequisite[1..].Split("&");
                return subSentences
                    .Any(subSentence => MatchesDNF(subSentence, input));
            } else {
                MLogger.LogEditorError("Unrecognized prerequisite string!");
            }
            return false;
        }

        private static bool MatchesDNF(string subSentence, IEnumerable<int> input) {
            subSentence = subSentence.Trim('(', ')');
            var literals = subSentence.Split('|');
            foreach (var literal in literals) {
                if (literal.StartsWith('!')) {
                    int.TryParse(literal[1..], out var id);
                    if (!input.Contains(id)) {
                        return true;
                    }
                } else {
                    int.TryParse(literal, out var id);
                    if (input.Contains(id)) {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool MatchesCNF(string subSentence, IEnumerable<int> input) {
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