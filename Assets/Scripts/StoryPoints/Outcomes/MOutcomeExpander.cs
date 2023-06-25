using Animation;
using Core.EventSystem;
using DG.Tweening;
using Events.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryPoints.Outcomes {
    public class MOutcomeExpander : MonoBehaviour, IPointerClickHandler {

        [SerializeField] private GameObject outcomeDescription;

        [Header("Event Managers"), SerializeField]
        private SEventManager uiEventManager;
        
        public void OnPointerClick(PointerEventData eventData) {
            outcomeDescription.SetActive(!outcomeDescription.activeInHierarchy);
            transform.DORotate(transform.rotation.eulerAngles + Vector3.forward * 180, 0.75f, RotateMode.FastBeyond360);
            MCoroutineHelper.InvokeAfterNextFrame(() => uiEventManager.Raise(UIEvents.OnOutcomeExpanded, new UIExpandEventArgs(outcomeDescription.activeInHierarchy)));
        }
    }
}