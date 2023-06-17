using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.Ui.Particles;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board
{
    public class MUIBoardHightlight<T> : MonoBehaviour where T : BoardElement
    {
        private readonly Dictionary<Hex, MUIHoverParticleSystem> _highlights =
            new Dictionary<Hex, MUIHoverParticleSystem>();

        [SerializeField] private MBoardController<T> controller;
        [SerializeField] private GameObject highlightTiles;
        private Tilemap TileMap { get; set; }

        private void OnCreateBoard(IBoard<T> board)
        {
            Hide();
            _highlights.Clear();
            foreach (var p in board.Positions)
            {
                var hex = p.Point;
                var cell = BoardManipulationOddR<T>.GetCellCoordinate(hex);
                var worldPosition = TileMap.CellToWorld(cell);
                var highlight = Instantiate(highlightTiles, worldPosition, Quaternion.identity, transform)
                    .GetComponent<MUIHoverParticleSystem>();
                highlight.name = hex.ToString();
                if (!_highlights.ContainsKey(hex))
                    _highlights.Add(hex, highlight);
            }
        }

        private void Awake()
        {
            TileMap = GetComponentInChildren<Tilemap>();
            controller.OnCreateBoard += OnCreateBoard;
        }

        private void Hide()
        {
            foreach (var i in _highlights.Values)
                i.Hide();
        }

        public void Show(Hex[] positions)
        {
            Hide();
            foreach (var i in positions)
                if (_highlights.ContainsKey(i))
                    _highlights[i].Show();
        }
    }
}