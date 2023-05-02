namespace Neurons.WithNumLimit {
    public class Neuron5 : Neuron{
        public override NeuronType Type => NeuronType.Five;
          
        public override int AllowedNeighbors() {
            return 5;
        }
    }
}