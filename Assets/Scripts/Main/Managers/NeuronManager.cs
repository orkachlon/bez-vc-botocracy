using System;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.StoryPoints;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Managers {
    public class NeuronManager : MonoBehaviour, IGameStateResponder {

        [Header("Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager neuronEvents;
        [SerializeField] private SEventManager storyEventManager;

        [Header("Current Neuron Data"), SerializeField]
        private SNeuronData currentNeuronData;

        private BoardNeuron CurrentNeuron { get; set; }

        private void OnEnable() {
            neuronEvents.Register(NeuronEvents.OnDequeueNeuron, OnDequeueNeuron);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnGameStateChanged);
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        private void OnDisable() {
            neuronEvents.Unregister(NeuronEvents.OnDequeueNeuron, OnDequeueNeuron);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #region EventHandlers

        private void Init(EventArgs _) {
            // add some neurons to the queue
            neuronEvents.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(10));
            // place the initial neuron
            var invulnerableBoardNeuron = GetNeuron(ENeuronType.Invulnerable);
            currentNeuronData.SetData(invulnerableBoardNeuron.DataProvider);
            var firstNeuronEventData = new BoardElementEventArgs<BoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEventManager.Raise(ExternalBoardEvents.OnSetFirstElement, firstNeuronEventData);
            gmEventManager.Raise(GameManagerEvents.OnGameLoopStart, EventArgs.Empty);
        }

        private void DisableBoardInteraction() {
            CurrentNeuron = null;
            currentNeuronData.Type = ENeuronType.Undefined;
        }

        private void OnDequeueNeuron(EventArgs eventParams) {
            if (eventParams is NeuronQueueEventArgs data) {
                NextNeuron(data.NeuronQueue.Peek());
            }
        }

        private void OnGameStateChanged(EventArgs eventArgs) {
            if (eventArgs is not GameStateEventArgs stateEventArgs) {
                return;
            }
            HandleAfterGameStateChanged(stateEventArgs.State, stateEventArgs.CustomArgs);
        }

        #endregion

        private void NextNeuron(BoardNeuron nextNeuron) {
            neuronEvents.Raise(NeuronEvents.OnNeuronPlaced, new BoardNeuronEventArgs(CurrentNeuron));
            
            CurrentNeuron = nextNeuron;
            if (CurrentNeuron == null) {
                currentNeuronData.Type = ENeuronType.Undefined;
                return;
            }
            currentNeuronData.SetData(CurrentNeuron.DataProvider);
        }
        
        public static BoardNeuron GetNeuron(ENeuronType neuronType) {
            var data = MNeuronTypeToBoardData.GetNeuronData(neuronType);
            return new BoardNeuron(data);
        }

        public static BoardNeuron GetRandomNeuron() {
            var asArray = EnumUtil.GetValues<ENeuronType>()
                .Where(t => t != ENeuronType.Undefined)
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            var data = MNeuronTypeToBoardData.GetNeuronData(rnd);
            return new BoardNeuron(data);
        }

        public void HandleAfterGameStateChanged(GameState state, EventArgs customArgs = null) {
            if (state is GameState.Lose or GameState.Win) {
                DisableBoardInteraction();
            }
        }
    }
}
