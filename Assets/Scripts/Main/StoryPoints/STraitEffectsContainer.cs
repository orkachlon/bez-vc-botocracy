using System;
using Core.Utils.DataStructures;
using Main.Traits;
using UnityEngine;

namespace Main.StoryPoints {
    [CreateAssetMenu(fileName = "NewTraitEffectsContainer", menuName = "Story Points/Trait Effects Container")]
    public class STraitEffectsContainer : ScriptableObject {
        public int outcomeID;
        public string outcome;
        public TraitBoardEffect effectsOnBoard;
    }
    
    [Serializable]
    public class TraitBoardEffect : UDictionary<ETraitType, TraitEffect> { }
}