﻿using System.Collections.Generic;
using ExternBoardSystem.BoardSystem.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.BoardShape
{
    [CreateAssetMenu(menuName = "BoardShape/TriangleBoardData", fileName = "TriangleBoardData")]
    public class STriangleBoardDataShape : SBoardDataShape
    {
        private readonly List<Hex> _points = new List<Hex>();
        [Range(1, 10)] public int size;

        public override Hex[] GetHexPoints()
        {
            _points.Clear();

            var halfSize = size / 2;
            for (var x = -halfSize; x <= size; x++)
            for (var y = -halfSize; y <= halfSize - x; y++)
                _points.Add(new Hex(x, y));
            return _points.ToArray();
        }
    }
}