using Types.Board;
using Types.Board.UI;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Types.Neuron.Data {
    public interface INeuronDataBase: IElementDataProvider<IBoardNeuron, IUIBoardNeuron> {

        ENeuronType Type { get; }
        IBoardNeuron RuntimeElement { get; }
        public Color ConnectionColor { get; }

        void SetData(INeuronDataBase other);
        IUIQueueNeuron GetUIModel();
        Sprite GetQueueStackArtwork();
        Sprite GetQueueTopArtwork();
        Sprite GetFaceSprite();
        TileBase GetEffectTile();
    }
}