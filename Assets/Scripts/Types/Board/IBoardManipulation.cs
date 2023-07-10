using UnityEngine;

namespace Types.Board {
    /// <summary>
    ///     Interface with useful hex board algorithms.
    /// </summary>
    public interface IBoardManipulation {
        bool Contains(Vector3Int cell);
        Hex.Coordinates.Hex[] GetNeighbours(Vector3Int cell);
        Hex.Coordinates.Hex[] GetNeighbours(Hex.Coordinates.Hex hex, bool includeEmpty = false);
        Hex.Coordinates.Hex[] GetVertical(Vector3Int cell, int length);
        Hex.Coordinates.Hex[] GetHorizontal(Vector3Int cell, int length);
        Hex.Coordinates.Hex[] GetDiagonalAscendant(Vector3Int cell, int length);
        Hex.Coordinates.Hex[] GetDiagonalDescendant(Vector3Int cell, int length);
        Hex.Coordinates.Hex[] GetPathBreadthSearch(Vector3Int begin, Vector3Int end);
        Hex.Coordinates.Hex[] GetTriangle(Hex.Coordinates.Hex direction);
        Hex.Coordinates.Hex[] GetEdge(Hex.Coordinates.Hex direction);
        Hex.Coordinates.Hex[] GetEdge();
        Hex.Coordinates.Hex[] GetSurroundingHexes(Hex.Coordinates.Hex[] hexes, bool includeEmpty = false);
        Hex.Coordinates.Hex? GetDirection(Hex.Coordinates.Hex hex);
        Hex.Coordinates.Hex GetFarthestHex();


        //TODO:
        //1. Range
        //2. Path finding
        //3. More useful methods ...
    }
}