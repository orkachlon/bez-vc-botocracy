using System;
using System.Collections.Generic;
using Types.Trait;
using Types.Utils;
using UnityEngine;

namespace Types.StoryPoint {
    
    public interface ISPProvider {
        int Count { get; }
        
        StoryPointData? Next();
        void AddOutcome(int outcomeID);
        void RemoveOutcome(int outcomeID);
    }
    
    [Serializable]
    public struct StoryPointData {
        public int id;
        public string title;
        public string description;
        public int reward;
        public int turnsToEvaluation;
        public DecidingTraits decidingTraits;
        public string prerequisites;
        public Sprite image;
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
        public int OutcomeID;
        public string Decision;
        public string Outcome;
        public string OutcomeModification;
        public Dictionary<ETrait, int> BoardEffect;
    }

    #endregion
}
