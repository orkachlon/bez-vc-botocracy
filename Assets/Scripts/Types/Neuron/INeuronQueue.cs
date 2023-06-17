using System.Collections.Generic;
using Types.Neuron.Runtime;

namespace Types.Neuron {
    public interface INeuronQueue {

        public int Count { get; }
        public bool IsInfinite { get; }
        public IBoardNeuron NextBoardNeuron { get; }

        void Enqueue(IEnumerable<IStackNeuron> neurons);
        void Enqueue(IStackNeuron stackNeuron);
        void Enqueue(int amount);
        IStackNeuron Dequeue();
        IStackNeuron Peek();
        IStackNeuron PeekLast();
        IStackNeuron Peek(int index);
        IStackNeuron[] PeekFirst(int number);
    }
}