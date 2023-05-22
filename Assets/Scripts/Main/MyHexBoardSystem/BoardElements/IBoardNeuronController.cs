using ExternBoardSystem.BoardElements;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;

namespace Main.MyHexBoardSystem.BoardElements {
    public interface IBoardNeuronController : IBoardElementsController<BoardNeuron> {
        int GetTraitCount(ETraitType trait);
        int GetTraitOverall(ETraitType trait);
        int CountNeurons { get; }
    }
}