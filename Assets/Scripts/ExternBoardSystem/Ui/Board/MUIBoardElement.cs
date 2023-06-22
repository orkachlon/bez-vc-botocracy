using Types.Board;
using Types.Board.UI;
using UnityEngine;

namespace ExternBoardSystem.Ui.Board {
    
    /// <summary>
    ///     A board element's visual appearance. This is also used as a container for pooling board elements.
    /// </summary>
    public class MUIBoardElement : MonoBehaviour, IUIBoardElement {
        [SerializeField] protected SpriteRenderer baseSpriteRenderer;
        
        protected IBoardElement RuntimeData { get; set; }
        protected SpriteRenderer SpriteRenderer => baseSpriteRenderer;
        protected Transform Transform { get; set; }

        protected virtual void Awake() {
            Transform = transform;
        }

        public virtual void SetRuntimeElementData(IBoardElement data) {
            RuntimeData = data;
            UpdateView();
        }

        public virtual void SetWorldPosition(Vector3 position) {
            Transform.position = position;
        }

        protected virtual void UpdateView() {
            SpriteRenderer.sprite = RuntimeData.DataProvider.GetBoardArtwork();
        }

        public GameObject GO => gameObject;
        public virtual void Default() {
            RuntimeData = null;
            SpriteRenderer.sprite = null;
        }
    }
}