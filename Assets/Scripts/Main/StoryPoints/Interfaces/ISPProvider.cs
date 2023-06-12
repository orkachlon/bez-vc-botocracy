using System;
using System.Collections.Generic;
using Main.Traits;
using Main.Utils;
using UnityEngine;

namespace Main.StoryPoints.Interfaces {
    
    public interface ISPProvider {
        int Count { get; }
        
        StoryPointData? Next();
        bool IsEmpty();
        void Reset();
        void AddOutcome(int outcomeID);
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
        public string Decision;
        public string Outcome;
        public Dictionary<ETrait, int> BoardEffect;
    }

    #endregion
}
