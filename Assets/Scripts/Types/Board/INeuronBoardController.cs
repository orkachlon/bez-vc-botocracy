using System.Threading.Tasks;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Types.Board {
    public interface INeuronBoardController : IBoardController<IBoardNeuron> {
        int GetTraitTileCount(ETrait trait);

        Color GetColor(Hex.Coordinates.Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex.Coordinates.Hex[] hexTiles, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex.Coordinates.Hex hexTile, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        TileBase GetTile(Hex.Coordinates.Hex hex, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTile(Hex.Coordinates.Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTiles(Hex.Coordinates.Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        Hex.Coordinates.Hex WorldPosToHex(Vector3 position);
        Vector3 HexToWorldPos(Hex.Coordinates.Hex hex);

        Task RemoveTile(Hex.Coordinates.Hex hex);
        Task AddTile(Hex.Coordinates.Hex hex);
        void RecalculateTiles();
        TileBase GetTraitTileBase(ETrait trait);
    }
}