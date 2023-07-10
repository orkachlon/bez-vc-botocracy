
using System.Threading.Tasks;

namespace Types.StoryPoint {

    public interface IUIStoryPoint {

        void InitSPUI();
        void CloseDecisionPopup();
        void UpdateTurnCounter(int turns);
        Task PlayInitAnimation();
        Task PlayDecrementAnimation();
        Task PlayEvaluateAnimation();
    }
}