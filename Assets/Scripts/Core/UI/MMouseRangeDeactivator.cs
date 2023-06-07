using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI {
    public class MMouseRangeDeactivator : MonoBehaviour, IPointerEnterHandler {

        [SerializeField] private GameObject compass;

        private Image _bounds;

        private void Awake() {
            _bounds = GetComponent<Image>();
            _bounds.alphaHitTestMinimumThreshold = 0.5f;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            compass.SetActive(false);
        }
    }
}