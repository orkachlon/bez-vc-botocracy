using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events.Board;
using Events.Neuron;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;

namespace Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {
        
        public override INeuronDataBase DataProvider { get; }

        private MUIExplodeNeuron UIExplodeNeuron => UINeuron as MUIExplodeNeuron;
        protected sealed override IBoardNeuronConnector Connector { get; set; }


        public ExplodeNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Exploding);
            // Connectable = false;
            Connector = NeuronFactory.GetConnector();
        }
        
        public override async Task Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            var killTasks = new List<Task>();
            var i = 0;
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
                killTasks.Add(KillNeighbor(neighbour, i * 50));
                i++;
            }

            await Task.WhenAll(killTasks);
            ReportTurnDone();
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UIExplodeNeuron.SetRuntimeElementData(this);
            return UIExplodeNeuron;
        }
        
        public override async Task AwaitAddition() {
            await Task.Yield();
            UIExplodeNeuron.PlayAddSound();
            UIExplodeNeuron.PlayAddAnimation();
            await Connect();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
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
        
        public override Hex[] GetAffectedTiles(Hex hex, INeuronBoardController controller = null) {
            if (controller != null) {
                return controller.Manipulator.GetNeighbours(hex)
                    .Where(n => controller.Board.GetPosition(n).HasData() && 
                                controller.Board.GetPosition(n).Data.DataProvider.Type is not ENeuronType.Invulnerable and not ENeuronType.Decaying)
                    .ToArray();
            }

            return Controller != null ? 
                Controller.Manipulator.GetNeighbours(hex)
                    .Where(n => Controller.Board.GetPosition(n).HasData() && 
                                Controller.Board.GetPosition(n).Data.DataProvider.Type is not ENeuronType.Invulnerable and not ENeuronType.Decaying)
                    .ToArray() : 
                BoardManipulationOddR<IBoardNeuron>.GetNeighboursStatic(hex);
        }

private async Task KillNeighbor(Hex neighbour, int delay = 0) {
            await Task.Delay(delay);
            UIExplodeNeuron.PlayKillSound();
            await Controller.RemoveNeuron(neighbour);
        }
    }
}