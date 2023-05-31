using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Tooltip {
    [ExecuteInEditMode]
    public class Tooltip : MonoBehaviour {

        [Header("Content"), SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI content;

        [Header("Layout"), SerializeField] private LayoutElement layoutElement;
        [SerializeField] private int characterWrapLimit;

        private RectTransform _rectTransform;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update() {
            var position = Input.mousePosition;

            // if (!Application.isEditor) {
            //     var pivotX = position.x / Screen.width;
            //     var pivotY = position.y / Screen.height;
            //
            //     _rectTransform.pivot = new Vector2(pivotX, pivotY);
            // }
            transform.position = position;

        }

        public void SetText(string tooltipContent, string tooltipHeader = "") {
            // set content
            if (string.IsNullOrEmpty(tooltipHeader)) {
                header.gameObject.SetActive(false);
            } else {
                header.gameObject.SetActive(true);
                header.text = tooltipHeader;
            }

            content.text = tooltipContent;
            
            // set layout
            var headerLength = header.text.Length; 
            var contentLength = content.text.Length;

            layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;

        }
    }
}
