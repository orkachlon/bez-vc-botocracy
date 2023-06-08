using ExternBoardSystem.Ui.Board;
using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Item {
    // [CreateAssetMenu]
    public class SItemData : ScriptableObject, IElementDataProvider<BoardElement, MUIBoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private MUIBoardElement model;

        public BoardElement GetElement() {
            return new BoardItem(this);
        }

        public Sprite GetBoardArtwork() {
            return artwork;
        }

        public MUIBoardElement GetModel() {
            return model;
        }
    }
}