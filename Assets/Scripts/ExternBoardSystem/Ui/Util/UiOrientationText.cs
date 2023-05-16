﻿using ExternBoardSystem.BoardSystem;
using TMPro;
using UnityEngine;

namespace ExternBoardSystem.Ui.Util
{
    [RequireComponent(typeof(TMP_Text))]
    public class UiOrientationText : UiTmpText
    {
        private const string Vertical = "Vertical";
        private const string Horizontal = "Horizontal";

        [SerializeField] private BoardController controller;

        private void OnEnable()
        {
            CheckOrientation();
        }

        private void CheckOrientation()
        {
            var board = controller.Board;
            if (board == null)
                return;
            var txt = board.Orientation == Orientation.FlatTop ? Vertical : Horizontal;
            SetText(txt);
        }
    }
}