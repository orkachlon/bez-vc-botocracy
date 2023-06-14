using System;

namespace Main.Utils {
    public static class UIEvents {

        // outcomes
        public const string OnOutcomesPanelExpanded = "UI_OnOutcomesPanelExpanded";
        public const string OnOutcomeAdded = "UI_OnOutcomeAdded";
        public const string OnOutcomeExpanded = "UI_OnOutcomeExpanded";
    }

    public class UIExpandEventArgs : EventArgs {
        public bool Expanded;

        public UIExpandEventArgs(bool expanded) {
            Expanded = expanded;
        }
    }
}