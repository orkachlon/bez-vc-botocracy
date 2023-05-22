using System;
using ExternBoardSystem.BoardSystem.Board;
using Main.MyHexBoardSystem.BoardElements.Neuron;

namespace Main.Neurons {
    public static class NeuronEvents {
        public const string OnNeuronPlaced = "NeuronsOnNeuronPlaced";
        public const string OnNoMoreNeurons = "NeuronsOnNoMoreNeurons";
        
        // queue events
        public const string OnEnqueueNeuron = "NeuronsOnEnqueueNeuron";
        public const string OnDequeueNeuron = "NeuronsOnDequeueNeuron";
        public const string OnRewardNeurons = "NeuronsOnRewardNeurons";
    }

    public class NeuronEventArgs : EventArgs {
        public readonly BoardNeuron Neuron;

        public NeuronEventArgs(BoardNeuron neuron) {
            Neuron = neuron;
        }
        
        public NeuronEventArgs() {
            Neuron = null;
        }
    }

    public class NeuronRewardEventArgs : EventArgs {
        public int Amount;

        public NeuronRewardEventArgs(int amount) {
            Amount = amount;
        }
    }

    public class NeuronTurnEventArgs : EventArgs {
        public BoardNeuron Neuron;
        public IBoardManipulation BoardManipulation;
    }
}