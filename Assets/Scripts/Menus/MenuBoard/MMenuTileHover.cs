using Events.Board;
using Menus.Buttons;
using MyHexBoardSystem.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types.Board;
using Types.Hex.Coordinates;
using UnityEngine;

namespace Menus.MenuBoard {
    public class MMenuTileHover : MTileHover {
        [SerializeField] private List<MTileButton> buttons;

        protected override void UpdatePosition(Vector2 mouseScreen) {
            var mouseWorld = Cam.ScreenToWorldPoint(mouseScreen);
            var mouseHex = boardController.WorldPosToHex(mouseWorld);
            if (mouseHex == CurrentTile || Highlighted.Contains(mouseHex) || buttons.Where(b => b.IsActive).Any(b => b.IsPressed)) {
                return;
            }
            Hide();
            Show(mouseHex);
        }

        protected override void Show(Hex hex) {
            if (!boardController.Board.HasPosition(hex) || buttons.All(b => !b.IsActive)) {
                return;
            }

            bool canBePlaced;
            if (buttons.Any(b => b.Contains(hex))) {
                canBePlaced = true;
                ShowEffect(hex);
            } else {
                canBePlaced = false;
                boardController.SetTile(hex, cannotBePlacedTileBase, BoardConstants.MouseHoverTileLayer);
                Highlighted.Add(hex);
            }
            CurrentTile = hex;
            boardEventManager.Raise(ExternalBoardEvents.OnTileHover, new TileHoverArgs(hex, canBePlaced));
        }

        protected override void ShowEffect(Hex hex) {
            var button = buttons.First(b => b.Contains(hex));
            foreach (var hexTile in button.GetTiles()) {
                boardController.SetTile(hexTile, canBePlacedTileBase, BoardConstants.MouseHoverTileLayer);
                Highlighted.Add(hexTile);
            }
        }
    }
}