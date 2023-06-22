using System.Collections.Generic;
using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {

        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }


        public ExpandNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding);
            Connector = NeuronFactory.GetConnector();
        }

        public override async Task Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            var spawnTasks = new List<Task>();
            for (var i = 0; i < neighbours.Length; i++) {
                var neighbour = neighbours[i];
                if (!Controller.Board.HasPosition(neighbour) || Controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                spawnTasks.Add(SpawnNeighbour(neighbour, i * 50));
            }

            await Task.WhenAll(spawnTasks);
            ReportTurnDone();
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        private async Task SpawnNeighbour(Hex neighbour, int delay = 0) {
            await Task.Delay(delay);
            var newElement = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
            await Controller.AddElement(newElement, neighbour);
        }
    }
}