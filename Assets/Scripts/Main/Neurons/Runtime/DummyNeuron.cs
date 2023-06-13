using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {
        public DummyNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy)) { }

        public override void Activate() { }
    }
}