using System.Linq;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.Tools.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board {
    
    /// <summary>
    ///     A component to show each tile's coordinates.
    /// </summary>
    public class MUIBoardDebug<T> : MonoBehaviour where T : BoardElement {
        private GameObject[] _positions;
        private GameObject[] _triangle;
        [SerializeField] private MBoardController<T> controller;
        [SerializeField] private GameObject textPosition;
        [SerializeField] private Tilemap tileMap;
        [SerializeField] private uint direction;
        private IBoard<T> CurrentBoard { get; set; }

        protected void Awake() {
            controller.OnCreateBoard += OnCreateBoard;
        }

        [Button]
        protected void DrawPositions() {
            const string uiPosition = "UiPosition_";
            var identity = Quaternion.identity;
            ClearPositions();
            ClearDirection();
            _positions = new GameObject[CurrentBoard.Positions.Count];
            for (var i = 0; i < CurrentBoard.Positions.Count; i++) {
                var hex = CurrentBoard.Positions[i].Point;
                var cell = BoardManipulationOddR<T>.GetCellCoordinate(hex);
                var worldPosition = tileMap.CellToWorld(cell);
                var gameObj = Instantiate(textPosition, worldPosition, identity, transform);
                _positions[i] = gameObj;
                var tmpText = gameObj.GetComponent<TMP_Text>();
                var sPosition = $"q:{hex.q}\nr:{hex.r}\ns:{hex.s}";
//                var sPosition = $"x:{cell.x}\ny:{cell.y}";
                tmpText.text = sPosition;
                tmpText.name = uiPosition + sPosition;
            }
        }

        [Button]
        protected void ClearPositions() {
            if (_positions == null)
                return;

            foreach (var i in _positions)
                Destroy(i);
        }

        [Button]
        protected void ShowDirection() {
            ClearPositions();
            ClearDirection();
            _triangle = new GameObject[] { };
            var dir = direction % 6;
            controller.Manipulator
                .GetTriangle(DirectionToHex(dir))
                .Where(h => CurrentBoard.HasPosition(h))
                .Select(BoardManipulationOddR<T>.GetCellCoordinate)
                .Select(c => tileMap.CellToWorld(c)).ToList().ForEach(v => {
                    var p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    p.transform.position = v;
                    p.transform.localScale = 0.5f * Vector3.one;
                    _triangle = _triangle.Append(p).ToArray();
                });
        }

        [Button]
        protected void ClearDirection() {
            if (_triangle == null)
                return;

            foreach (var i in _triangle)
                Destroy(i);
        }

        private void OnDrawGizmos() {
            if (CurrentBoard == null || _positions == null)
                return;

            foreach (var hex in controller.GetHexPoints()) {
                var cell = BoardManipulationOddR<T>.GetCellCoordinate(hex);
                var worldPosition = tileMap.CellToWorld(cell);
                Gizmos.DrawWireSphere(worldPosition, 0.5f);
            }
        }

        private void OnCreateBoard(IBoard<T> board) {
            CurrentBoard = board;
        }

        private Hex DirectionToHex(uint dir) {
            return dir switch {
                0 => new Hex(1, 0),
                1 => new Hex(1, -1),
                2 => new Hex(0, -1),
                3 => new Hex(-1, 0),
                4 => new Hex(-1, 1),
                5 => new Hex(0, 1),
                _ => new Hex()
            };
        }
    }
}