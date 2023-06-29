using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Events.Board;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.BoardSystem {
    
    /// <summary>
    ///     A component to be added to objects that need access to the board through the traits
    /// </summary>
    public class MTraitAccessor : MonoBehaviour, ITraitAccessor {

        [Header("Controllers"), SerializeField] protected MNeuronBoardController boardController;
        [SerializeField] protected MBoardNeuronsController neuronsController;

        [Header("Event Managers"), SerializeField]
        protected SEventManager boardEventManager;
        
        // lazy
        private readonly ConcurrentDictionary<ETrait, HashSet<Hex>> _traitHexes = new();
        protected IDictionary<ETrait, HashSet<Hex>> TraitHexes => _traitHexes;


        #region UnityMethods

        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, SaveTilesPerTrait);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, CheckForFullBoard);
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, OnAddTile);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, OnRemoveTile);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, SaveTilesPerTrait);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, CheckForFullBoard);
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
            return TraitHexes.ContainsKey(trait) ? TraitHexes[trait].ToArray() : null;
        }

        public Color GetColor(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(TraitToDirection(trait));
            if (traitHexes == null || traitHexes.Length == 0) {
                return Color.magenta;
            }
            return boardController.GetColor(traitHexes[0]);
        }

        public void SetTraitColor(ETrait trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = GetTraitHexes(trait);
            if (traitHexes == null) {
                return;
            }
            boardController.SetColor(traitHexes, color);
        }

        public TileBase GetTraitTile(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            return boardController.GetTraitTileBase(trait);
        }

        public void SetTraitTiles(ETrait trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = GetTraitHexes(trait);
            if (traitHexes == null) {
                return;
            }
            boardController.SetTiles(traitHexes, tile, tilemapLayer);
        }

        public Hex[] GetTraitEdgeHexes(ETrait trait) {
            return boardController.Manipulator.GetEdge(TraitToDirection(trait));
        }

        public IEnumerable<ETrait> GetMaxNeuronsTrait(IEnumerable<ETrait> fromTraits = null) {
            return neuronsController.GetMaxTrait(fromTraits);
        }

        public Hex[] GetTraitEmptyHexes(ETrait trait, IEnumerable<Hex> fromHexes = null) {
            fromHexes ??= TraitHexes[trait];
            return fromHexes
                .Where(h => neuronsController.Board.HasPosition(h) && 
                            !neuronsController.Board.GetPosition(h).HasData())
                .ToArray();
        }

        #endregion

        #region EventHandlers

        protected virtual void CheckForFullBoard(EventArgs obj) {
            if (TraitHexes.Keys
                .All(t => TraitHexes[t]
                    .All(h => neuronsController.Board.HasPosition(h) && 
                              neuronsController.Board.GetPosition(h).HasData()))) {
                boardEventManager.Raise(ExternalBoardEvents.OnBoardFull, EventArgs.Empty);
            }
        }

        protected virtual void OnAddTile(EventArgs eventArgs) {
            if (eventArgs is not OnTileModifyEventArgs tileModifyEventArgs) {
                return;
            }

            var hex = tileModifyEventArgs.Hex;
            var trait = DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(hex));
            if (!trait.HasValue) {
                return;
            }
            if (!TraitHexes.ContainsKey(trait.Value)) {
                TraitHexes[trait.Value] = new HashSet<Hex> { hex };
            } else {
                TraitHexes[trait.Value].Add(hex); 
            }
        }

        protected virtual void OnRemoveTile(EventArgs obj) {
            if (obj is not OnTileModifyEventArgs tileModifyEventArgs) {
                return;
            }

            var hex = tileModifyEventArgs.Hex;
            var trait = TraitHexes.Keys.FirstOrDefault(t => TraitHexes[t].Contains(hex));
            TraitHexes[trait].Remove(hex);
        }

        public virtual ETrait? DirectionToTrait(Hex hex) {
            if (hex == new Hex(0, 1)) {
                return ETrait.Entropist;
            }
            if (hex == new Hex(1, 0)) {
                return ETrait.Commander;
            }
            if (hex == new Hex(1, -1)) {
                return ETrait.Entrepreneur;
            }
            if (hex == new Hex(0, -1)) {
                return ETrait.Logistician;
            }
            if (hex == new Hex(-1, 0)) {
                return ETrait.Protector;
            }
            if (hex == new Hex(-1, 1)) {
                return ETrait.Mediator;
            }
            if (hex == Hex.zero) {
                return null;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }

        public virtual Hex TraitToDirection(ETrait trait) {
            var direction = trait switch {
                ETrait.Entropist => new Hex(0, 1),
                ETrait.Commander => new Hex(1, 0),
                ETrait.Entrepreneur => new Hex(1, -1),
                ETrait.Logistician => new Hex(0, -1),
                ETrait.Protector => new Hex(-1, 0),
                ETrait.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }

        public virtual Vector3 TraitToVectorDirection(ETrait trait) {
            var direction = trait switch {
                ETrait.Commander => Quaternion.AngleAxis(30, Vector3.forward) * Vector3.up,
                ETrait.Entrepreneur => Quaternion.AngleAxis(90, Vector3.forward) * Vector3.up,
                ETrait.Logistician => Quaternion.AngleAxis(150, Vector3.forward) * Vector3.up,
                ETrait.Protector => Quaternion.AngleAxis(210, Vector3.forward) * Vector3.up,
                ETrait.Mediator => Quaternion.AngleAxis(270, Vector3.forward) * Vector3.up,
                ETrait.Entropist => Quaternion.AngleAxis(330, Vector3.forward) * Vector3.up,
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }

        #endregion

        protected virtual void SaveTilesPerTrait(EventArgs args = null) {
            foreach (var hex in boardController.GetHexPoints()) {
                var trait = DirectionToTrait(BoardManipulationOddR<IBoardNeuron>.GetDirectionStatic(hex));
                if (!trait.HasValue) {
                    continue;
                }
                if (!_traitHexes.ContainsKey(trait.Value)) {
                    _traitHexes[trait.Value] = new HashSet<Hex>();
                }
                _traitHexes[trait.Value].Add(hex);
            }
        }
    }
}