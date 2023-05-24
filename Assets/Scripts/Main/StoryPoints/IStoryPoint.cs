using Main.MyHexBoardSystem.BoardElements;

namespace Main.StoryPoints {
    public interface IStoryPoint {

        public void Evaluate(IBoardNeuronController controller);
    }
}