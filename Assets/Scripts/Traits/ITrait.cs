namespace Traits {
    public interface ITrait {

        public string GetName();
        public int Sum();
    }
    
    public enum ETraitType {
        Empathy,
        Righteousness,
        Charisma,
        Perception,
        Optimism,
        Intelligence
    }
}