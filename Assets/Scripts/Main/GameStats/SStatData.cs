using UnityEngine;

namespace Main.GameStats {
    [CreateAssetMenu(menuName = "Stats/Stat Data")]
    public class SStatData : ScriptableObject {
        [SerializeField] private EStatType statType;
        [SerializeField] private float value;

        #region Properties

        public float Value {
            get => value;
            set => this.value = value;
        }
        public EStatType Type => statType;

        #endregion
    }
}