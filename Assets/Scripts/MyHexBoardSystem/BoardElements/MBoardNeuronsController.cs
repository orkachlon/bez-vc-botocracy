using System;
using System.Linq;
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
            
            externalEventManager.Register(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
        }

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
            var eventData = new OnPlaceElementEventArgs<BoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnPlaceElement, eventData);
            // externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            // base.OnClickTile(cell);
        }
        
        public void OnSetFirstNeuron(EventArgs eventData) {
            if (eventData is not OnPlaceElementEventArgs<BoardNeuron> neuronData) {
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
        }

        public override bool AddElement(BoardNeuron element, Hex hex) {
            var position = Board.GetPosition(hex);
            var cell = GetCellCoordinate(hex);
            if (position == null)
                return false;
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

            position.AddData(element);
            element.DataProvider.GetActivation().Invoke(this, GetCellCoordinate(hex));
            
            // dispatch event
            DispatchOnAddElement(element, cell);
            
            var eventData = new OnPlaceElementEventArgs<BoardNeuron>(element, hex);
            externalEventManager.Raise(ExternalBoardEvents.OnAddElement, eventData);
            return true;
        }

        public override void RemoveElement(Hex hex) {
            base.RemoveElement(hex);
            // var eventData = new OnPlaceElementEventArgs<BoardNeuron>(element, hex);

        }
    }
}