using System;

namespace Events.General {
    public static class GameModificationEvents {
        public const string OnInfiniteSP = "Modifications_OnInfiniteSP";
        public const string OnInfiniteNeurons = "Modifications_OnInfiniteNeurons";
    }

    public class IsInfiniteStoryPointsEventArgs : EventArgs {
        public bool IsInfinite;

        public IsInfiniteStoryPointsEventArgs(bool isInfinite) {
            IsInfinite = isInfinite;
        }
    }

    public class IsInfiniteNeuronsEventArgs : EventArgs {
        public bool IsInfinite;
        public IsInfiniteNeuronsEventArgs(bool isInfinite) {
            IsInfinite = isInfinite;
        }
    }
}