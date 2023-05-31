using System;
using System.Collections.Generic;
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
        public int reward;
        public int turnsToEvaluation;
        public DecidingTraits decidingTraits;
    }

    #region Wrappers

    // why are these serializable?
    [Serializable]
    public class TraitWeights : SerializableDictionary<ETraitType, float> { }
    [Serializable]
    public class TraitsToOutcomes : SerializableDictionary<ETraitType, string> { }

    public class DecidingTraits : Dictionary<ETraitType, TraitDecisionEffects> { }

    public class TraitDecisionEffects {

        public static readonly TraitDecisionEffects NoDecision = new();
        
        public string Outcome;
        public Dictionary<ETraitType, int> BoardEffect;
    }

    #endregion
}
