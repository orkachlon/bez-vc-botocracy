﻿using System;
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

        [SerializeField] private uint startNeuronAmount;

        [Header("Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [FormerlySerializedAs("neuronEvents")] [SerializeField] private SEventManager neuronEventManager;
        
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
