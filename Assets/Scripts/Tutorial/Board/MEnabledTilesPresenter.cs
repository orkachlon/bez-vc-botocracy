using Core.EventSystem;
using Core.Utils;
using Events.Tutorial;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

        private readonly HashSet<Tween> _tweens = new ();

        private void OnEnable() {
            tutorialEventManager.Register(TutorialEvents.OnTilesEnabled, UpdateEnabledTiles);
            tutorialEventManager.Register(TutorialEvents.OnTilesDisabled, UpdateEnabledTiles);
        }

        private void OnDisable() {
            tutorialEventManager.Unregister(TutorialEvents.OnTilesEnabled, UpdateEnabledTiles);
            tutorialEventManager.Unregister(TutorialEvents.OnTilesDisabled, UpdateEnabledTiles);
        }

        private void OnDestroy() {
            foreach (var tween in _tweens) {
                tween?.Kill();
            }
        }

        private void UpdateEnabledTiles(EventArgs args) {
            if (args is not TutorialTilesEventArgs tilesEventArgs) {
                MLogger.LogEditorWarning($"Incorrect arg received in UpdateEnabledTiles");
                return;
            }

            if (tilesEventArgs.Enabled) {
                if (tilesEventArgs.Immediate) {
                    controller.SetTiles(tilesEventArgs.Hexes.ToArray(), null, TutorialConstants.EnabledTilesMap);
                    return;
                }
                var tilesToAnimate = tilesEventArgs.Hexes
                    .Where(h => controller.GetTile(h, TutorialConstants.EnabledTilesMap) != null).ToArray();
                PlayTilesAnimation(tilesToAnimate, new Color(0, 0, 0, 0.5f), Color.clear)
                    .OnComplete(() =>
                        controller.SetTiles(tilesEventArgs.Hexes.ToArray(), null,
                            TutorialConstants.EnabledTilesMap));
            }
            else {
                controller.SetTiles(tilesEventArgs.Hexes.ToArray(), disabledTile, TutorialConstants.EnabledTilesMap);
                if (tilesEventArgs.Immediate) {
                    return;
                }

                var tilesToAnimate = tilesEventArgs.Hexes
                    .Where(h => controller.GetTile(h, TutorialConstants.EnabledTilesMap) == null).ToArray();
                PlayTilesAnimation(tilesToAnimate, Color.clear, new Color(0, 0, 0, 0.5f));
            }
        }

        private Tween PlayTilesAnimation(Hex[] hexes, Color from, Color to) {
            foreach (var hex in hexes) {
                controller.SetColor(hex, from, TutorialConstants.EnabledTilesMap);
            }

            var tween = DOVirtual.Color(from, to, 0.3f,
                c => controller.SetColor(hexes, c, TutorialConstants.EnabledTilesMap));
            tween.OnComplete(() => _tweens.Remove(tween));
            _tweens.Add(tween);
            return tween;
        }
    }
}