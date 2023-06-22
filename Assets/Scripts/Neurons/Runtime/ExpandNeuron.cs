using System.Collections.Generic;
using System.Threading.Tasks;
using Events.Board;
using Events.Neuron;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

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
            var i = 0;
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour) || Controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                spawnTasks.Add(SpawnNeighbour(neighbour, i * 50));
                i++;
            }

            await Task.WhenAll(spawnTasks);
            ReportTurnDone();
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitAddition() {
            await Task.Yield();
            UINeuron.PlayAddSound();
            UINeuron.PlayAddAnimation();
            Connect();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        private async Task SpawnNeighbour(Hex neighbour, int delay = 0) {
            await Task.Delay(delay);
            var newElement = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
            await Controller.AddElement(newElement, neighbour);
        }
    }
}