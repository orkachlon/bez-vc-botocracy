using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.MyHexBoardSystem.UI;
using Main.Neurons;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron, MUIBoardNeuron> , IBoardNeuronsController {

        [Header("Neuron Data Provider"), SerializeField]
        private SNeuronData currentNeuronData;

        [Header("Neuron Board Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        
        protected SNeuronData CurrentProvider => currentNeuronData;


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
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnBoardStateBroadcast);
        }

        private void OnDisable() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnBoardStateBroadcast);
        }

        #endregion

        protected override void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (ENeuronType.Undefined.Equals(CurrentProvider.Type)) {
                return;
            }

            // use the static data instead of the CurrentProvider data which is going to change every time
            var dataCopy = MNeuronTypeToBoardData.GetNeuronData(CurrentProvider.Type);
            var element = new BoardNeuron(dataCopy);
            if (!AddElement(element, hex)) {
                return;
            }
            var eventData = new BoardElementEventArgs<BoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnPlaceElement, eventData);
            // base.OnClickTile(cell);
        }

        public void OnSetFirstNeuron(EventArgs eventData) {
            if (eventData is not BoardElementEventArgs<BoardNeuron> neuronData) {
                return;
            }
            var position = Board.GetPosition(neuronData.Hex);
            if (position == null)
                return;
            if (position.HasData()) // should never fail
                return;
            position.AddData(neuronData.Element);
            DispatchOnAddElement(neuronData.Element, GetCellCoordinate(neuronData.Hex));
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData); // ?
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
        }

        public override bool AddElement(BoardNeuron element, Hex hex) {
            var position = Board.GetPosition(hex);
            if (position == null)
                return false;
            var cell = GetCellCoordinate(hex);
            if (position.HasData()) {
                DispatchOnAddElementFailed(element, cell);
                return false;
            }

            // check if any neighbors exist
            var neighbours = Manipulator.GetNeighbours(cell);
            var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) && 
                                                           Board.GetPosition(neighbour).HasData());
            if (!hasNeighbour) {
                DispatchOnAddElementFailed(element, cell);
                return false;
            }

            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                DispatchOnAddElementFailed(element, cell);
                return false;
            }

            position.AddData(element);
            NeuronsPerTrait[trait.Value]++;
            // dispatch inner event
            DispatchOnAddElement(element, cell);
            
            if (element.DataProvider.GetActivation() != null) {
                element.DataProvider.GetActivation().Invoke(this, cell);
                //     ActivateNeuron(element.DataProvider.GetActivation(), cell);
            }
            
            var eventData = new BoardElementEventArgs<BoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
            return true;
        }

        public override void RemoveElement(Hex hex) {
            if (!Board.HasPosition(hex) || !Board.GetPosition(hex).HasData()) {
                return;
            }

            var element = Board.GetPosition(hex).Data;
            base.RemoveElement(hex);
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return;
            }
            NeuronsPerTrait[trait.Value]--;
            externalEventManager.Raise(ExternalBoardEvents.OnRemoveElement, new BoardElementEventArgs<BoardNeuron>(element, hex));
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
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

        #region EventHandlers

        private void OnBoardStateBroadcast(EventArgs args) {
            if (args is not GameStateEventArgs {State: GameState.BoardStateBroadcast}) {
                return;
            }
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
        }

        #endregion

        private async void ActivateNeuron(Func<IBoardElementsController<BoardNeuron>,Vector3Int,Task> activation, Vector3Int cell) {
            await activation.Invoke(this, cell);
        }
    }
}