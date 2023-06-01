using System;
using System.Collections.Generic;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public interface ITraitAccessor {

        public ETraitType HexToTrait(Hex hex);
        public ETraitType WorldPosToTrait(Vector3 worldPosition);



        #region Tiles

        public Hex[] GetTraitHexes(ETraitType trait);
        
        
        public Color GetColor(ETraitType trait, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetColor(ETraitType trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        public void SetTiles(ETraitType trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        #endregion

        #region Neurons

        IEnumerable<ETraitType> GetMaxNeuronsTrait(IEnumerable<ETraitType> fromTraits = null);

        #endregion

        #region StaticFunctions

        static Hex TraitToDirection(ETraitType trait) {
            var direction = trait switch {
                ETraitType.Entropist => new Hex(0, 1),
                ETraitType.Commander => new Hex(1, 0),
                ETraitType.Entrepreneur => new Hex(1, -1),
                ETraitType.Logistician => new Hex(0, -1),
                ETraitType.Defender => new Hex(-1, 0),
                ETraitType.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }
        
        static ETraitType DirectionToTrait(Hex hex) {
            if (hex == new Hex(0, 1)) {
                return ETraitType.Entropist;
            }
            if (hex == new Hex(1, 0)) {
                return ETraitType.Commander;
            }
            if (hex == new Hex(1, -1)) {
                return ETraitType.Entrepreneur;
            }
            if (hex == new Hex(0, -1)) {
                return ETraitType.Logistician;
            }
            if (hex == new Hex(-1, 0)) {
                return ETraitType.Defender;
            }
            if (hex == new Hex(-1, 1)) {
                return ETraitType.Mediator;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }

        #endregion
    }
}