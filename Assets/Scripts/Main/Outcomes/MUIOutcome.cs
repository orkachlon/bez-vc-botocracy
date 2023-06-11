using TMPro;
using UnityEngine;

namespace Main.Outcomes {
    public class MUIOutcome : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI spTitleText;
        [SerializeField] private TextMeshProUGUI spDeciderText;
        [SerializeField] private TextMeshProUGUI spDecisionText;
        [SerializeField] private TextMeshProUGUI outcomeText;

        public void SetSPTitle(string title) {
            spTitleText.text = title;
        }
        
        public void SetDecider(string decider) {
            spDeciderText.text = decider;
        }

        public void SetDecision(string decision) {
            spDecisionText.text = decision;
        }
        
        public void SetOutcomeText(string text) {
            outcomeText.text = text;
        }
    }
}