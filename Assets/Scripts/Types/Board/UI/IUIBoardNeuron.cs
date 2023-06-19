
using System.Threading.Tasks;

namespace Types.Board.UI {
    public interface IUIBoardNeuron : IUIBoardElement {
        void ToHoverLayer();
        void ToBoardLayer();

        Task PlayRemoveAnimation();
        Task PlayAddAnimation();
        Task PlayTurnAnimation();
        Task PlayHoverAnimation();
        void StopHoverAnimation();
        Task PlayMoveAnimation();

        void PlayAddSound();
    }
}