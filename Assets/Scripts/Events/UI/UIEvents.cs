using System;

namespace Events.UI {
    public static class UIEvents {

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
}