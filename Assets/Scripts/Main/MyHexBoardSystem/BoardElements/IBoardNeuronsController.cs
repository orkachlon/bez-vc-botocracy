using System.Collections.Generic;
using System.Threading.Tasks;
using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.Neurons.Runtime;
using Main.Traits;

namespace Main.MyHexBoardSystem.BoardElements {
    public interface IBoardNeuronsController : IBoardElementsController<BoardNeuron> {
        #region Properties

        int CountNeurons { get; }

        #endregion

        #region Methods

        int GetTraitCount(ETrait trait);
        IEnumerable<ETrait> GetMaxTrait(IEnumerable<ETrait> fromTraits = null);

        bool AddNeuron(BoardNeuron neuron, Hex hex, bool activate = true);

        Task RemoveNeuron(Hex hex);

        #endregion
    }
}