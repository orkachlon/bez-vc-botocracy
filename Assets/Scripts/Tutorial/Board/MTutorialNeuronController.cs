using System;
using Core.EventSystem;
using MyHexBoardSystem.BoardElements;
using Types.Tutorial;
using UnityEngine;

namespace Tutorial.Board {
    public class MTutorialNeuronController : MBoardNeuronsController, ITutorialNeuronController {
        
        [Header("Neuron Board Event Managers"), SerializeField] private SEventManager tutorialEventManager;
        
        #region UnityMethods

        // protected override void OnEnable() {
        // }
        //
        // protected override void OnDisable() {
        // }

        #endregion

        #region EventHandlers

        // protected override void OnSetFirstNeuron(EventArgs eventData) {
        // }

        // protected override void UpdateNextNeuron(EventArgs obj) {
        // }

        protected override void OnSingleNeuronDone(EventArgs args) {
        }

        #endregion
    }
}