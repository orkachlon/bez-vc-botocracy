using ExternBoardSystem.Ui.Board;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardElements.Neuron {
    public class MUIBoardNeuron : MUIBoardElement {
        
        public void ToFront() {
            SpriteRenderer.sortingOrder = 1;
            transform.localScale *= 1.2f;
        }

        public void ToBack() {
            SpriteRenderer.sortingOrder = 0;
            transform.localScale = Vector3.one;
        }
    }
}