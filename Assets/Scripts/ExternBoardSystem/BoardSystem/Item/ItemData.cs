using ExternBoardSystem.BoardSystem.Position;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem.Item
{
    [CreateAssetMenu]
    public class ItemData : ScriptableObject, IElementDataProvider
    {
        [SerializeField] private Sprite artwork;
        [SerializeField] private GameObject model;

        public BoardElement GetElement()
        {
            return new BoardItem(this);
        }

        public Sprite GetArtwork()
        {
            return artwork;
        }

        public GameObject GetModel()
        {
            return model;
        }
    }
}