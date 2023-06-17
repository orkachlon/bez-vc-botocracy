using Types.Pooling;
using UnityEngine;

namespace Types.Board.UI {
    public interface IUIBoardElement : IPoolable {
        void SetRuntimeElementData(IBoardElement data);
        void SetWorldPosition(Vector3 position);
    }
}