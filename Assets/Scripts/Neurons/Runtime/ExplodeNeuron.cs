using System.Linq;
using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Events;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {
        
        public override INeuronDataBase DataProvider { get; }

        private MUIExplodeNeuron UIExplodeNeuron => UINeuron as MUIExplodeNeuron;
        protected sealed override IBoardNeuronConnector Connector { get; set; }


        public ExplodeNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Exploding);
            Connectable = false;
            Connector = NeuronFactory.GetConnector();
        }
        
        public override async Task Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour)) {
                    continue;
                }
                var neighbourPos = Controller.Board.GetPosition(neighbour);
                if (!neighbourPos.HasData() || 
                    ENeuronType.Decaying.Equals(neighbourPos.Data.DataProvider.Type) || 
                    ENeuronType.Invulnerable.Equals(neighbourPos.Data.DataProvider.Type))
                    continue;
                // explode this neuron
                UIExplodeNeuron.PlayKillSound();
                await Controller.RemoveNeuron(neighbour);
            }
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UIExplodeNeuron.SetRuntimeElementData(this);
            return UIExplodeNeuron;
        }

        public override async Task Connect() {
            var neighbors = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => Controller.Board.GetPosition(h).HasData())
                .Select(h => Controller.Board.GetPosition(h).Data)
                .Where(n => n.DataProvider.Type is ENeuronType.Decaying or ENeuronType.Invulnerable);
            
            foreach (var other in neighbors) {
                await Connector.Connect(this, other);
            }
        }
    }
}