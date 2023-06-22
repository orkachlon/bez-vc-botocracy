using System.Threading.Tasks;
using Types.Animation;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Neuron.Data;
using UnityEngine;

namespace Types.Neuron.Runtime {
    public interface IBoardNeuron : IBoardElement, IAnimatable {
        
        public new INeuronDataBase DataProvider { get; }
        public Types.Hex.Coordinates.Hex Position { get; }
        bool Connectable { get; }
        bool TurnDone { get; }

        void BindToNeurons(IEventManager neuronEventManager);
        void UnbindFromNeurons();

        void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller,
            Types.Hex.Coordinates.Hex position);

        void UnbindFromBoard();
        
        Task Activate();
        IUIBoardNeuron Pool();
        void Release();
        
        Task AwaitAddition();
        Task AwaitRemoval();
        Task AwaitMove(Vector3 fromPos, Vector3 toPos);

        Task Connect();
        Task Disconnect();
    }
}