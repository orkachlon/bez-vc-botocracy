#if UNITY_EDITOR
using Core.EventSystem;
using MyHexBoardSystem.BoardSystem;
using Events.Board;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using Types.Trait;
using Core.Utils;
using System.Linq;

namespace Assets.Scripts.Tests {
    public class MHexCacheTester : MonoBehaviour {

        [SerializeField] private MNeuronBoardController controller;
        [SerializeField] private SEventManager boardEventManager;

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, TestHexes);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, TestHexes);
        }

        private void TestHexes(EventArgs args) {

            int fromController = EnumUtil.GetValues<ETrait>().Sum(t => controller.GetTraitTileCount(t)) + 1; // zero
            int fromBoard = controller.Board.Positions.Count;
            if (fromController != fromBoard) {
                MLogger.LogEditorError($"Error: hex counts aren't equal: controller - {fromController},\tboard  - {fromBoard}");
            }
        }
    }
}
#endif