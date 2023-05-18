using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using TMPro;
using UnityEngine;

namespace ExternBoardSystem.Ui.Util {
    [RequireComponent(typeof(TMP_Text))]
    public class MUIOrientationText<T> : MUITmpText where T : BoardElement{
        private const string Vertical = "Vertical";
        private const string Horizontal = "Horizontal";

        [SerializeField] private MBoardController<T> controller;

        private void OnEnable() {
            CheckOrientation();
        }

        private void CheckOrientation() {
            var board = controller.Board;
            if (board == null)
                return;
            var txt = board.Orientation == EOrientation.FlatTop ? Vertical : Horizontal;
            SetText(txt);
        }
    }
}