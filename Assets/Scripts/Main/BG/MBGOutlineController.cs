using Core.EventSystem;
using DG.Tweening;
using Events.Board;
using System;
using UnityEngine;

namespace Main.BG {
    public class MBGOutlineController : MonoBehaviour {

        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private float animationDuration = 1f;

        private Material _outlineMaterial;

        private void Awake() {
            _outlineMaterial = GetComponent<Renderer>().material;
        }

        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, ShowLines);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, ShowLines);
        }

        private void ShowLines(EventArgs args) {
            DOVirtual.Float(0, 20, animationDuration, l => _outlineMaterial.SetFloat("_Length", l))
                .OnComplete(() => _outlineMaterial.SetFloat("_Length", -1f));
        }
    }
}