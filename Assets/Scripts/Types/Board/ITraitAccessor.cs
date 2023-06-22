using System;
using System.Collections.Generic;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Types.Board {
    public interface ITraitAccessor {

        public ETrait? HexToTrait(Hex.Coordinates.Hex hex);
        public ETrait? WorldPosToTrait(Vector3 worldPosition);



        #region Tiles

        public Hex.Coordinates.Hex[] GetTraitHexes(ETrait trait);
        public Hex.Coordinates.Hex[] GetTraitEmptyHexes(ETrait trait, IEnumerable<Hex.Coordinates.Hex> fromHexes = null);

        public Color GetColor(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetTraitColor(ETrait trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public TileBase GetTraitTile(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetTraitTiles(ETrait trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public Hex.Coordinates.Hex[] GetTraitEdgeHexes(ETrait trait);

        #endregion

        #region Neurons

        IEnumerable<ETrait> GetMaxNeuronsTrait(IEnumerable<ETrait> fromTraits = null);

        #endregion

        #region StaticFunctions

        static Hex.Coordinates.Hex TraitToDirection(ETrait trait) {
            var direction = trait switch {
                ETrait.Entropist => new Hex.Coordinates.Hex(0, 1),
                ETrait.Commander => new Hex.Coordinates.Hex(1, 0),
                ETrait.Entrepreneur => new Hex.Coordinates.Hex(1, -1),
                ETrait.Logistician => new Hex.Coordinates.Hex(0, -1),
                ETrait.Protector => new Hex.Coordinates.Hex(-1, 0),
                ETrait.Mediator => new Hex.Coordinates.Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }
        
        static ETrait? DirectionToTrait(Hex.Coordinates.Hex hex) {
            if (hex == new Hex.Coordinates.Hex(0, 1)) {
                return ETrait.Entropist;
            }
            if (hex == new Hex.Coordinates.Hex(1, 0)) {
                return ETrait.Commander;
            }
            if (hex == new Hex.Coordinates.Hex(1, -1)) {
                return ETrait.Entrepreneur;
            }
            if (hex == new Hex.Coordinates.Hex(0, -1)) {
                return ETrait.Logistician;
            }
            if (hex == new Hex.Coordinates.Hex(-1, 0)) {
                return ETrait.Protector;
            }
            if (hex == new Hex.Coordinates.Hex(-1, 1)) {
                return ETrait.Mediator;
            }
            if (hex == Hex.Coordinates.Hex.zero) {
                return null;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }

        static Vector3 TraitToVectorDirection(ETrait trait) {
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
    }
}