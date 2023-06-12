using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
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

        #endregion
    }
}