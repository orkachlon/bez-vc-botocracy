
using System.Threading.Tasks;

namespace Types.Board {
    public interface IBoardElementsController<TElement> where TElement : IBoardElement {
        public IBoardManipulation Manipulator { get; }
        public IBoard<TElement> Board { get; }

        Task AddElement(TElement element, Hex.Coordinates.Hex hex);
        Task RemoveElement(Hex.Coordinates.Hex hex);
        Task MoveElement(Hex.Coordinates.Hex from, Hex.Coordinates.Hex to);
    }
}