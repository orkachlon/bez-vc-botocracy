using TMPro;
using UnityEngine;

namespace Main.StoryPoints {
    public class MUIOutcome : MonoBehaviour {
        [Header("Visuals"), SerializeField] private TextMeshProUGUI outcomeText;
        
        public void SetText(string text) {
            outcomeText.text = text;
        }
    }
}