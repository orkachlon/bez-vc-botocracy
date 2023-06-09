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
        public int id;
        public string description;
        public int reward;
        public int turnsToEvaluation;
        public DecidingTraits decidingTraits;
        public int[] prerequisites;
    }

    #region Wrappers

    // why are these serializable?
    [Serializable]
    public class TraitWeights : SerializableDictionary<ETrait, float> { }
    [Serializable]
    public class TraitsToOutcomes : SerializableDictionary<ETrait, string> { }

    public class DecidingTraits : Dictionary<ETrait, TraitDecisionEffects> { }

    public class TraitDecisionEffects {

        public static readonly TraitDecisionEffects NoDecision = new();

        public ETrait DecidingTrait;
        public string Outcome;
        public Dictionary<ETrait, int> BoardEffect;
    }

    #endregion
}
