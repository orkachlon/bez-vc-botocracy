using System;
using Main.StoryPoints.Interfaces;

namespace Main.StoryPoints {
    public static class StoryEvents {
        public const string OnOutcomesEnter = "Story_OnOutcomesEnter";
        public const string OnOutcomesExit = "Story_OnOutcomesExit";

        // manager events
        public const string OnStoryTurn = "Story_OnStoryTurn";
        public const string OnNoMoreStoryPoints = "Story_OnNoMoreEvent";
        
        // instance events
        public const string OnInitStory = "Story_OnInitStory";
        public const string OnDecrement = "Story_OnDecrement";
        public const string OnEvaluate = "Story_OnEvaluate";
    }

    public class StoryEventArgs : EventArgs {
        public IStoryPoint Story;
        
        public StoryEventArgs(MStoryPoint story) {
            Story = story;
        }
    }
}