using DG.Tweening;
using Types.Menus;
using UnityEngine;

namespace Menus.MainMenu {
    public class MBackButton : MonoBehaviour, IClickableButton {
        
        [SerializeField] private RectTransform mainMenuContainer;
        [SerializeField] private RectTransform levelMenuContainer;

        public void OnButtonClick() {
            var delta = levelMenuContainer.anchoredPosition.x - mainMenuContainer.anchoredPosition.x;
            mainMenuContainer.DOAnchorPosX(0, 1.5f).OnUpdate(() => levelMenuContainer.anchoredPosition = new Vector2(mainMenuContainer.anchoredPosition.x + delta, levelMenuContainer.anchoredPosition.y));
        }
    }
}