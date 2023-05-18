namespace Neurons.WithNumLimit {
    public class Neuron4 : Neuron{
        public override ENeuronType Type => ENeuronType.Four;
          
        public override int AllowedNeighbors() {
            return 4;
        }
    }
}