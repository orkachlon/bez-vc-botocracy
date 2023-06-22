using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.Neuron;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardSystem.Interfaces;
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

        [SerializeField] private MUINeuronPlacer placer;
        
        protected IBoardNeuron CurrentNeuron { get; private set; }


        public int CountNeurons => Board.Positions.Count(p => p.HasData());
        // For faster access to max trait
        private IDictionary<ETrait, int> NeuronsPerTrait { get; } = new Dictionary<ETrait, int>();
        private bool _placed;

        #region UnityMethods

        protected override void Awake() {
            base.Awake();
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                NeuronsPerTrait[trait] = 0;
            }
        }

        private void OnEnable() {
            externalEventManager.Register(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            externalEventManager.Register(ExternalBoardEvents.OnSingleNeuronTurn, OnSingleNeuronDone);
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        private void OnDisable() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            externalEventManager.Unregister(ExternalBoardEvents.OnSingleNeuronTurn, OnSingleNeuronDone);
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        #endregion

        #region EventHandlers

        protected override async void OnClickTile(Vector3Int cell) {
            if (CurrentNeuron == null) {
                return;
            }
            
            var hex = GetHexCoordinate(cell);
            var element = CurrentNeuron;
            
            var eventData = new BoardElementEventArgs<IBoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElementPreActivation, eventData); // disable click
            
            if (!await AddElement(element, hex)) {
                externalEventManager.Raise(ExternalBoardEvents.OnPlaceElementFailed, eventData); // enable click
                return;
            }

            await WaitForNeuron(element, 3000);

        }

        private async Task WaitForNeuron(IBoardNeuron neuron, int timeout = 250) {
            try {
                await AsyncHelpers.WaitUntil(() => neuron.TurnDone, timeout: timeout);
                var neuronArgs = new BoardElementEventArgs<IBoardNeuron>(neuron, neuron.Position);
                externalEventManager.Raise(ExternalBoardEvents.OnPlaceElement, neuronArgs);
                _placed = true;
                MLogger.LogEditor($"Placed {neuron.DataProvider.Type}");
            }
            catch (TimeoutException) {
                MLogger.LogEditor($"Timed out on waiting for {neuron.DataProvider.Type} to finish its turn.");
            }
        }

        public async void OnSetFirstNeuron(EventArgs eventData) {
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
            neuronData.Element.BindToBoard(externalEventManager, this, neuronData.Hex);
            neuronData.Element.BindToNeurons(neuronEventManager);
            DispatchOnAddElement(neuronData.Element, GetCellCoordinate(neuronData.Hex));
            await placer.AddElementAsync(neuronData.Element, GetCellCoordinate(neuronData.Hex));
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
            return RemoveNeuron(hex);
        }

        public override Task MoveElement(Hex from, Hex to) {
            return MoveNeuron(from, to);
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

            if (!IncrementTrait(hex)) {
                MLogger.LogEditor("[AddNeuron] Failed to update trait count");
                DispatchOnAddElementFailed(neuron, cell);
                return false;
            }

            position.AddData(neuron);
            neuron.BindToBoard(externalEventManager, this, hex);
            neuron.BindToNeurons(neuronEventManager);
            
            // dispatch inner event
            DispatchOnAddElement(neuron, cell);
            await placer.AddElementAsync(neuron, GetCellCoordinate(hex));
            
            // initialize neuron
            if (activate) {
                await neuron.Activate();
            }
            
            var eventData = new BoardElementEventArgs<IBoardNeuron>(neuron, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
            return true;
        }

        public async Task RemoveNeuron(Hex hex) {
            if (!Board.HasPosition(hex) || !Board.GetPosition(hex).HasData()) {
                return;
            }

            var element = Board.GetPosition(hex).Data;
            element.UnbindFromBoard();
            element.UnbindFromNeurons();
            await base.RemoveElement(hex);
            await placer.RemoveElementAsync(element);
            
            if (!DecrementTrait(hex)) {
                MLogger.LogEditor("[RemoveNeuron] Failed to update trait count");
                return;
            }
            externalEventManager.Raise(ExternalBoardEvents.OnRemoveElement, new BoardElementEventArgs<IBoardNeuron>(element, hex));
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
        }

        public async Task MoveNeuron(Hex from, Hex to, bool activate = false) {
            if (!Board.HasPosition(from) || !Board.GetPosition(from).HasData() ||
                !Board.HasPosition(to) || Board.GetPosition(to).HasData()) {
                return;
            }

            var element = Board.GetPosition(from).Data;
            await base.MoveElement(from, to);
            await placer.MoveElementAsync(element, GetCellCoordinate(from), GetCellCoordinate(to));
            if (activate) {
                await element.Activate();
            }
            
            // update traits count
            if (!DecrementTrait(from) || !IncrementTrait(to)) {
                MLogger.LogEditor("[MoveNeuron] Failed to update trait count");
                return;
            }
            externalEventManager.Raise(ExternalBoardEvents.OnMoveElement, new BoardElementMovedEventArgs<IBoardNeuron>(element, from, to));
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
        }

        #endregion

        private bool DecrementTrait(Hex hex) {
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return false;
            }

            NeuronsPerTrait[trait.Value]--;
            return true;
        }

        private bool IncrementTrait(Hex hex) {
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return false;
            }

            NeuronsPerTrait[trait.Value]++;
            return true;
        }

        private async void DispatchNeuronsAdded() {
            // await AnimationManager.WaitForAll();
            externalEventManager.Raise(ExternalBoardEvents.OnAllNeuronsDone, EventArgs.Empty);
        }
        
        private bool HasNeighbors(Vector3Int cell) {
            var neighbours = Manipulator.GetNeighbours(cell);
            var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) &&
                                                           Board.GetPosition(neighbour).HasData());
            return hasNeighbour;
        }

        private async void OnSingleNeuronDone(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> neuronArgs) {
                return;
            }

            MLogger.LogEditor($"{neuronArgs.Element.DataProvider.Type} done!");
            if (Board.Positions.Where(p => p.HasData()).Any(p => !p.Data.TurnDone)) {
                return;
            }

            await AsyncHelpers.WaitUntil(() => _placed);
            MLogger.LogEditor("All neurons done!");
            DispatchNeuronsAdded();
            _placed = false;
        }
    }
}