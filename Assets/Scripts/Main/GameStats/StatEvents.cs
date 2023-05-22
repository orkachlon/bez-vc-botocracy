using System;

namespace Main.GameStats {
    public static class StatEvents {
        public const string OnStatTurn = "StatsOnStatTurn";
        public const string OnStatOutOfBounds = "StatOnStatOutOfBounds";
        public const string OnStatValueChanged = "StatsOnStatValueChanged";
        public const string OnContributeToStat = "StatsOnContributeToStat";
        
        // utility
        public const string OnPrintStats = "StatsOnPrintStats";
    }

    public class StatEventArgs : EventArgs {
        public MStat Stat;

        public StatEventArgs(MStat stat) {
            Stat = stat;
        }
    }

    public class StatContributeEventArgs : EventArgs {
        public EStatType StatType;
        public float Amount;

        public StatContributeEventArgs(EStatType statType, float amount = 0) {
            StatType = statType;
            Amount = amount;
        }
    }
}