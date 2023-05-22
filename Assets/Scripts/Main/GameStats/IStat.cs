namespace Main.GameStats {
    public interface IStat {
        
        public float Value { get; set; }

        public bool IsInBounds();
    }

    public enum EStatType {
        Economy,
        Defense,
        Health
    }
}