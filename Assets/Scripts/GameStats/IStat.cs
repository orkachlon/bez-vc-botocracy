namespace GameStats {
    public interface IStat {
        
        public float Value { get; set; }

        public bool IsInBounds(float lo, float hi);
    }

    public enum EStatType {
        Economy,
        Defense,
        Health
    }
}