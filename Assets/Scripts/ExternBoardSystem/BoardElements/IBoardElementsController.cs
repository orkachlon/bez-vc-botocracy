using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardElements {
    public interface IBoardElementsController<TElement> where TElement : BoardElement {
        public IBoardManipulation Manipulator { get; }
        public IBoard<TElement> Board { get; }

        void AddStartingElement(TElement element, Hex hex);
        void AddElement(TElement element, Hex hex);
        void RemoveElement(Hex hex);
    }
}