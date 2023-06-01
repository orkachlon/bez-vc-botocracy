using System;
using Core.EventSystem;
using Main.StoryPoints;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardSystem {
    
    [RequireComponent(typeof(ITraitAccessor))]
    public class MBoardModifier : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;

        private ITraitAccessor _traitAccessor;

        #region UnityMethods

        private void Awake() {
            _traitAccessor = GetComponent<ITraitAccessor>();
        }

        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        #endregion


        private void OnBoardEffect(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }

            foreach (var trait in storyEventArgs.Story.DecisionEffects.BoardEffect.Keys) {
                if (storyEventArgs.Story.DecisionEffects.BoardEffect[trait] < 0) {
                    _traitAccessor.RemoveTraitTile(trait);
                }
            }
        }
    }
}