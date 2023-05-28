using System;

namespace Main.GameModifications {
    public static class GameModificationEvents {
        public const string OnInfiniteSP = "ModificationsOnInfiniteSP";
    }

    public class IsInfiniteStoryPointsEventArgs : EventArgs {
        public bool IsInfinite;

        public IsInfiniteStoryPointsEventArgs(bool isInfinite) {
            IsInfinite = isInfinite;
        }
    }
}