﻿using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Board;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExternBoardSystem.Ui.Util {
    public class MUICamera<T> : MonoBehaviour where T : BoardElement
    {
        private Transform _myTransform;
        [SerializeField] private MBoardController<T> controller;
        [SerializeField] private Tilemap tileMap;
        private Camera MainCamera { get; set; }

        private void Awake()
        {
            MainCamera = Camera.main;
            controller.OnCreateBoard += OnCreateBoard;
            _myTransform = transform;
        }

        private void OnCreateBoard(IBoard<T> board)
        {
            var maxPosY = float.MinValue;
            var maxPosX = float.MinValue;
            var minPosY = float.MaxValue;
            var minPosX = float.MaxValue;

            foreach (var pos in board.Positions)
            {
                var hex = pos.Point;
                var cell = BoardManipulationOddR<T>.GetCellCoordinate(hex);
                var worldCellPos = tileMap.CellToWorld(cell);

                if (worldCellPos.x > maxPosX)
                    maxPosX = worldCellPos.x;
                if (worldCellPos.y > maxPosY)
                    maxPosY = worldCellPos.y;

                if (worldCellPos.x < minPosX)
                    minPosX = worldCellPos.x;
                if (worldCellPos.y < minPosY)
                    minPosY = worldCellPos.y;
            }

            Centralize(minPosX, minPosY, maxPosX, maxPosY);
        }

        private void Centralize(float minPosX, float minPosY, float maxPosX, float maxPosY)
        {
            var mediumX = (minPosX + maxPosX) / 2;
            var mediumY = (minPosY + maxPosY) / 2;
            _myTransform.position = new Vector3(mediumX, mediumY, _myTransform.position.z);
        }
    }
}