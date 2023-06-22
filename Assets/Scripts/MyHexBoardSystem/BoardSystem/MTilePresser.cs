using System;
using Core.EventSystem;
using Events.Board;
using MyHexBoardSystem.BoardSystem.Interfaces;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using UnityEngine;

namespace MyHexBoardSystem.BoardSystem {
    public class MTilePresser : MonoBehaviour {
        
        [SerializeField] private TraitTiles pressedTileBases;
        [Header("Event Managers"), SerializeField] private SEventManager boardEventManager;

        private INeuronBoardController _controller;

        private void Awake() {
            _controller = GetComponent<INeuronBoardController>();
        }
        
        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnTileOccupied, OnTilePressed);
            boardEventManager.Register(ExternalBoardEvents.OnTileUnoccupied, OnTileUnpressed);
            boardEventManager.Register(ExternalBoardEvents.OnTileOccupantMoved, OnNeuronMoved);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupied, OnTilePressed);
            boardEventManager.Unregister(ExternalBoardEvents.OnTileUnoccupied, OnTileUnpressed);
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupantMoved, OnNeuronMoved);
        }


        #region EventHandlers

        private void OnTilePressed(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronData) {
                return;
            }

            PressTile(neuronData.Hex);
        }

        private void OnTileUnpressed(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronData) {
                return;
            }

            UnpressTile(neuronData.Hex);
        }

        private void OnNeuronMoved(EventArgs obj) {
            if (obj is not BoardElementMovedEventArgs<IBoardNeuron> moveArgs) {
                return;
            }
            UnpressTile(moveArgs.Hex);
            PressTile(moveArgs.To);
        }

        #endregion
        
        
        private void PressTile(Hex hex) {
            if (!_controller.Board.HasPosition(hex)) {
                return;
            }

            var dir = _controller.Manipulator.GetDirection(hex);
            if (!dir.HasValue) {
                return;
            }

            var trait = ITraitAccessor.DirectionToTrait(dir.Value);
            if (!trait.HasValue) {
                return;
            }

            _controller.SetTile(hex, pressedTileBases[trait.Value]);
        }

        private void UnpressTile(Hex hex) {
            if (!_controller.Board.HasPosition(hex)) {
                return;
            }

            var dir = _controller.Manipulator.GetDirection(hex);
            if (!dir.HasValue) {
                return;
            }

            var trait = ITraitAccessor.DirectionToTrait(dir.Value);
            if (!trait.HasValue) {
                return;
            }

            _controller.SetTile(hex, _controller.GetTraitTileBase(trait.Value));
        }
    }
}