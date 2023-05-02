namespace Neurons.WithNumLimit {
    public class Neuron6 : Neuron{
        public override NeuronType Type => NeuronType.Six;
           
        public override int AllowedNeighbors() {
            return 6;
        }
    }
}