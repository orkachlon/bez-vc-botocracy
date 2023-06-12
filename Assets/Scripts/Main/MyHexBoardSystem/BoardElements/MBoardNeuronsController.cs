using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Neurons;
using Main.Neurons.Data;
using Main.Neurons.Runtime;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron, MUIBoardNeuron> , IBoardNeuronsController {

        [Header("Neuron Data Provider"), SerializeField]
        private SNeuronDataBase currentNeuronData;

        [Header("Neuron Board Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        
        protected SNeuronDataBase CurrentProvider => currentNeuronData;


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
        }

        private void OnDisable() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
        }

        #endregion

        protected override void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (ENeuronType.Undefined.Equals(CurrentProvider.Type)) {
                return;
            }

            // use the static data instead of the CurrentProvider data which is going to change every time
            var element = NeuronFactory.GetBoardNeuron(CurrentProvider.Type);
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
            return AddNeuron(element, hex);
        }

        private bool HasNeighbors(Vector3Int cell) {
            var neighbours = Manipulator.GetNeighbours(cell);
            var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) &&
                                                           Board.GetPosition(neighbour).HasData());
            return hasNeighbour;
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

        public bool AddNeuron(BoardNeuron neuron, Hex hex, bool activate = true) {
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

            if (activate) {
                neuron.Activate(externalEventManager, this, cell);
            }
            
            var eventData = new BoardElementEventArgs<BoardNeuron>(neuron, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
            return true;
        }
    }
}