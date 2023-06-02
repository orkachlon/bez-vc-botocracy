using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Board
{
    /// <summary>
    ///     Interface with useful hex board algorithms.
    /// </summary>
    public interface IBoardManipulation {
        bool Contains(Vector3Int cell);
        Hex[] GetNeighbours(Vector3Int cell);
        Hex[] GetNeighbours(Hex hex, bool includeEmpty = false);
        Hex[] GetVertical(Vector3Int cell, int length);
        Hex[] GetHorizontal(Vector3Int cell, int length);
        Hex[] GetDiagonalAscendant(Vector3Int cell, int length);
        Hex[] GetDiagonalDescendant(Vector3Int cell, int length);
        Hex[] GetPathBreadthSearch(Vector3Int begin, Vector3Int end);
        Hex[] GetTriangle(Hex direction);
        Hex[] GetEdge(Hex direction);
        Hex[] GetSurroundingHexes(Hex[] hexes, bool includeEmpty = false);
        Hex GetDirection(Hex hex);

        //TODO:
        //1. Range
        //2. Path finding
        //3. More useful methods ...
    }
}