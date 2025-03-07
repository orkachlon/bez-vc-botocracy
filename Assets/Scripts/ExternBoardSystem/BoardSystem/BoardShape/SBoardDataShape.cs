﻿using Types.Hex.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.BoardShape
{
    public abstract class SBoardDataShape : ScriptableObject
    {
        public abstract Hex[] GetHexPoints();
    }
}