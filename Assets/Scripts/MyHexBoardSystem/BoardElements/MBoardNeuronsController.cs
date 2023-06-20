using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Animation;
using Core.EventSystem;
using Core.Utils;
using Events.Neuron;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem.Interfaces;
using MyHexBoardSystem.Events;
using Types.Board;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<IBoardNeuron, IUIBoardNeuron> , IBoardNeuronsController {
        [Header("Neuron Board Event Managers"), SerializeField]
        private SEventManager neuronEventManager;
        
        protected IBoardNeuron CurrentNeuron { get; private set; }


        public int CountNeurons => Board.Positions.Count(p => p.HasData());
        // For faster access to max trait
        private IDictionary<ETrait, int> NeuronsPerTrait { get; } = new Dictionary<ETrait, int>();

        #region UnityMethods

        protected override void Awake() {
            base.Awake();
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                NeuronsPerTrait[trait] = 0;
            }
        }

        private void OnEnable() {
            externalEventManager.Register(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        private void OnDisable() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        #endregion

        #region EventHandlers

        protected override async void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (CurrentNeuron == null) {
                return;
            }

            // use the static data instead of the CurrentProvider data which is going to change every time
            var element = CurrentNeuron;
            if (!await AddElement(element, hex)) {
                return;
            }
            var eventData = new BoardElementEventArgs<IBoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnPlaceElement, eventData);
            DispatchNeuronsAdded();
            // base.OnClickTile(cell);
        }

        public void OnSetFirstNeuron(EventArgs eventData) {
            if (eventData is not BoardElementEventArgs<IBoardNeuron> neuronData) {
                MLogger.LogEditor($"Event args type mismatch. Actual: {eventData.GetType()} != Expected: {typeof(BoardElementEventArgs<IBoardNeuron>)}");
                return;
            }
            var position = Board.GetPosition(neuronData.Hex);
            if (position == null)
                return;
            if (position.HasData()) // should never fail
                return;
            position.AddData(neuronData.Element);
            DispatchOnAddElement(neuronData.Element, GetCellCoordinate(neuronData.Hex));
            neuronData.Element.BindToNeuronManager(neuronEventManager);
            neuronData.Element.BindToBoard(externalEventManager, this, neuronData.Hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData); // ?
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
        }

        private void UpdateNextNeuron(EventArgs obj) {
            if (obj is not NeuronQueueEventArgs queueEventArgs) {
                return;
            }

            CurrentNeuron = queueEventArgs.NeuronQueue.NextBoardNeuron;
        }

        #endregion

        #region InterfaceMethods

        public override Task<bool> AddElement(IBoardNeuron element, Hex hex) {
            return AddNeuron(element, hex);
        }

        public override Task RemoveElement(Hex hex) {
            if (!Board.HasPosition(hex) || !Board.GetPosition(hex).HasData()) {
                return Task.CompletedTask;
            }

            var element = Board.GetPosition(hex).Data;
            base.RemoveElement(hex);
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return Task.CompletedTask;
            }
            NeuronsPerTrait[trait.Value]--;
            externalEventManager.Raise(ExternalBoardEvents.OnRemoveElement, new BoardElementEventArgs<IBoardNeuron>(element, hex));
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
            return Task.CompletedTask;
        }

        public override Task MoveElement(Hex from, Hex to) {
            MoveNeuron(from, to);
            return Task.CompletedTask;
        }

        public int GetTraitCount(ETrait trait) {
            return NeuronsPerTrait[trait];
        }

        public IEnumerable<ETrait> GetMaxTrait(IEnumerable<ETrait> fromTraits = null) {
            fromTraits ??= EnumUtil.GetValues<ETrait>();
            var traits = fromTraits as ETrait[] ?? fromTraits.ToArray();
            var max = NeuronsPerTrait
                .Where(kvp => traits.Contains(kvp.Key))
                .Max(kvp => kvp.Value);
            return NeuronsPerTrait
                .Where(kvp => traits.Contains(kvp.Key) && kvp.Value == max)
                .Select(kvp => kvp.Key);
        }

        public async Task<bool> AddNeuron(IBoardNeuron neuron, Hex hex, bool activate = true) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return false;
            var cell = GetCellCoordinate(hex);
            if (position.HasData()) {
                DispatchOnAddElementFailed(neuron, cell);
                return false;
            }

            // check if any neighbors exist
            if (!HasNeighbors(cell)) {
                DispatchOnAddElementFailed(neuron, cell);
                return false;
            }

            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                DispatchOnAddElementFailed(neuron, cell);
                return false;
            }
            NeuronsPerTrait[trait.Value]++;

            position.AddData(neuron);
            
            // dispatch inner event
            DispatchOnAddElement(neuron, cell);

            // initialize neuron
            neuron.BindToNeuronManager(neuronEventManager);
            neuron.BindToBoard(externalEventManager, this, hex);
            if (activate) {
                await neuron.Activate();
            }
            
            var eventData = new BoardElementEventArgs<IBoardNeuron>(neuron, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
            return true;
        }

        public Task RemoveNeuron(Hex hex) {
            if (!Board.HasPosition(hex) || !Board.GetPosition(hex).HasData()) {
                return Task.CompletedTask;
            }

            RemoveElement(hex);
            return Task.CompletedTask;
        }

        public Task MoveNeuron(Hex from, Hex to, bool activate = false) {
            if (!Board.HasPosition(from) || !Board.GetPosition(from).HasData() ||
                !Board.HasPosition(to) || Board.GetPosition(to).HasData()) {
                return Task.CompletedTask;
            }

            var element = Board.GetPosition(from).Data;
            base.MoveElement(from, to);
            if (activate) {
                element.Activate();
            }
            externalEventManager.Raise(ExternalBoardEvents.OnMoveElement, new BoardElementMovedEventArgs<IBoardNeuron>(element, from, to));
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
            
            // update traits count
            var fromTrait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(from));
            var toTrait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(to));
            if (!fromTrait.HasValue || !toTrait.HasValue || fromTrait == toTrait) {
                return Task.CompletedTask;
            }

            NeuronsPerTrait[fromTrait.Value]--;
            NeuronsPerTrait[toTrait.Value]++;
            return Task.CompletedTask;
        }
    #endregion
    
    private async void DispatchNeuronsAdded() {
        await AnimationManager.WaitForAll();
        externalEventManager.Raise(ExternalBoardEvents.OnAllNeuronsDone, EventArgs.Empty);
    }

    private bool HasNeighbors(Vector3Int cell) {
        var neighbours = Manipulator.GetNeighbours(cell);
        var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) &&
                                                       Board.GetPosition(neighbour).HasData());
        return hasNeighbour;
    }
    }
}