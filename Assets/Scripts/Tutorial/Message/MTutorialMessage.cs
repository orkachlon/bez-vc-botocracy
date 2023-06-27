using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Message {
    public class MTutorialMessage : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image bg;
        [SerializeField] private int characterWrapLimit;
        
        private LayoutElement layoutElement;


        private void Awake() {
            layoutElement = GetComponentInChildren<LayoutElement>();
        }

        public void SetText(string message) {
            messageText.text = message;
            layoutElement.enabled = message.Length > characterWrapLimit;
        }

        public async Task AwaitEntranceAnimation() {
            bg.rectTransform.anchoredPosition = new Vector2(-bg.rectTransform.sizeDelta.x, -250);
            await bg.rectTransform.DOAnchorPosX(100, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }
    }
}