using System.Collections.Generic;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Position;
using ExternBoardSystem.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Board
{
    public class UiBoard : MonoBehaviour
    {
        private readonly Dictionary<BoardElement, UiBoardElement> _registerUiElements =
            new Dictionary<BoardElement, UiBoardElement>();

        [SerializeField] private BoardController controller;
        [SerializeField] private BoardElementsController elementsController;
        [SerializeField] private TileBase test;
        [SerializeField] private Sprite tileSprite;
        private IBoard CurrentBoard { get; set; }
        private Tilemap TileMap { get; set; }

        private void OnCreateBoard(IBoard board)
        {
            CurrentBoard = board;
            CreateBoardUi();
        }

        private void Awake()
        {
            TileMap = GetComponentInChildren<Tilemap>();
            controller.OnCreateBoard += OnCreateBoard;
            elementsController.OnAddElement += OnAddElement;
            elementsController.OnRemoveElement += OnRemoveElement;
        }

        private void OnRemoveElement(BoardElement element, Vector3Int cell)
        {
            var uiElement = _registerUiElements[element];
            ObjectPooler.Instance.Release(uiElement.gameObject);
        }

        private void OnAddElement(BoardElement element, Vector3Int cell)
        {
            var data = element.DataProvider;
            var model = data.GetModel();
            var obj = ObjectPooler.Instance.Get(model);
            var uiBoardElement = obj.GetComponent<UiBoardElement>();
            var worldPosition = TileMap.CellToWorld(cell);
            uiBoardElement.SetRuntimeElementData(element);
            uiBoardElement.SetWorldPosition(worldPosition);
            _registerUiElements.Add(element, uiBoardElement);
        }

        private void CreateBoardUi()
        {
            foreach (var element in _registerUiElements.Values)
                ObjectPooler.Instance.Release(element.gameObject);

            _registerUiElements.Clear();
            TileMap.ClearAllTiles();
            foreach (var pos in CurrentBoard.Positions)
            {
                var hex = pos.Point;
                var cell = BoardManipulationOddR.GetCellCoordinate(hex);
                TileMap.SetTile(cell, test);

            }
            TileMap.layoutGrid.cellSize =
                new Vector3(tileSprite.bounds.size.x, tileSprite.bounds.size.y);
            print(tileSprite.bounds.size);
        }
    }
}