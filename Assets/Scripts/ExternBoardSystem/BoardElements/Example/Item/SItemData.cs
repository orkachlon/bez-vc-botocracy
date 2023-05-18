using UnityEngine;

namespace ExternBoardSystem.BoardElements.Example.Item {
    [CreateAssetMenu]
    public class SItemData : ScriptableObject, IElementDataProvider<BoardElement> {
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardElement GetElement() {
            return new BoardItem(this);
        }

        public Sprite GetArtwork() {
            return artwork;
        }

        public GameObject GetModel() {
            return model;
        }
    }
}