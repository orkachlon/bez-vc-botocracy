namespace Neurons.WithNumLimit {
    public class Neuron6 : Neuron{
        public override ENeuronType Type => ENeuronType.Six;
           
        public override int AllowedNeighbors() {
            return 6;
        }
    }
}