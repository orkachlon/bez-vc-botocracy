using System;
using Types.Events;
using Types.StoryPoint;

namespace Events.SP {
    public static class StoryEvents {
        public const string OnOutcomesEnter = "Story_OnOutcomesEnter";
        public const string OnOutcomesExit = "Story_OnOutcomesExit";

        
        // instance events
        public const string OnInitStory = "Story_OnInitStory";
        public const string OnDecrement = "Story_OnDecrement";
        public const string OnBeforeEvaluate = "Story_OnBeforeEvaluate";
        public const string OnEvaluate = "Story_OnEvaluate";
    }

    // manager events
    public struct OnNoMoreStoryPoints : IEvent { }
    public struct OnStoryTurn : IEvent { }

    public struct OnInitStory : IEvent {
        public IStoryPoint story;
    }


    public class StoryEventArgs : EventArgs {
        public IStoryPoint Story;
        
        public StoryEventArgs(IStoryPoint story) {
            Story = story;
        }
    }
}