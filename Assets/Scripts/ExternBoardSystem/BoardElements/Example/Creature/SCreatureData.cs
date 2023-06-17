using ExternBoardSystem.Ui.Board;
using Types.Board;
using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Creature {
    // [CreateAssetMenu]
    public class SCreatureData : ScriptableObject, IElementDataProvider<BoardElement, MUIBoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private MUIBoardElement model;

        public BoardElement GetElement() {
            return new BoardCreature(this);
        }

        public Sprite GetBoardArtwork() {
            return artwork;
        }

        public MUIBoardElement GetModel() {
            return model;
        }
    }
}