using System;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Events;
using Main.Managers;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem;
using Main.MyHexBoardSystem.UI;
using Main.Neurons;
using Main.Traits;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements {
    public class MBoardNeuronsController : MBoardElementsController<BoardNeuron, MUIBoardNeuron> , IBoardNeuronController {

        [Header("Neuron Data Provider"), SerializeField]
        private SNeuronData currentNeuronData;

        [Header("Neuron Board Event Managers"), SerializeField]
        private SEventManager gmEventManager;
        
        protected SNeuronData CurrentProvider => currentNeuronData;


        protected override void Awake() {
            base.Awake();
            externalEventManager.Register(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnBoardStateBroadcast);
        }

        private void OnDestroy() {
            externalEventManager.Unregister(ExternalBoardEvents.OnSetFirstElement, OnSetFirstNeuron);
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnBoardStateBroadcast);
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

        public int GetTraitOverall(ETraitType trait) {
            return Manipulator.GetTriangle(TraitToDirection(trait)).Count(h => Board.HasPosition(h));
        }

        public int GetTraitCount(ETraitType trait) {
            var direction = TraitToDirection(trait);

            return Manipulator.GetTriangle(direction)
                .Select(h => Board.GetPosition(h))
                .Count(p => p != null && p.HasData());
        }

        private static Hex TraitToDirection(ETraitType trait) {
            var direction = trait switch {
                ETraitType.Defender => new Hex(0, 1),
                ETraitType.Commander => new Hex(1, 0),
                ETraitType.Entrepreneur => new Hex(1, -1),
                ETraitType.Logistician => new Hex(0, -1),
                ETraitType.Entropist => new Hex(-1, 0),
                ETraitType.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }

        public int CountNeurons => Board.Positions.Count(p => p.HasData());

        #region EventHandlers

        private void OnBoardStateBroadcast(EventArgs args) {
            if (args is not GameStateEventArgs {State: GameState.BoardStateBroadcast}) {
                return;
            }
            externalEventManager.Raise(ExternalBoardEvents.OnBoardBroadCast, new OnBoardStateBroadcastEventArgs(this));
        }

        #endregion
    }
}