using UnityEngine;

namespace Main.StoryPoints {
    [CreateAssetMenu(fileName = "NewTraitEffect", menuName = "Story Points/Trait Effect")]
    public class TraitEffect : ScriptableObject {
        [Range(-1, 1)] public int effect;
    }
}