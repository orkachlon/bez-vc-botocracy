namespace Neurons.WithNumLimit {
    public class Neuron5 : Neuron{
        public override ENeuronType Type => ENeuronType.Five;
          
        public override int AllowedNeighbors() {
            return 5;
        }
    }
}