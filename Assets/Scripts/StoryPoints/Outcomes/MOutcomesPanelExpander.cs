﻿using System;
using Core.EventSystem;
using Events.UI;
using Types.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryPoints.Outcomes {
    public class MOutcomesPanelExpander : MonoBehaviour, IPointerClickHandler {

        [Header("Event Managers"), SerializeField]
        private SEventManager uiEventManager;

        private IExpandable _outcomesContainer;

        private bool _isExpanded;
        private RectTransform _bottomOutcomeRect;

        private void Start() {
            _outcomesContainer = transform.parent.GetComponent<MOutcomesController>();
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
            transform.eulerAngles = Vector3.forward * 270 - transform.rotation.eulerAngles;
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