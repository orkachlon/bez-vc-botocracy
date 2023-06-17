using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryPoints.Outcomes {
    public class MCloseDecisionButton : MonoBehaviour, IPointerClickHandler {

        [SerializeField] private MDecisionPopup decisionPopup;
        
        public void OnPointerClick(PointerEventData eventData) {
            print("CLICKED");
            if (!decisionPopup) {
                return;
            }
            decisionPopup.Hide();
        }
    }
}