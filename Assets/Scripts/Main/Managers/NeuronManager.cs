using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Events;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.Neurons;
using Main.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Managers {
    public class NeuronManager : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager neuronEvents;

        [Header("Current Neuron Data"), SerializeField]
        private SNeuronData currentNeuronData;
        
        public BoardNeuron CurrentNeuron { get; private set; }

        private MUINeuron CurrentUINeuron { get; set; }
        private Dictionary<ENeuronType, Neuron> _typeToPrefab;

        private void Awake() {
            neuronEvents.Register(NeuronEvents.OnDequeueNeuron, OnDequeueNeuron);
            neuronEvents.Register(NeuronEvents.OnNoMoreNeurons, OnNoMoreNeurons);
            // hover smoothly with mouse, but mark the tile below
            // uiTileMapHoverHandler.OnHoverTile += i => {};
        }

        private void Start() {
            // add some neurons to the queue
            neuronEvents.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(10));
            // place the initial neuron
            var invulnerableBoardNeuron = GetNeuron(ENeuronType.Invulnerable);
            currentNeuronData.SetData(invulnerableBoardNeuron.DataProvider);
            var firstNeuronEventData = new OnPlaceElementEventArgs<BoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEventManager.Raise(ExternalBoardEvents.OnSetFirstElement, firstNeuronEventData);
            gmEventManager.Raise(GameManagerEvents.OnGameLoopStart, EventArgs.Empty);
        }

        private void OnDestroy() {
            neuronEvents.Unregister(NeuronEvents.OnDequeueNeuron, OnDequeueNeuron);
            neuronEvents.Unregister(NeuronEvents.OnNoMoreNeurons, OnNoMoreNeurons);
        }
        
        #region EventHandlers

        private void OnNoMoreNeurons(EventArgs eventParams) {
            if (eventParams is not NeuronEventArgs)
                return;
            CurrentNeuron = null;
            currentNeuronData.Type = ENeuronType.Undefined;
        }

        private void OnDequeueNeuron(EventArgs eventParams) {
            if (eventParams is NeuronEventArgs data) {
                NextNeuron(data.Neuron);
            }
        }

        #endregion

        private void NextNeuron(BoardNeuron nextNeuron) {
            // todo handle neurons being placed by other neurons correctly
            CurrentNeuron = nextNeuron;
            if (CurrentNeuron == null) {
                currentNeuronData.Type = ENeuronType.Undefined;
                return;
            }
            
            currentNeuronData.SetData(CurrentNeuron.DataProvider);
            neuronEvents.Raise(NeuronEvents.OnNeuronPlaced, new NeuronEventArgs(CurrentNeuron));
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
    }
}
