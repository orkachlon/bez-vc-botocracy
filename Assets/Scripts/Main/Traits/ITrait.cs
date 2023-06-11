﻿namespace Main.Traits {
    public interface ITrait {

        public string GetName();
        public int Sum();
    }
    
    public enum ETrait {
        Protector,
        Commander,
        Entrepreneur,
        Logistician,
        Entropist,
        Mediator
    }
}