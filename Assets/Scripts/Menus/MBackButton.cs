using DG.Tweening;
using Types.Menus;
using UnityEngine;

namespace Menus {
    public class MBackButton : MonoBehaviour, IMenuButton {
        
        [SerializeField] private RectTransform mainMenuContainer;
        [SerializeField] private RectTransform levelMenuContainer;

        public void OnButtonClick() {
            var delta = levelMenuContainer.anchoredPosition.x - mainMenuContainer.anchoredPosition.x;
            mainMenuContainer.DOAnchorPosX(0, 1.5f).OnUpdate(() => levelMenuContainer.anchoredPosition = new Vector2(mainMenuContainer.anchoredPosition.x + delta, levelMenuContainer.anchoredPosition.y));
        }
    }
}