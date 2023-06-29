using System;
using System.Collections.Generic;
using Types.Hex.Coordinates;
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

        #region Traits

        ETrait? DirectionToTrait(Hex.Coordinates.Hex hex);
        Hex.Coordinates.Hex TraitToDirection(ETrait trait);
        Vector3 TraitToVectorDirection(ETrait trait);

        #endregion
    }
}