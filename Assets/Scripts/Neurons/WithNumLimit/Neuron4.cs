namespace Neurons.WithNumLimit {
    public class Neuron4 : Neuron{
        public override NeuronType Type => NeuronType.Four;
          
        public override int AllowedNeighbors() {
            return 4;
        }
    }
}