using System.Threading.Tasks;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class ExplodeNeuron : BoardNeuron {
        
        public override INeuronDataBase DataProvider { get; }

        private MUIExplodeNeuron UIExplodeNeuron => UINeuron as MUIExplodeNeuron;

        public ExplodeNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Exploding);
            Connectable = false;
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
                await Task.Delay(50);
                // var handle = AnimationManager.GetDefaultAnimatable();
                // AnimationManager.Register(handle, Task.Delay(50));
                // await AnimationManager.WaitForElement(handle);
            }
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UINeuron.SetRuntimeElementData(this);
            return UINeuron;
        }

        public override async Task AwaitRemoval() {
            await UINeuron.PlayRemoveAnimation();
        }

        public override void Connect() { }
    }
}