using Types.Board;
using Types.Board.UI;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;

namespace Types.Neuron.Data {
    public interface INeuronDataBase: IElementDataProvider<IBoardNeuron, IUIBoardNeuron> {

        ENeuronType Type { get; }
        IBoardNeuron RuntimeElement { get; }
        public Color ConnectionColor { get; }

        void SetData(INeuronDataBase other);
        IUINeuron GetUIModel();
        Sprite GetUIArtwork(ENeuronUIState uiState);
        Sprite GetFaceSprite();
    }
}