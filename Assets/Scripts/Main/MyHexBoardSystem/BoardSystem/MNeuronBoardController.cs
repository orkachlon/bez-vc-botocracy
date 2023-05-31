using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.StoryPoints;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardSystem {
    public class MNeuronBoardController : MBoardController<BoardNeuron> {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        
        
        private void OnEnable() {
            storyEventManager.Register(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, OnBoardEffect);
        }

        #region EventHandlers

        private void OnBoardEffect(EventArgs obj) {
            if (obj is not StoryEventArgs storyEventArgs) {
                return;
            }
            // todo add effect on board
            print(storyEventArgs.Story.Description);
        }

        #endregion
    }
}