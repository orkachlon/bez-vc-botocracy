using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Outcomes {
    public class MOutcomeExpander : MonoBehaviour, IPointerClickHandler {

        [SerializeField] private GameObject outcomeDescription;
        
        public void OnPointerClick(PointerEventData eventData) {
            outcomeDescription.SetActive(!outcomeDescription.activeInHierarchy);
            transform.Rotate(Vector3.forward, 180);
        }
    }
}