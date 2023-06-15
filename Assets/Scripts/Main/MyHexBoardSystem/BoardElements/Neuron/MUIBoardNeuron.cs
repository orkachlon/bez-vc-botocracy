using System.Threading.Tasks;
using ExternBoardSystem.Ui.Board;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements.Neuron {
    public class MUIBoardNeuron : MUIBoardElement {

        [Header("Sorting orders"), SerializeField]
        protected int hoverSortingOrder;
        [SerializeField] protected int boardSortingOrder;
        
        public virtual void ToHoverLayer() {
            SpriteRenderer.sortingOrder = hoverSortingOrder;
            transform.localScale *= 1.2f;
        }

        public virtual void ToBoardLayer() {
            SpriteRenderer.sortingOrder = boardSortingOrder;
            transform.localScale = Vector3.one;
        }

        public virtual async Task PlayRemoveAnimation() {
            await Task.Delay(50);
        }

        public virtual async Task PlayAddAnimation() {
            await Task.Delay(50);
        }

        public virtual Task PlayTurnAnimation() {
            return Task.Delay(50);
        }
    }
}