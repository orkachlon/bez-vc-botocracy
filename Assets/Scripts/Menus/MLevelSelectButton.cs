using DG.Tweening;
using System;
using Types.Menus;
using UnityEngine;

namespace Menus {
    public class MLevelSelectButton : MMainMenuBaseButton, IMenuButton {
		[SerializeField] private RectTransform mainMenuContainer;
		[SerializeField] private RectTransform levelMenuContainer;
        [SerializeField] private RectTransform anchor;

        public void OnButtonClick() {
            var delta = mainMenuContainer.anchoredPosition.x - levelMenuContainer.anchoredPosition.x;
            levelMenuContainer.DOAnchorPosX(0, 1.5f).OnUpdate(() => mainMenuContainer.anchoredPosition = new Vector2(levelMenuContainer.anchoredPosition.x + delta, mainMenuContainer.anchoredPosition.y));
        }
    }
}
