using System.Threading.Tasks;
using Animation;
using Core.Tools.Pooling;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using MyHexBoardSystem.BoardElements.Neuron.UI;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Data;

namespace Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {

        public override INeuronDataBase DataProvider { get; }

        public ExpandNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding);
        }


        public override async Task Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour) || Controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                var newElement = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy);
                await Controller.AddElement(newElement, neighbour);
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
    }
}