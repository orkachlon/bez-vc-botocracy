using DG.Tweening;
using System.Linq;
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
            // set bg width with respect to message content, up to a maximum width.
            if (message.Contains("\n")) {
                var lines = message.Split("\n");
                var longestLine = lines.Max(l => l.Length);
                layoutElement.enabled = longestLine > characterWrapLimit;
            } else {
                layoutElement.enabled = message.Length > characterWrapLimit;
            }
        }

        public async Task AwaitShowAnimation() {
            bg.rectTransform.anchoredPosition = new (-bg.rectTransform.sizeDelta.x, -250);
            await bg.rectTransform.DOAnchorPosX(100, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }

        public async Task AwaitHideAnimation() {
            await bg.rectTransform.DOAnchorPosX(-bg.rectTransform.sizeDelta.x, 0.5f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        }
    }
}