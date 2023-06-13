using Main.Neurons.Data;

namespace Main.Neurons.Runtime {
    public class InvulnerableNeuron : BoardNeuron {
        public InvulnerableNeuron() : base(MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Invulnerable)) { }

        public override void Activate() { }
    }
}