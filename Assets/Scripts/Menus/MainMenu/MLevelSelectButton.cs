using DG.Tweening;
using Types.Menus;
using UnityEngine;

namespace Menus.MainMenu {
    public class MLevelSelectButton : MonoBehaviour, IClickableButton {
		[SerializeField] private RectTransform mainMenuContainer;
		[SerializeField] private RectTransform levelMenuContainer;
        [SerializeField] private RectTransform bgBoard;
        [SerializeField] private Canvas canvas;

        [SerializeField] private AnimationCurve menuSlideEasing;
        [SerializeField] private float animationDuration;

        public void OnButtonClick() {
            var delta = mainMenuContainer.anchoredPosition.x - levelMenuContainer.anchoredPosition.x;
            DOTween.Sequence()
                .Append(levelMenuContainer.DOAnchorPosX(0, animationDuration)
                    .OnUpdate(() => mainMenuContainer.anchoredPosition = new Vector2(levelMenuContainer.anchoredPosition.x + delta, mainMenuContainer.anchoredPosition.y)))
                .Join(bgBoard.DOAnchorPosX(Screen.width / canvas.scaleFactor - bgBoard.anchoredPosition.x, animationDuration));
        }
    }
}
