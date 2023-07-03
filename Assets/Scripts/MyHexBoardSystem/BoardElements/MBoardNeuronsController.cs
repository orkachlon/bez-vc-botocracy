using System;
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
using MyHexBoardSystem.BoardSystem;
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
        private ITraitAccessor _traitAccessor;
        private bool _placed;

        #region UnityMethods

        protected override void Awake() {
            base.Awake();
            _traitAccessor = GetComponent<ITraitAccessor>();
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                NeuronsPerTrait[trait] = 0;
            }
        }

        protected virtual void OnEnable() {
            externalEventManager.Register(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            externalEventManager.Register(ExternalBoardEvents.OnSingleNeuronTurn, OnSingleNeuronDone);
            neuronEventManager.Register(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        protected virtual void OnDisable() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            externalEventManager.Unregister(ExternalBoardEvents.OnSingleNeuronTurn, OnSingleNeuronDone);
            neuronEventManager.Unregister(NeuronEvents.OnQueueStateChanged, UpdateNextNeuron);
        }

        #endregion

        #region EventHandlers

        protected virtual async void OnSetFirstNeuron(EventArgs eventData) {
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

        protected virtual void UpdateNextNeuron(EventArgs obj) {
            if (obj is not NeuronQueueEventArgs queueEventArgs) {
                return;
            }

            CurrentNeuron = queueEventArgs.NeuronQueue.NextBoardNeuron;
        }

        protected virtual async void OnSingleNeuronDone(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron> neuronArgs) {
                return;
            }

            //MLogger.LogEditor($"{neuronArgs.Element.DataProvider.Type} done!");
            if (Board.Positions.Where(p => p.HasData()).Any(p => !p.Data.TurnDone)) {
                return;
            }

            await AsyncHelpers.WaitUntil(() => _placed);
            MLogger.LogEditor("All neurons done!");
            DispatchNeuronsAdded();
            _placed = false;
        }

        #endregion

        #region InterfaceMethods

        public override Task AddElement(IBoardNeuron element, Hex hex) {
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

        public async Task AddNeuron(IBoardNeuron neuron, Hex hex, bool activate = true) {
            var position = Board.GetPosition(hex);
            var cell = GetCellCoordinate(hex);
            if (position == null || position.HasData()) {
                DispatchOnAddElementFailed(neuron, cell);
                return; 
            }

            // check if any neighbors exist
            if (!HasNeighbors(cell)) {
                DispatchOnAddElementFailed(neuron, cell);
                return;
            }

            if (!IncrementTrait(hex)) {
                MLogger.LogEditor("[AddNeuron] Failed to update trait count");
                DispatchOnAddElementFailed(neuron, cell);
                return;
            }

            position.AddData(neuron);
            neuron.BindToBoard(externalEventManager, this, hex);
            neuron.BindToNeurons(neuronEventManager);
            
            // dispatch inner event
            DispatchOnAddElement(neuron, cell);
            // tile related actions
            externalEventManager.Raise(ExternalBoardEvents.OnTileOccupied, new BoardElementEventArgs<IBoardNeuron>(neuron, hex));
            // wait for neuron add animation and connections
            await placer.AddElementAsync(neuron, GetCellCoordinate(hex));
            
            // initialize neuron
            if (activate) {
                await neuron.Activate();
            }
            
            var eventData = new BoardElementEventArgs<IBoardNeuron>(neuron, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));
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
            // unpress tile
            externalEventManager.Raise(ExternalBoardEvents.OnTileUnoccupied, new BoardElementEventArgs<IBoardNeuron>(element, hex));
            
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
            // press tiles
            externalEventManager.Raise(ExternalBoardEvents.OnTileUnoccupied, new BoardElementEventArgs<IBoardNeuron>(element, from));
            // wait for animation and connection
            await placer.MoveElementAsync(element, GetCellCoordinate(from), GetCellCoordinate(to));
            externalEventManager.Raise(ExternalBoardEvents.OnTileOccupied, new BoardElementEventArgs<IBoardNeuron>(element, to));
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

        protected override async void OnClickTile(Vector3Int cell) {
            if (CurrentNeuron == null) {
                return;
            }

            var hex = GetHexCoordinate(cell);
            var element = CurrentNeuron;
            var eventData = new BoardElementEventArgs<IBoardNeuron>(element, hex);

            
            // check position validity
            var position = Board.GetPosition(hex);
            if (position == null || position.HasData() || !HasNeighbors(cell) || !IncrementTrait(hex) || !position.IsEnabled) {
                DispatchOnAddElementFailed(element, cell);
                return;
            }

            position.AddData(element);
            element.BindToBoard(externalEventManager, this, hex);
            element.BindToNeurons(neuronEventManager);

            // dispatch inner event (does nothing rn)
            DispatchOnAddElement(element, cell);
            // tile related actions
            externalEventManager.Raise(ExternalBoardEvents.OnTileOccupied, new BoardElementEventArgs<IBoardNeuron>(element, hex));
            // dispatch pre activation event
            externalEventManager.Raise(ExternalBoardEvents.OnPlaceElementPreActivation, eventData); // disable click
            // wait for neuron add animation and connections
            await placer.AddElementAsync(element, GetCellCoordinate(hex));
            await element.Activate();

            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new BoardStateEventArgs(this));

            await WaitForNeuron(element, 3000);
        }

        private async Task WaitForNeuron(IBoardNeuron neuron, int timeout = 250) {
            try {
                await AsyncHelpers.WaitUntil(() => neuron.TurnDone, timeout: timeout);
                var neuronArgs = new BoardElementEventArgs<IBoardNeuron>(neuron, neuron.Position);
                externalEventManager.Raise(ExternalBoardEvents.OnPlaceElementTurnDone, neuronArgs);
                _placed = true;
            }
            catch (TimeoutException) {
                MLogger.LogEditor($"Timed out on waiting for {neuron.DataProvider.Type} to finish its turn.");
            }
        }

        #region Traits

        protected bool DecrementTrait(Hex hex) {
            var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return false;
            }

            NeuronsPerTrait[trait.Value]--;
            return true;
        }

        protected bool IncrementTrait(Hex hex) {
            var trait = _traitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return false;
            }

            NeuronsPerTrait[trait.Value]++;
            return true;
        } 

        #endregion

        private void DispatchNeuronsAdded() {
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