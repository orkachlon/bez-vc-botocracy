using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;

namespace ExternBoardSystem.BoardElements {
    public interface IBoardElementsController<in TElement> where TElement : BoardElement {
        public IBoardManipulation Manipulator { get; }
        public IBoard Board { get; }

        void AddElement(TElement element, Hex hex);
        void RemoveElement(Hex hex);
    }
}