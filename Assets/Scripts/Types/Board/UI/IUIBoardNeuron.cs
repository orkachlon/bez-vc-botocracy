
using System.Threading.Tasks;
using Types.Audio;

namespace Types.Board.UI {
    public interface IUIBoardNeuron : IUIBoardElement, IAudioSource {
        void ToHoverLayer();
        void ToBoardLayer();

        Task PlayRemoveAnimation();
        Task PlayAddAnimation();
        Task PlayTurnAnimation();
        Task PlayHoverAnimation();
        void StopHoverAnimation();
        Task PlayMoveAnimation();

        void PlayAddSound();
        void PlayRemoveSound();
    }
}