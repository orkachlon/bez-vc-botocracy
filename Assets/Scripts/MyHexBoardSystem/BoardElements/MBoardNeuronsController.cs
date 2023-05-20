using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Events;
using MyHexBoardSystem.BoardElements.Neuron;
using MyHexBoardSystem.UI;
using Neurons;
using UnityEngine;

namespace MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron, MUIBoardNeuron> {

        [Header("Neuron Data Provider"), SerializeField]
        private SNeuronData currentNeuronData;
        
        protected SNeuronData CurrentProvider => currentNeuronData;


        protected override void Awake() {
            base.Awake();
            
            eventManager.Register(BoardEvents.OnSetFirstNeuron, OnSetFirstNeuron);
        }

        protected override void OnClickTile(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            if (ENeuronType.Undefined.Equals(CurrentProvider.Type)) {
                return;
            }
            var element = CurrentProvider.GetElement();
            AddElement(element, hex);
            eventManager.Raise(BoardEvents.OnPlaceElement, new OnPlaceElementData<BoardNeuron>(element, hex));
            // base.OnClickTile(cell);
        }
        
        public void OnSetFirstNeuron(EventParams eventData) {
            if (eventData is not OnPlaceElementData<BoardNeuron> neuronData) {
                return;
            }
            var position = Board.GetPosition(neuronData.Hex);
            if (position == null)
                return;
            if (position.HasData()) // should never fail
                return;
            position.AddData(neuronData.Element);
            // OnAddElement?.Invoke(element, GetCellCoordinate(hex));
            DispatchOnAddElement(neuronData.Element, GetCellCoordinate(neuronData.Hex));
        }

        public override void AddElement(BoardNeuron element, Hex hex) {
            var position = Board.GetPosition(hex);
            var cell = GetCellCoordinate(hex);
            if (position == null)
                return;
            if (position.HasData()) {
                DispatchOnAddElementFailed(element, cell);
                return;
            }

            // check if any neighbors exist
            var neighbours = Manipulator.GetNeighbours(cell);
            var hasNeighbour = neighbours.Any(neighbour => Board.HasPosition(neighbour) && 
                                                           Board.GetPosition(neighbour).HasData());
            if (!hasNeighbour) {
                DispatchOnAddElementFailed(element, cell);
                return;
            }

            position.AddData(element);
            element.DataProvider.GetActivation().Invoke(this, GetCellCoordinate(hex));
            
            // dispatch event
            DispatchOnAddElement(element, cell);
        }

        public override void RemoveElement(Hex hex) {
            base.RemoveElement(hex);
        }
    }
}