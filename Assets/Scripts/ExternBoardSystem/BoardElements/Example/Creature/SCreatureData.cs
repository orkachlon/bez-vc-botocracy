using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Creature {
    [CreateAssetMenu]
    public class SCreatureData : ScriptableObject, IElementDataProvider<BoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardElement GetElement() {
            return new BoardCreature(this);
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public GameObject GetModel() {
            return model;
        }
    }
}