using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.MyHexBoardSystem.Events;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


namespace Main.MyHexBoardSystem.BoardSystem {
    
    /// <summary>
    ///     A component to be added to objects that need access to the board through the traits
    /// </summary>
    public class MTraitAccessor : MonoBehaviour, ITraitAccessor {

        [Header("Controllers"), SerializeField] private MNeuronBoardController boardController;
        [SerializeField] private MBoardNeuronsController neuronsController;

        [Header("Event Managers"), SerializeField]
        private SEventManager boardEventManager;
        
        // lazy
        private readonly Dictionary<ETrait, HashSet<Hex>> _traitHexes = new();
        private Dictionary<ETrait, HashSet<Hex>> TraitHexes {
            get
            {
                if (_traitHexes.Count > 0) {
                    return _traitHexes;
                }
                SaveTilesPerTrait();

                return _traitHexes;
            }
        }

        #region UnityMethods

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, OnAddTile);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnRemoveTile);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnAddTile, OnAddTile);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveTile, OnRemoveTile);
        }

        #endregion

        #region InterfaceMethods

        public ETrait? HexToTrait(Hex hex) {
            foreach (var trait in TraitHexes.Keys) {
                if (TraitHexes[trait].Contains(hex)) {
                    return trait;
                }
            }

            return null;
        }

        public ETrait? WorldPosToTrait(Vector3 worldPosition) {
            var hex = boardController.WorldPosToHex(worldPosition);
            return HexToTrait(hex);
        }

        public Hex[] GetTraitHexes(ETrait trait) {
            return TraitHexes[trait].ToArray();
        }

        public Color GetColor(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            if (traitHexes == null || traitHexes.Length == 0) {
                return Color.magenta;
            }
            return boardController.GetColor(traitHexes[0]);
        }

        public void SetColor(ETrait trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = GetTraitHexes(trait);
            boardController.SetColor(traitHexes, color);
        }

        public TileBase GetTile(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            return boardController.GetTraitTileBase(trait);
        }

        public void SetTiles(ETrait trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = GetTraitHexes(trait);
            boardController.SetTiles(traitHexes, tile, tilemapLayer);
        }

        public IEnumerable<ETrait> GetMaxNeuronsTrait(IEnumerable<ETrait> fromTraits = null) {
            return neuronsController.GetMaxTrait(fromTraits);
        }

        #endregion

        #region EventHandlers

        private void OnAddTile(EventArgs eventArgs) {
            if (eventArgs is not OnTileModifyEventArgs tileModifyEventArgs) {
                return;
            }

            var hex = tileModifyEventArgs.Hex;
            var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return;
            }
            TraitHexes[trait.Value].Add(hex);
        }

        private void OnRemoveTile(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs tileModifyEventArgs) {
                return;
            }

            var hex = tileModifyEventArgs.Hex;
            var trait = TraitHexes.Keys.FirstOrDefault(t => TraitHexes[t].Contains(hex));
            TraitHexes[trait].Remove(hex);
        }

        #endregion
        
        private void SaveTilesPerTrait() {
            foreach (var hex in boardController.GetHexPoints()) {
                var trait = ITraitAccessor.DirectionToTrait(BoardManipulationOddR<BoardNeuron>.GetDirectionStatic(hex));
                if (!trait.HasValue) {
                    continue;
                }
                if (!_traitHexes.ContainsKey(trait.Value)) {
                    _traitHexes[trait.Value] = new HashSet<Hex>();
                }
                _traitHexes[trait.Value].Add(hex);
            }
        }

        public Hex[] GetEmptyTiles(ETrait trait) {
            return TraitHexes[trait]
                .Where(h => neuronsController.Board.HasPosition(h) && 
                            !neuronsController.Board.GetPosition(h).HasData())
                .ToArray();
        }
    }
}