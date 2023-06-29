using Core.EventSystem;
using Core.Utils;
using Events.Tutorial;
using System;
using System.Collections.Generic;
using System.Linq;
using Tutorial;
using Tutorial.Board;
using Types.Hex.Coordinates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tutorial.Board {
    public class MEnabledTilesPresenter : MonoBehaviour {

        [SerializeField] private TileBase disabledTile;
        [SerializeField] private MTutorialBoardController controller;
        [SerializeField] private SEventManager tutorialEventManager;

        private readonly HashSet<Hex> _disabledTiles = new();

        private void OnEnable() {
            tutorialEventManager.Register(TutorialEvents.OnTilesEnabled, UpdateEnabledTiles);
            tutorialEventManager.Register(TutorialEvents.OnTilesDisabled, UpdateEnabledTiles);
        }

        private void OnDisable() {
            tutorialEventManager.Unregister(TutorialEvents.OnTilesEnabled, UpdateEnabledTiles);
            tutorialEventManager.Unregister(TutorialEvents.OnTilesDisabled, UpdateEnabledTiles);
        }

        private void UpdateEnabledTiles(EventArgs args) {
            if (args is not TutorialTilesEventArgs tilesEventArgs) {
                MLogger.LogEditorWarning($"Incorrect arg received in UpdateEnabledTiles");
                return;
            }
            controller.SetTiles(_disabledTiles.ToArray(), null, TutorialConstants.EnabledTilesMap);
            if (!tilesEventArgs.Enabled) {
                controller.SetTiles(tilesEventArgs.Hexes.ToArray(), disabledTile, TutorialConstants.EnabledTilesMap);
            } else if (tilesEventArgs.Enabled) {
                controller.SetTiles(tilesEventArgs.Hexes.ToArray(), null, TutorialConstants.EnabledTilesMap);
            }
        }
    }
}