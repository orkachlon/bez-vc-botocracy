using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Events;
using ExternBoardSystem.Ui.Particles;
using ExternBoardSystem.Ui.Util;
using MyHexBoardSystem.BoardElements;
using MyHexBoardSystem.BoardElements.Neuron;
using Neurons;
using Tiles;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Managers {
    public class NeuronManager : MonoBehaviour {

        [Header("Neuron Queue")]
        [SerializeField] private NeuronQueue.NeuronQueueController nextNeurons;

        [Header("Event Managers")] [SerializeField]
        private SEventManager boardEvents;

        [Header("Current Neuron Data"), SerializeField]
        private SNeuronData currentNeuronData;
        
        public BoardNeuron CurrentNeuron { get; private set; }
        public static NeuronManager Instance { get; private set; }
        public static event Action OnNeuronPlaced;
        public event Action OnNoMoreNeurons;

        private MUINeuron CurrentUINeuron { get; set; }
        private Dictionary<Neuron.ENeuronType, Neuron> _typeToPrefab;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
            }
            
            // Grid.GridDisabled += HideCurrentNeuron;
            
            // new events
            // uiTileMapInputHandler.OnClickTile += PlaceNeuron;
            // elementsController.OnPlaceElement += NextNeuron;
            boardEvents.Register(BoardEvents.OnPlaceElement, OnPlaceNeuron);
            // hover smoothly with mouse, but mark the tile below
            // uiTileMapHoverHandler.OnHoverTile += i => {};
        }

        private void Start() {
            // add some neurons to the queue
            RewardNeurons(10);
            // place the initial neuron
            var invulnerableBoardNeuron = GetNeuron(Neuron.ENeuronType.Invulnerable);
            currentNeuronData.SetData(invulnerableBoardNeuron.DataProvider);
            // elementsController.SetElementProvider(invulnerableBoardNeuron.DataProvider);
            // elementsController.AddStartingElement(invulnerableBoardNeuron, new Hex(0, 0));
            var firstNeuronEventData = new OnPlaceElementData<BoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEvents.Raise(BoardEvents.OnSetFirstNeuron, firstNeuronEventData);
            NextNeuron();
            
            currentNeuronData.SetData(CurrentNeuron.DataProvider);
            // elementsController.SetElementProvider(CurrentNeuron.DataProvider);
        }

        private void OnDestroy() {
            // Tile.OnTileClickedEvent -= PlaceNeuron;
            // Tile.OnTileMouseEnterEvent -= SnapNeuronToTile;
            // Tile.OnTileMouseExitEvent -= HideCurrentNeuron;
            // Grid.GridDisabled -= HideCurrentNeuron;
            // uiTileMapInputHandler.OnClickTile -= PlaceNeuron;
            // elementsController.OnAddElement -= NextNeuron;
        }
        
        #region EventHandlers

        private void OnPlaceNeuron(EventParams eventParams) {
            if (eventParams is OnPlaceElementData<BoardNeuron> data) {
                NextNeuron();
            }
        }

        #endregion

        private void NextNeuron() {
            // todo handle neurons being placed by other neurons correctly
            CurrentNeuron = nextNeurons.Dequeue();
            if (CurrentNeuron == null) {
                currentNeuronData.Type = Neuron.ENeuronType.Undefined;
                return;
            }
            
            if (nextNeurons.Count == 0) {
                OnNoMoreNeurons?.Invoke();
            }
            currentNeuronData.SetData(CurrentNeuron.DataProvider);
            // elementsController.SetElementProvider(CurrentNeuron.DataProvider);
        }

        private void HideCurrentNeuron(Tile tile) {
            if (CurrentNeuron == null) {
                return;
            }
            CurrentUINeuron.Hide();
        }
        
        private void HideCurrentNeuron() {
            if (CurrentNeuron == null) {
                return;
            }
            CurrentUINeuron.Hide();
        }

        private void SnapNeuronToTile(Tile tile) {
            CurrentUINeuron.transform.position = tile.transform.position;
            if (tile.IsEmpty()) {
                CurrentUINeuron.Show();
            }
            else {
                CurrentUINeuron.Hide();
            }
        }

        public void PlaceFirstNeuron(Tile tile) {
            throw new NotImplementedException("NeuronManager::PlaceFirstNeuron isn't implemented!");
        }
        
        public void RewardNeurons(int numOfNeurons) {
            nextNeurons.Enqueue(numOfNeurons);
        }

        public BoardNeuron GetNeuron(Neuron.ENeuronType neuronType) {
            var data = MNeuronTypeToBoardData.GetNeuronData(neuronType);
            return new BoardNeuron(data);
        }

        public BoardNeuron GetRandomNeuron() {
            var asArray = EnumUtil.GetValues<Neuron.ENeuronType>()
                .Where(t => t != Neuron.ENeuronType.Undefined)
                .ToArray();
            var rnd = asArray[Random.Range(0, asArray.Length)];
            var data = MNeuronTypeToBoardData.GetNeuronData(rnd);
            return new BoardNeuron(data);
        }

        public void HandleGameStateChanged(GameManager.GameState state) {
            switch (state) {
                case GameManager.GameState.InitGrid:
                case GameManager.GameState.EventTurn:
                    break;
                case GameManager.GameState.PlayerTurn:
                    break;
                case GameManager.GameState.EventEvaluation:
                    break;
                case GameManager.GameState.StatTurn:
                    break;
                case GameManager.GameState.Win:
                    break;
                case GameManager.GameState.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
