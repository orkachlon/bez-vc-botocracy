using Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace StoryPoints.Interfaces {
    public static class Prerequisite {

        public static bool Evaluate(string prerequisite, IEnumerable<int> input) {
            if (string.IsNullOrEmpty(prerequisite)) {
                return true;
            }

            prerequisite = prerequisite.Replace(" ", "");
            switch (prerequisite[0]) {
                case 'C': {
                    var subSentences = prerequisite[1..].Split('|');
                    return subSentences
                        .Any(subSentence => MatchesCNF(subSentence, input.ToArray()));
                }
                case 'D': {
                    var subSentences = prerequisite[1..].Split("&");
                    return subSentences
                        .All(subSentence => MatchesDNF(subSentence, input.ToArray()));
                }
                default:
                    MLogger.LogEditorError("Unrecognized prerequisite string!");
                    return false;
            }
        }

        private static bool MatchesDNF(string subSentence, int[] input) {
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

        private static bool MatchesCNF(string subSentence, int[] input) {
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