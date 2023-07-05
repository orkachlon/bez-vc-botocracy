using System;

namespace Events.UI {
    public static class UIEvents {

        // pause
        public const string OnGamePaused = "UI_OnGamePaused";
        public const string OnGameUnpaused = "UI_OnGameUnpaused";

        // outcomes
        public const string OnOutcomesPanelExpanded = "UI_OnOutcomesPanelExpanded";
        public const string OnOutcomeAdded = "UI_OnOutcomeAdded";
        public const string OnOutcomeExpanded = "UI_OnOutcomeExpanded";
        
        // tooltip
        public const string OnTooltipShow = "UI_OnTooltipShow";
        public const string OnTooltipHide = "UI_OnTooltipHide";
    }

    public class UIExpandEventArgs : EventArgs {
        public bool Expanded;

        public UIExpandEventArgs(bool expanded) {
            Expanded = expanded;
        }
    }

    public class PauseArgs : EventArgs {
        public bool IsPaused;

        public PauseArgs(bool isPaused) {
            IsPaused = isPaused;
        }
    }
}