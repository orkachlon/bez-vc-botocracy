using System;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.Managers {
    public class NeuronManager : MonoBehaviour {

        [SerializeField] private uint startNeuronAmount;

        [Header("Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager neuronEvents;
        
        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #region EventHandlers

        private void Init(EventArgs _) {
            // place the initial neuron
            var invulnerableBoardNeuron = NeuronFactory.GetBoardNeuron(ENeuronType.Invulnerable);
            var firstNeuronEventData = new BoardElementEventArgs<BoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEventManager.Raise(ExternalBoardEvents.OnSetFirstElement, firstNeuronEventData);
            // add some neurons to the queue
            neuronEvents.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs((int) startNeuronAmount));
            // start game loop
            gmEventManager.Raise(GameManagerEvents.OnGameLoopStart, EventArgs.Empty);
        }

        #endregion
    }
}
