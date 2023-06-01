using UnityEngine;

namespace Main.Traits {
    public class Trait : ITrait {

        protected ETraitType TraitType;
        
        public void Init(ETraitType type) {
            TraitType = type;
        }
        
        public string GetName() {
            return TraitType.ToString();
        }

        public int Sum() {
            throw new System.NotImplementedException();
        }
    }
}