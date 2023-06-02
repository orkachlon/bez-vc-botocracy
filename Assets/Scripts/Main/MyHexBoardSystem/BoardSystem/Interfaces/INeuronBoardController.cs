using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem.Interfaces {
    public interface INeuronBoardController : IBoardController<BoardNeuron> {
        int GetTraitTileCount(ETrait trait);

        Color GetColor(Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex[] hexTiles, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex hexTile, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTile(Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTiles(Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        Hex WorldPosToHex(Vector3 position);

        void RemoveTile(Hex hex);
        void AddTile(Hex hex);
    }
}