
namespace Types.Board {
    public interface IBoardElementsController<TElement> where TElement : IBoardElement {
        public IBoardManipulation Manipulator { get; }
        public IBoard<TElement> Board { get; }

        bool AddElement(TElement element, Hex.Coordinates.Hex hex);
        void RemoveElement(Hex.Coordinates.Hex hex);
        void MoveElement(Hex.Coordinates.Hex from, Hex.Coordinates.Hex to);
    }
}