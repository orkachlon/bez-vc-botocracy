using DG.Tweening;
using Types.Menus;
using UnityEngine;

namespace Menus.MainMenu {
    public class MBackButton : MonoBehaviour, IClickableButton {
        
        [SerializeField] private RectTransform mainMenuContainer;
        [SerializeField] private RectTransform levelMenuContainer;
        [SerializeField] private RectTransform bgBoard;
        [SerializeField] private Canvas canvas;

        [SerializeField] private float animationDuration;

        public void OnButtonClick() {
            var delta = levelMenuContainer.anchoredPosition.x - mainMenuContainer.anchoredPosition.x;
            DOTween.Sequence()
                .Append(mainMenuContainer.DOAnchorPosX(0, animationDuration)
                    .OnUpdate(() => levelMenuContainer.anchoredPosition = new Vector2(mainMenuContainer.anchoredPosition.x + delta, levelMenuContainer.anchoredPosition.y)))
                .Join(bgBoard.DOAnchorPosX(Screen.width / canvas.scaleFactor - bgBoard.anchoredPosition.x, animationDuration));
        }
    }
}