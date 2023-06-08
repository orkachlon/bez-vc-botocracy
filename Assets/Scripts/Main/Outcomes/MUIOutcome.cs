using TMPro;
using UnityEngine;

namespace Main.Outcomes {
    public class MUIOutcome : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI outcomeDecider;
        [SerializeField] private TextMeshProUGUI outcomeText;
        
        public void SetText(string text) {
            outcomeText.text = text;
        }

        public void SetDecider(string decider) {
            outcomeDecider.text = decider;
        }
    }
}