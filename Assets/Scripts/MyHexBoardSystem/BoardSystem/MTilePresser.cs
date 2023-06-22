using System;
using System.Collections.Generic;
using Core.EventSystem;
using Events.Board;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

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
            boardEventManager.Register(ExternalBoardEvents.OnBoardBroadCast, UpdateTiles);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupied, OnTilePressed);
            boardEventManager.Unregister(ExternalBoardEvents.OnTileUnoccupied, OnTileUnpressed);
            boardEventManager.Unregister(ExternalBoardEvents.OnTileOccupantMoved, OnNeuronMoved);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardBroadCast, UpdateTiles);
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

        private void UpdateTiles(EventArgs obj) {
            if (obj is not BoardStateEventArgs boardArgs) {
                return;
            }

            foreach (var position in boardArgs.ElementsController.Board.Positions) {
                if (position.HasData()) {
                    PressTile(position.Point);
                }
            }
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