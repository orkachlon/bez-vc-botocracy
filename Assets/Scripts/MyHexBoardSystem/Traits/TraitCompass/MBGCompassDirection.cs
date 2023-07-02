using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Board;
using Events.SP;
using Types.Trait;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyHexBoardSystem.Traits.TraitCompass {
    public class MBGCompassDirection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField] protected ETrait trait;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;

        private bool fromLabel;
        private bool isInsideCollider;

        private bool HasEffect { get; set; }
        public bool IsEnabled { get; set; } = true;

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnBeforeEvaluate, OnBeforeEvaluate);
            storyEventManager.Register(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnBeforeEvaluate, OnBeforeEvaluate);
            storyEventManager.Unregister(StoryEvents.OnInitStory, OnInitStory);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!IsEnabled) {
                return;
            }
            if (fromLabel) {
                fromLabel = false;
                return;
            }
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnterStatic, new TraitCompassHoverEventArgs(trait));
            if (HasEffect) {
                boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassEnter, new TraitCompassHoverEventArgs(trait));
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!IsEnabled) {
                return;
            }

            fromLabel = isInsideCollider;
            if (fromLabel) {
                return;
            }
            DispatchPointerExit();
        }

        protected virtual void DispatchPointerExit() {
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExitStatic, new TraitCompassHoverEventArgs(trait));
            if (HasEffect) {
                boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
            }
        }

        private void OnMouseEnter() {
            isInsideCollider = true;
        }

        private void OnMouseExit() {
            isInsideCollider = false;
        }

        private void OnMouseOver() {
            // if over board - isinsidecol = false;
            var eventData = new PointerEventData(EventSystem.current) {
                position = UnityEngine.Input.mousePosition
            };
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            if (raycastResults.Any(r => r.gameObject.CompareTag("Board"))) {
                isInsideCollider = false;
                DispatchPointerExit();
            }
        }

        #region EventHandlers

        private void OnInitStory(EventArgs obj) {
            if (obj is not StoryEventArgs spArgs) {
                return;
            }

            HasEffect = spArgs.Story.DecidingTraits.ContainsKey(trait);
        }
        private void OnBeforeEvaluate(EventArgs obj) {
            IsEnabled = false;
            boardEventManager.Raise(ExternalBoardEvents.OnTraitCompassExit, new TraitCompassHoverEventArgs(trait));
        }

        private void OnBoardModified(EventArgs obj) {
            IsEnabled = true;
        }

        #endregion
    }
}