using ExternBoardSystem.Ui.Board;

namespace Main.MyHexBoardSystem.UI {
    public class MUIBoardNeuron : MUIBoardElement {


        public void ToFront() {
            SpriteRenderer.sortingOrder = 1;
        }

        public void ToBack() {
            SpriteRenderer.sortingOrder = 0;
        }
    }
}