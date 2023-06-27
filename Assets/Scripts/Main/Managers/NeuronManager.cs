using System;
using Core.EventSystem;
using Events.Board;
using Events.General;
using Events.Neuron;
using Neurons.Runtime;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Main.Managers {
    public class NeuronManager : MonoBehaviour {

        [SerializeField] protected uint startNeuronAmount;

        [Header("Event Managers"), SerializeField]
        protected SEventManager gmEventManager;
        [SerializeField] protected SEventManager boardEventManager;
        [FormerlySerializedAs("neuronEvents")] [SerializeField] protected SEventManager neuronEventManager;
        
        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #region EventHandlers

        protected virtual void Init(EventArgs _) {
            // place the initial neuron
            var invulnerableBoardNeuron = NeuronFactory.GetBoardNeuron(ENeuronType.Invulnerable);
            var firstNeuronEventData = new BoardElementEventArgs<IBoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEventManager.Raise(ExternalBoardEvents.OnSetFirstElement, firstNeuronEventData);
            // add some neurons to the queue
            neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs((int) startNeuronAmount));
            // start game loop
            gmEventManager.Raise(GameManagerEvents.OnGameLoopStart, EventArgs.Empty);
        }

        #endregion
    }
}
