namespace Types.Board {
    public interface IPosition<T> 
        where T : IBoardElement {
        T Data { get; }
        bool IsEnabled { get; set; }
        Hex.Coordinates.Hex Point { get; }
        void AddData(T baseData);
        void RemoveData();
        bool HasData();
    }
}