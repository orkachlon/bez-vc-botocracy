using ExternBoardSystem.Ui.Board;
using Types.Board;
using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Item {
    // [CreateAssetMenu]
    public class SItemData : ScriptableObject, IElementDataProvider<BoardElement, MUIBoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private MUIBoardElement model;

        public BoardElement GetNewElement() => new BoardItem(this);

        public Sprite GetBoardArtwork() => artwork;

        public AudioClip GetAddSound() => null;
        public AudioClip GetRemoveSound() => null;

        public MUIBoardElement GetModel() => model;
    }
}