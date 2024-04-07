
using System.Collections.Generic;
using Types.Trait;
using UnityEngine;

namespace Types.StoryPoint {
    public interface IStoryPointData {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }
        public int TurnsToEvaluation { get; set; }
        public IDictionary<ETrait, ITraitDecisionEffects> DecidingTraits { get; set; }
        public string Prerequisites { get; set; }
        public Sprite Image { get; set; }
    }
}
