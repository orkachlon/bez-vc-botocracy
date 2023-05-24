namespace Main.Traits {
    public interface ITrait {

        public string GetName();
        public int Sum();
    }
    
    public enum ETraitType {
        Defender,
        Commander,
        Entrepreneur,
        Logistician,
        Entropist,
        Mediator
    }
}