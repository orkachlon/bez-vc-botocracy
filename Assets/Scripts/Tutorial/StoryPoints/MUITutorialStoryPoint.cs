using StoryPoints.UI;
using Types.StoryPoint;
using UnityEngine;

namespace Tutorial.StoryPoints {
    public class MUITutorialStoryPoint : MUIStoryPoint, IUIStoryPoint {

        public void Hide() {
            backGround.rectTransform.anchoredPosition = new Vector2(-backGround.rectTransform.sizeDelta.x, 50);
        }
    }
}