using System.Threading.Tasks;
using ExternBoardSystem.Ui.Board;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements.Neuron {
    public class MUIBoardNeuron : MUIBoardElement {

        [Header("Sorting orders"), SerializeField]
        private int hoverSortingOrder;
        [SerializeField] private int boardSortingOrder;
        
        public void ToHoverLayer() {
            SpriteRenderer.sortingOrder = hoverSortingOrder;
            transform.localScale *= 1.2f;
        }

        public void ToBoardLayer() {
            SpriteRenderer.sortingOrder = boardSortingOrder;
            transform.localScale = Vector3.one;
        }

        public async Task PlayRemoveAnimation() {
            await Task.Delay(1000);
        }
    }
}