using System;
using Main.GameStats;
using Main.Traits;
using Main.Utils;

namespace Main.StoryPoints.SPProviders {
    
    public interface ISPProvider {
        int Count { get; }
        
        StoryPointData Next();
        bool IsEmpty();

        void Reset();
    }
    
    [Serializable]
    public struct StoryPointData {
        public string description;
        public StatToTraitWeights statEffects;
        public int reward;
        public int turnsToEvaluation;
        public TraitsToOutcomes outcomes;
    }

    #region Wrappers

    [Serializable]
    public class TraitWeights : SerializableDictionary<ETraitType, float> { }
    [Serializable]
    public class StatToTraitWeights : SerializableDictionary<EStatType, TraitWeights> { }
    [Serializable]
    public class TraitsToOutcomes : SerializableDictionary<ETraitType, string> { }

    #endregion
}
