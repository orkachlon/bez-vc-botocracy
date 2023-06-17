using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.Tools.Extensions;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Board {
    /// <summary>
    ///     The way to manipulate a board in the Odd-Row layout.
    ///     TODO: Open for many memory/cache optimizations and algorithms improvements.
    /// </summary>
    public class BoardManipulationOddR<T> : IBoardManipulation where T : IBoardElement {
        public static readonly Hex[] NeighboursDirections = {
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
            
            // querying direction.X directly because Sign(0) returns 1
            if (direction.q == 0) { // s outer loop
                GetQ0Triangle(rSign, sSign, hexes);
            } else if (direction.r == 0) { // q outer loop
                GetR0Triangle(qSign, sSign, hexes);
            }
            else { // r outer loop
                GetS0Triangle(qSign, rSign, hexes);
            }
            return hexes.ToArray();
        }

        private void GetS0Triangle(int qSign, int rSign, ICollection<Hex> hexes) {
            var sign2 = rSign;
            var i2 = sign2;
            var lastI = sign2 * MaxCoord(p => sign2 * p.Point.r);
            while (lastI.HasValue && Mathf.Abs(i2) <= Mathf.Abs(lastI.Value)) {
                var i1 = qSign;
                while (Mathf.Abs(i1) <= Mathf.Abs(i2)) {
                    var hexToAdd = new Hex(i1, i2);
                    if (Contains(GetCellCoordinate(hexToAdd))) {
                        hexes.Add(hexToAdd);
                    }

                    i1 += qSign;
                }

                i2 += sign2;
            }
        }

        private void GetR0Triangle(int qSign, int sSign, ICollection<Hex> hexes) {
            var sign1 = qSign;
            var i1 = sign1;
            var lastI = sign1 * MaxCoord(p => sign1 * p.Point.q);
            while (lastI.HasValue && Mathf.Abs(i1) <= Mathf.Abs(lastI.Value)) {
                var i2 = sSign;
                while (Mathf.Abs(i2) <= Mathf.Abs(i1)) {
                    var hexToAdd = new Hex(i1, -i1 - i2);
                    if (Contains(GetCellCoordinate(hexToAdd))) {
                        hexes.Add(hexToAdd);
                    }

                    i2 += sSign;
                }

                i1 += sign1;
            }
        }

        private void GetQ0Triangle(int rSign, int sSign, ICollection<Hex> hexes) {
            var sign2 = sSign;
            var i2 = sign2;
            var lastI = sign2 * MaxCoord(p => sign2 * p.Point.s);
            while (lastI.HasValue && Mathf.Abs(i2) <= Mathf.Abs(lastI.Value)) {
                var i1 = rSign;
                while (Mathf.Abs(i1) <= Mathf.Abs(i2)) {
                    var hexToAdd = new Hex(-i1 - i2, i1);
                    if (Contains(GetCellCoordinate(hexToAdd))) {
                        hexes.Add(hexToAdd);
                    }

                    i1 += rSign;
                }

                i2 += sign2;
            }
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

        private int? MaxCoord(Func<IPosition<T>,int> selector) {
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
        
        public static Hex[] GetNeighboursStatic(Hex hex) {
            var neighbours = new Hex[] { };
            return NeighboursDirections
                .Select(direction => Hex.Add(hex, direction))
                .Aggregate(neighbours, (current, neighbour) => current.Append(new[] {neighbour}));
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