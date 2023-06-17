using System;
using Core.Utils.DataStructures;
using Types.Trait;
using UnityEngine;

namespace Main.StoryPoints.SOVersion {
    [CreateAssetMenu(fileName = "NewStoryPoint", menuName = "Story Points/Story Point")]
    public class SStoryPointData : ScriptableObject {
        public string description;
        public TraitsToEffects decidingTraits;
    }

    [Serializable]
    public class TraitsToEffects : UDictionary<ETrait, STraitEffectsContainer> { }
}