using System.Collections.Generic;
using System.Threading.Tasks;
using Types.Neuron.Runtime;
using Types.Trait;

namespace Types.Board {
    public interface IBoardNeuronsController : IBoardElementsController<IBoardNeuron> {
        #region Properties

        int CountNeurons { get; }

        #endregion

        #region Methods

        int GetTraitCount(ETrait trait);
        IEnumerable<ETrait> GetMaxTrait(IEnumerable<ETrait> fromTraits = null);

        Task<bool> AddNeuron(IBoardNeuron neuron, Hex.Coordinates.Hex hex, bool activate = true);

        Task RemoveNeuron(Hex.Coordinates.Hex hex);

        Task MoveNeuron(Hex.Coordinates.Hex from, Hex.Coordinates.Hex to, bool activate = false);

        #endregion
    }
}