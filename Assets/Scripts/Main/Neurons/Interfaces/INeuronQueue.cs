using System.Collections.Generic;

namespace Main.Neurons.Interfaces {
    public interface INeuronQueue {

        public int Count { get; }

        void Enqueue(IEnumerable<Neuron> neurons);
        void Enqueue(Neuron neuron);
        void Enqueue(int amount);
        Neuron Dequeue();
        Neuron Peek();
        Neuron PeekLast();
        Neuron Peek(int index);
        Neuron[] PeekFirst(int number);
    }
}