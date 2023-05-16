using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.BoardShape
{
    public abstract class BoardDataShape : ScriptableObject
    {
        public abstract Hex[] GetHexPoints();
    }
}