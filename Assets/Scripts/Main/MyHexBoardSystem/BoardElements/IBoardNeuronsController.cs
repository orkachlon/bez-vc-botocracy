using System.Collections.Generic;
using ExternBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;

namespace Main.MyHexBoardSystem.BoardElements {
    public interface IBoardNeuronsController : IBoardElementsController<BoardNeuron> {
        #region Properties

        int CountNeurons { get; }

        #endregion

        #region Methods

        int GetTraitCount(ETraitType trait);
        IEnumerable<ETraitType> GetMaxTrait(IEnumerable<ETraitType> fromTraits = null);

        #endregion
    }
}