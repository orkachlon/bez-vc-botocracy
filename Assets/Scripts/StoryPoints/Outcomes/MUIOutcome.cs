using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StoryPoints.Outcomes {
    public class MUIOutcome : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI spTitleText;
        [SerializeField] private TextMeshProUGUI spDeciderText;
        [SerializeField] private TextMeshProUGUI spDecisionText;
        [SerializeField] private TextMeshProUGUI outcomeText;
        [SerializeField] private Image artwork;

        private const string DeciderPrefix = "<size=80%>Decided by:</size>\n";
        private const string DecisionPrefix = "<font=\"EzerBlock Bold SDF\">Action: </font>";
        private const string OutcomePrefix = "<font=\"EzerBlock Bold SDF\">Outcome: </font>";

        public void SetSPTitle(string title) {
            spTitleText.text = title;
        }
        
        public void SetDecider(string decider) {
            spDeciderText.text = $"{DeciderPrefix}{decider}";
        }

        public void SetDecision(string decision) {
            spDecisionText.text = $"{DecisionPrefix}{decision}";
        }
        
        public void SetOutcomeText(string outcome) {
            outcomeText.text = $"{OutcomePrefix}{outcome}";
        }

        public void SetArtwork(Sprite image) {
            artwork.sprite = image;
        }
    }
}