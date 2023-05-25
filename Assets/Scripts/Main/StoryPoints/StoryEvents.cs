using System;

namespace Main.StoryPoints {
    public static class StoryEvents {
        // manager events
        public const string OnStoryTurn = "StoryOnStoryTurn";
        public const string OnNoMoreStoryPoints = "StoryOnNoMoreEvent";
        
        // instance events
        public const string OnInitStory = "StoryOnInitStory";
        public const string OnDecrement = "StoryOnDecrement";
        public const string OnEvaluate = "StoryOnEvaluate";
    }

    public class StoryEventArgs : EventArgs {
        public MStoryPoint Story;
        
        public StoryEventArgs(MStoryPoint story) {
            Story = story;
        }
    }
}