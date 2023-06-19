using System.Threading.Tasks;
using Types.Animation;
using Types.Board;
using Types.Board.UI;
using Types.Events;
using Types.Neuron.Data;

namespace Types.Neuron.Runtime {
    public interface IBoardNeuron : IBoardElement, IAnimatable {
        
        public new INeuronDataBase DataProvider { get; }
        public Types.Hex.Coordinates.Hex Position { get; }
        bool Connectable { get; }

        void BindToBoard(IEventManager boardEventManager, IBoardNeuronsController controller,
            Types.Hex.Coordinates.Hex position);

        void BindToNeuronManager(IEventManager neuronEventManager);
        
        Task Activate();
        IUIBoardNeuron Pool();
        void Release();
        Task AwaitRemoval();
    }
}