using System;
using Types.Trait;

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
    }

    public class TutorialStageEventArgs : EventArgs {
        public int Stage;

        public TutorialStageEventArgs(int stage) { Stage = stage; }
    }

    public class TutorialTraitHoverEventArgs : EventArgs {
        public ETrait Trait;

        public TutorialTraitHoverEventArgs(ETrait trait) {
            Trait = trait;
        }
    }
}