using System;
using Core.Utils.DataStructures;
using Types.Trait;
using UnityEngine;

namespace StoryPoints.SOVersion {
    [CreateAssetMenu(fileName = "NewTraitEffectsContainer", menuName = "Story Points/Trait Effects Container")]
    public class STraitEffectsContainer : ScriptableObject {
        public int outcomeID;
        public string outcome;
        public TraitBoardEffect effectsOnBoard;
    }
    
    [Serializable]
    public class TraitBoardEffect : UDictionary<ETrait, TraitEffect> { }
}