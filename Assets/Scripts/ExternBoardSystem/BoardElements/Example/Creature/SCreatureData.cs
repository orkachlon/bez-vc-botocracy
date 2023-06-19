using ExternBoardSystem.Ui.Board;
using Types.Board;
using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Creature {
    // [CreateAssetMenu]
    public class SCreatureData : ScriptableObject, IElementDataProvider<BoardElement, MUIBoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private MUIBoardElement model;

        public BoardElement GetElement() => new BoardCreature(this);

        public Sprite GetBoardArtwork() => artwork;

        public AudioClip GetAddSound() => null;
        public AudioClip GetRemoveSound() => null;

        public MUIBoardElement GetModel() => model;
    }
}