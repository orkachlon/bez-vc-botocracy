using System;
using System.Collections.Generic;
using Core.Types;
using Types.StoryPoint;
using Types.Trait;
using UnityEngine;

namespace StoryPoints.Types {
    internal class TraitTypes {
    }

    #region Wrappers

    // why are these serializable?
    [Serializable]
    public class TraitWeights : SerializableDictionary<ETrait, float> { }
    [Serializable]
    public class TraitsToOutcomes : SerializableDictionary<ETrait, string> { }

    public class DecidingTraits : Dictionary<ETrait, ITraitDecisionEffects> { }

    public class TraitDecisionEffects : ITraitDecisionEffects {

        public static readonly ITraitDecisionEffects NoDecision = new TraitDecisionEffects();

        public ETrait decidingTrait;
        public int outcomeID;
        public string decision;
        public string outcome;
        public string outcomeModification;
        public Dictionary<ETrait, int> boardEffect;

        ETrait ITraitDecisionEffects.DecidingTrait { get => decidingTrait; set => decidingTrait = value; }
        int ITraitDecisionEffects.OutcomeID { get => outcomeID; set => outcomeID = value; }
        string ITraitDecisionEffects.Decision { get => decision; set => decision = value; }
        string ITraitDecisionEffects.Outcome { get => outcome; set => outcome = value; }
        string ITraitDecisionEffects.OutcomeModification { get => outcomeModification; set => outcomeModification = value; }
        Dictionary<ETrait, int> ITraitDecisionEffects.BoardEffect { get => boardEffect; set => boardEffect = value; }
    }

    #endregion

    [Serializable]
    public struct StoryPointData : IStoryPointData {
        public int id;
        public string title;
        public string description;
        public int reward;
        public int turnsToEvaluation;
        public IDictionary<ETrait, ITraitDecisionEffects> decidingTraits;
        public string prerequisites;
        public Sprite image;

        int IStoryPointData.Id { get => id; set => id = value; }
        string IStoryPointData.Title { get => title; set => title = value; }
        string IStoryPointData.Description { get => description; set => description = value; }
        int IStoryPointData.Reward { get => reward; set => reward = value; }
        int IStoryPointData.TurnsToEvaluation { get => turnsToEvaluation; set => turnsToEvaluation = value; }
        public IDictionary<ETrait, ITraitDecisionEffects> DecidingTraits { get => decidingTraits; set => decidingTraits = value; }
        string IStoryPointData.Prerequisites { get => prerequisites; set => prerequisites = value; }
        Sprite IStoryPointData.Image { get => image; set => image = value; }
    }
}
