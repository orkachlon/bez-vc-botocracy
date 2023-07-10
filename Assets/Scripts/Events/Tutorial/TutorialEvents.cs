using System;
using System.Collections.Generic;
using Types.Hex.Coordinates;
using Types.Trait;
using Types.Tutorial;

namespace Events.Tutorial {
    public static class TutorialEvents {
        public const string OnBoardSetupComplete = "Tutorial_OnBoardSetupComplete";

        public const string OnBeforeStage = "Tutorial_OnBeforeStage";
        public const string OnAfterStage = "Tutorial_OnAfterStage";

        public const string OnDisableBoard = "Tutorial_OnDisableBoard";
        public const string OnEnableBoard = "Tutorial_OnEnableBoard ";

        public const string OnDisableBGInteraction = "Tutorial_OnDisableBGOutlines";
        public const string OnEnableBGInteraction = "Tutorial_OnEnableBGOutlines";

        public const string OnTraitHover = "Tutorial_OnTraitHover";

        public const string OnQueueDepleted = "Tutorial_OnQueueDepleted";
        public const string OnHideNeuronQueue = "Tutorial_OnHideNeuronQueue";
        public const string OnShowNeuronQueue = "Tutorial_OnShowNeuronQueue ";
        public const string OnHideOutcomes = "Tutorial_OnHideOutcomes";
        public const string OnShowOutcomes = "Tutorial_OnShowOutcomes";
        public const string OnTutorialSP = "Tutorial_OnTutorialSP";

        public const string OnBoardFull = "Tutorial_OnBoardFull";
        public const string OnTilesDisabled = "Tutorial_OnTilesDisabled";
        public const string OnTilesEnabled = "Tutorial_OnTilesEnabled";
    }

    public class TutorialStageEventArgs : EventArgs {
        public ETutorialStage Stage;

        public TutorialStageEventArgs(ETutorialStage stage) { Stage = stage; }
    }

    public class TutorialTraitHoverEventArgs : EventArgs {
        public ETrait Trait;

        public TutorialTraitHoverEventArgs(ETrait trait) {
            Trait = trait;
        }
    }

    public class TutorialTilesEventArgs : EventArgs {
        public Hex[] Hexes;
        public bool Enabled;
        public bool Immediate;

        public TutorialTilesEventArgs(Hex[] hexes, bool enabled, bool immediate = false) {
            Hexes = hexes;
            Enabled = enabled;
            Immediate = immediate;
        }
    }
}