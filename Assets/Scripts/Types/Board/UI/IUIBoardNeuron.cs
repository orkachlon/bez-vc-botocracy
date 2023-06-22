using System.Threading.Tasks;
using Types.Audio;
using UnityEngine;

namespace Types.Board.UI {
    public interface IUIBoardNeuron : IUIBoardElement, IAudioSource {
        void ToHoverLayer();
        void ToBoardLayer();

        Task PlayRemoveAnimation();
        Task PlayAddAnimation();
        Task PlayTurnAnimation();
        Task PlayHoverAnimation();
        void StopHoverAnimation();
        Task PlayMoveAnimation(Vector3 fromPos, Vector3 toPos);

        void PlayAddSound();
        void PlayRemoveSound();
    }
}