using System;
using System.Collections.Generic;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public interface ITraitAccessor {

        public ETrait? HexToTrait(Hex hex);
        public ETrait? WorldPosToTrait(Vector3 worldPosition);



        #region Tiles

        public Hex[] GetTraitHexes(ETrait trait);

        public Color GetColor(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetColor(ETrait trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetTiles(ETrait trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        #endregion

        #region Neurons

        IEnumerable<ETrait> GetMaxNeuronsTrait(IEnumerable<ETrait> fromTraits = null);

        #endregion

        #region StaticFunctions

        static Hex TraitToDirection(ETrait trait) {
            var direction = trait switch {
                ETrait.Entropist => new Hex(0, 1),
                ETrait.Commander => new Hex(1, 0),
                ETrait.Entrepreneur => new Hex(1, -1),
                ETrait.Logistician => new Hex(0, -1),
                ETrait.Defender => new Hex(-1, 0),
                ETrait.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }
        
        static ETrait DirectionToTrait(Hex hex) {
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
                return ETrait.Defender;
            }
            if (hex == new Hex(-1, 1)) {
                return ETrait.Mediator;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }

        #endregion
    }
}