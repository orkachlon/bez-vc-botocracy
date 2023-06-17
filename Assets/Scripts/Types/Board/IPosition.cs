namespace Types.Board {
    public interface IPosition<T> 
        where T : IBoardElement {
        T Data { get; }
        Hex.Coordinates.Hex Point { get; }
        void AddData(T baseData);
        void RemoveData();
        bool HasData();
    }
}