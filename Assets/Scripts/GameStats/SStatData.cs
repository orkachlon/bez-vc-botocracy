using UnityEngine;

namespace GameStats {
    [CreateAssetMenu(menuName = "Stats/Stat Data")]
    public class SStatData : ScriptableObject {
        [SerializeField] private string statName;
        [SerializeField] private float value;

        #region Properties

        public float Value {
            get => value;
            set => this.value = value;
        }
        public string Name => statName;

        #endregion
    }
}