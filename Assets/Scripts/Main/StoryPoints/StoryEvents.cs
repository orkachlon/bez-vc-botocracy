using System;

namespace Main.StoryPoints {
    public static class StoryEvents {
        // manager events
        public const string OnStoryTurn = "StoryOnStoryTurn";
        public const string OnNoMoreStoryPoints = "StoryOnNoMoreEvent";
        
        // instance events
        public const string OnInitStory = "StoryOnInitStory";
        public const string OnDecrement = "StoryOnDecrement";
    }

    public class StoryEventArgs : EventArgs {
        public StoryPoint Story;
        
        public StoryEventArgs(StoryPoint story) {
            Story = story;
        }
    }
}