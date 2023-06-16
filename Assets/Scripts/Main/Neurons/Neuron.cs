using System.Threading.Tasks;
using Core.Utils;
using ExternBoardSystem.Tools;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Data;
using Main.Neurons.Runtime;

namespace Main.Neurons {
    public class Neuron : BoardNeuron {
        public Neuron(SNeuronDataBase dataProvider) : base(dataProvider) { }
        
        public Neuron(BoardNeuron boardNeuron) : base(boardNeuron.DataProvider) { }

        public ENeuronUIState UIState { get; set; }
        
        
        public override void Activate() {
            MLogger.LogEditor("Tried to activate UI neuron!!!");
        }

        public override MUIBoardNeuron Pool() {
            return MObjectPooler.Instance.Get(DataProvider.GetModel());
        }

        public override Task AwaitNeuronRemoval() {
            return Task.Delay(0);
        }
    }
    
    public enum ENeuronUIState {
        First = 0,
        Second = 1,
        Third = 2,
        Stack = 3
    }
}