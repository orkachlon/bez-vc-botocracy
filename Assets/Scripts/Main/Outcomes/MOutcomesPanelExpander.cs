using System;
using Core.EventSystem;
using DG.Tweening;
using Main.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Outcomes {
    public class MOutcomesPanelExpander : MonoBehaviour, IPointerClickHandler {

        [Header("Event Managers"), SerializeField]
        private SEventManager uiEventManager;

        private IExpandable _outcomesContainer;

        private bool _isExpanded;
        private RectTransform _bottomOutcomeRect;

        private void Start() {
            _outcomesContainer = transform.parent.GetComponent<MOutcomesController>();
            UpdateSize();
        }

        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnOutcomeExpanded, UpdateSize);
            uiEventManager.Register(UIEvents.OnOutcomeAdded, UpdateSize);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnOutcomeExpanded, UpdateSize);
            uiEventManager.Unregister(UIEvents.OnOutcomeAdded, UpdateSize);
        }

        public void OnPointerClick(PointerEventData eventData) {
            _isExpanded = !_isExpanded;
            transform.DORotate(transform.rotation.eulerAngles + Vector3.forward * 180, 0.75f, RotateMode.FastBeyond360);
            UpdateSize();
            uiEventManager.Raise(UIEvents.OnOutcomesPanelExpanded, new UIExpandEventArgs(_isExpanded));
        }

        private void UpdateSize(EventArgs args = null) {
            if (_isExpanded) {
                _outcomesContainer.Expand();
            }
            else {
                _outcomesContainer.Collapse();
            }
        }
    }
}