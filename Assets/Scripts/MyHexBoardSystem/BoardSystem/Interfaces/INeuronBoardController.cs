using ExternBoardSystem.BoardSystem;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.Traits;
using Types.Board;
using Types.Hex.Coordinates;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.BoardSystem.Interfaces {
    public interface INeuronBoardController : IBoardController<IBoardNeuron> {
        int GetTraitTileCount(ETrait trait);

        Color GetColor(Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex[] hexTiles, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex hexTile, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        TileBase GetTile(Hex hex, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTile(Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTiles(Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        Hex WorldPosToHex(Vector3 position);
        Vector3 HexToWorldPos(Hex hex);

        void RemoveTile(Hex hex);
        void AddTile(Hex hex);
        TileBase GetTraitTileBase(ETrait trait);
    }
}