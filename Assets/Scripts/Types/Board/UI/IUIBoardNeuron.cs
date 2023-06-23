using System.Threading.Tasks;
using UnityEngine;

namespace Types.Board.UI {
    public interface IUIBoardNeuron : IUIBoardElement {
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