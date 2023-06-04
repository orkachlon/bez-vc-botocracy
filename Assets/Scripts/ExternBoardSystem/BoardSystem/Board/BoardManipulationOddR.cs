using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.BoardSystem.Position;
using ExternBoardSystem.Tools.Extensions;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     The way to manipulate a board in the Odd-Row layout.
    ///     TODO: Open for many memory/cache optimizations and algorithms improvements.
    /// </summary>
    public class BoardManipulationOddR<T> : IBoardManipulation where T : BoardElement {
        private static readonly Hex[] NeighboursDirections = {
            new Hex(1, 0), new Hex(1, -1), new Hex(0, -1),
            new Hex(-1, 0), new Hex(-1, 1), new Hex(0, 1)
        };

        private readonly IBoard<T> _board;

        public BoardManipulationOddR(IBoard<T> board) {
            _board = board;
        }

        /// <summary>
        ///     If the point is present ~among the starting configuration~ returns it. Otherwise returns a empty array.
        /// </summary>
        private Hex[] GetIfExistsOrEmpty(Hex hex) {
            var cell = GetCellCoordinate(hex);
            return Contains(cell) ? new[] {GetHexCoordinate(cell)} : new Hex[] { };
        }

        #region Operations

        public Hex[] GetNeighbours(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            return GetNeighbours(hex);
        }

        public Hex[] GetNeighbours(Hex hex, bool includeEmpty = false) {
            var center = GetIfExistsOrEmpty(hex);
            if (center.Length == 0 && !includeEmpty) {
                throw new ArgumentOutOfRangeException(nameof(hex), hex, "Hex out of bounds in GetNeighbours");
            }
            center = new[] {hex};
            var neighbours = new Hex[] { };
            foreach (var direction in NeighboursDirections) {
                var neighbour = Hex.Add(center[0], direction);
                neighbours = neighbours.Append(includeEmpty ? new []{neighbour} : GetIfExistsOrEmpty(neighbour));
            }

            return neighbours;
        }

        public bool Contains(Vector3Int cell) {
            var hex = GetHexCoordinate(cell);
            return _board.HasPosition(hex);
        }

        public Hex[] GetVertical(Vector3Int cell, int length) {
            //For Odd-R the vertical is always empty.
            return new Hex[] { };
        }

        public Hex[] GetHorizontal(Vector3Int cell, int length) {
            var point = GetHexCoordinate(cell);
            var halfLength = length / 2;
            var points = GetIfExistsOrEmpty(point);
            var x = point.q;
            var y = point.r;

            for (var i = 1; i <= halfLength; i++)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x + i, y)));

            for (var i = -1; i >= -halfLength; i--)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x + i, y)));

            return points;
        }

        public Hex[] GetDiagonalAscendant(Vector3Int cell, int length) {
            var point = GetHexCoordinate(cell);
            var halfLength = length / 2;
            var points = GetIfExistsOrEmpty(point);
            var x = point.q;
            var y = point.r;

            for (var i = 1; i <= halfLength; i++)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x, y + i)));

            for (var i = -1; i >= -halfLength; i--)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x, y + i)));

            return points;
        }

        public Hex[] GetDiagonalDescendant(Vector3Int cell, int length) {
            var point = GetHexCoordinate(cell);
            var halfLength = length / 2;
            var points = GetIfExistsOrEmpty(point);
            var x = point.q;
            var y = point.r;

            for (var i = 1; i <= halfLength; i++)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x - i, y + i)));

            for (var i = -1; i >= -halfLength; i--)
                points = points.Append(GetIfExistsOrEmpty(new Hex(x - i, y + i)));

            return points;
        }

        public Hex[] GetPathBreadthSearch(Vector3Int begin, Vector3Int end) {
            var beginHex = GetHexCoordinate(begin);
            var endHex = GetHexCoordinate(end);
            var frontier = new Queue<Hex>();
            frontier.Enqueue(beginHex);
            var visited = new Dictionary<Hex, Hex>();

            
            //Creating the breadcrumbs
            
            while (frontier.Count > 0) {
                var current = frontier.Dequeue();
                if (current == endHex)
                    break;
                
                var currentCell = GetCellCoordinate(current);
                var neighbours = GetNeighbours(currentCell);
                foreach (var next in neighbours) {
                    if (!visited.ContainsKey(next)) {
                        frontier.Enqueue(next);
                        visited[next] = current;
                    }
                }
            }

            //Backtracking from the ending point
            
            
            var path = new List<Hex>();
            while(endHex != beginHex) {
                path.Add(endHex);
                endHex = visited[endHex];
            }
            
            path.Add(beginHex);
            path.Reverse();
            return path.ToArray();
        }

        public Hex[] GetTriangle(Hex direction) {
            var qSign = (int) Mathf.Sign(direction.q);
            var rSign = (int) Mathf.Sign(direction.r);
            var sSign = (int) Mathf.Sign(direction.s);
            
            var hexes = new List<Hex>();
            
            int sign1, sign2;
            if (direction.q == 0) { // s outer loop
                sign1 = rSign;
                sign2 = sSign;
                var i1 = sign1;
                var i2 = sign2;
                var lastI = sign2 * MaxCoord(p => sign2 * p.Point.s);
                while (lastI.HasValue && Mathf.Abs(i2) <= Mathf.Abs(lastI.Value)) {
                    i1 = sign1;
                    while (Mathf.Abs(i1) <= Mathf.Abs(i2)) {
                        var hexToAdd = new Hex(-i1 - i2, i1);
                        if (Contains(GetCellCoordinate(hexToAdd))) {
                            hexes.Add(hexToAdd);
                        }
                        i1 += sign1;
                    }

                    i2 += sign2;
                }
            } else if (direction.r == 0) { // q outer loop
                sign1 = qSign;
                sign2 = sSign;
                var i1 = sign1;
                var i2 = sign2;
                var lastI = sign1 * MaxCoord(p => sign1 * p.Point.q);
                while (lastI.HasValue && Mathf.Abs(i1) <= Mathf.Abs(lastI.Value)) {
                    i2 = sign2;
                    while (Mathf.Abs(i2) <= Mathf.Abs(i1)) {
                        var hexToAdd = new Hex(i1, -i1 - i2);
                        if (Contains(GetCellCoordinate(hexToAdd))) {
                            hexes.Add(hexToAdd);
                        }
                        i2 += sign2;
                    }

                    i1 += sign1;
                }
            }
            else { // r outer loop
                sign1 = qSign;
                sign2 = rSign;
                var i1 = sign1;
                var i2 = sign2;
                var lastI = sign2 * MaxCoord(p => sign2 * p.Point.r);
                while (lastI.HasValue && Mathf.Abs(i2) <= Mathf.Abs(lastI.Value)) {
                    i1 = sign1;
                    while (Mathf.Abs(i1) <= Mathf.Abs(i2)) {
                        var hexToAdd = new Hex(i1, i2);
                        if (Contains(GetCellCoordinate(hexToAdd))) {
                            hexes.Add(hexToAdd);
                        }
                        i1 += sign1;
                    }

                    i2 += sign2;
                }
            }
            return hexes.ToArray();
        }

        public Hex[] GetEdge(Hex direction) {
            var directionHexes = GetTriangle(direction);
            if (directionHexes.Length == 0) {
                return Array.Empty<Hex>();
            }
            return directionHexes
                .Where(h => GetNeighbours(h).Length != 6)
                .ToArray();
        }

        public Hex[] GetSurroundingHexes(Hex[] hexes, bool includeEmpty = false) {
            return hexes
                .SelectMany(h => GetNeighbours(h, includeEmpty))
                .Where(h => !hexes.Contains(h))
                .Distinct()
                .ToArray();
        }

        public Hex? GetDirection(Hex hex) {
            return Contains(hex) ? GetDirectionStatic(hex) : null;
        }

        public Hex GetFarthestHex() {
            var qMax = MaxCoord(p => p.Point.q);
            var rMax = MaxCoord(p => p.Point.r);
            var sMax = MaxCoord(p => p.Point.s);
            if (qMax > rMax && qMax > sMax) {
                return _board.Positions.First(p => p.Point.q == qMax).Point;
            }

            return rMax > sMax ? 
                _board.Positions.First(p => p.Point.r == rMax).Point : 
                _board.Positions.First(p => p.Point.s == sMax).Point;
        }

        #endregion

        private bool Contains(Hex hex) {
            return _board.HasPosition(hex);
        }

        private int? MaxCoord(Func<Position<T>,int> selector) {
            return _board.Positions.Select(selector).OrderByDescending(c => c).FirstOrDefault();
        }

        #region StaticFunctions

        public static Hex GetDirectionStatic(Hex hex) {
            return hex.r switch {
                //                     top-right
                > 0 when hex.q >= 0 => new Hex(0, 1),
                //                 bot-right            mid-right
                > 0 => hex.s > 0 ? new Hex(-1, 0) : new Hex(-1, 1),
                //                     bot-left
                < 0 when hex.q <= 0 => new Hex(0, -1),
                //                 top-left           mid-left
                < 0 => hex.s < 0 ? new Hex(1, 0) : new Hex(1, -1),
                //               top-left                        bot-right           center
                _ => hex.q > 0 ? new Hex(1, 0) : hex.q < 0 ? new Hex(-1, 0) : Hex.Zero
            };
        }

        /// <summary>
        ///     Unity by default makes use the R-Offset Odd to reference tiles inside a TileMap with a vector3Int cell.
        ///     The internal board manipulation works with HexCoordinates, this method converts vector3int cell to hex.
        /// </summary>
        public static Hex GetHexCoordinate(Vector3Int cell) {
            return OffsetCoordHelper.RoffsetToCube(OffsetCoord.Parity.Odd, new OffsetCoord(cell.x, cell.y));
        }

        /// <summary>
        ///     Unity by default makes use the R-Offset Odd to reference tiles inside a TileMap with a vector3Int cell.
        ///     The internal board manipulation works with HexCoordinates, this method converts hex to unity vector3int cell.
        /// </summary>
        public static Vector3Int GetCellCoordinate(Hex hex) {
            return OffsetCoordHelper.RoffsetFromCube(OffsetCoord.Parity.Odd, hex).ToVector3Int();
        }

        #endregion
    }
}