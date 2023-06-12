using System;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Neurons.Interfaces;

namespace Main.Neurons {
    public static class NeuronEvents {
        public const string OnNeuronPlaced = "Neurons_OnNeuronPlaced";
        public const string OnNoMoreNeurons = "Neurons_OnNoMoreNeurons";
        
        // queue events
        public const string OnEnqueueNeuron = "Neurons_OnEnqueueNeuron";
        public const string OnDequeueNeuron = "Neurons_OnDequeueNeuron";
        public const string OnRewardNeurons = "Neurons_OnRewardNeurons";
        public const string OnRewardTileReached = "Neurons_OnRewardTileReached";
        public const string OnRewardTilePicked = "Neurons_OnRewardTilePicked";
    }

    public class NeuronQueueEventArgs : EventArgs {
        public INeuronQueue NeuronQueue;

        public NeuronQueueEventArgs(INeuronQueue neuronQueue) {
            NeuronQueue = neuronQueue;
        }
    }

    public class NeuronRewardEventArgs : EventArgs {
        public int Amount;

        public NeuronRewardEventArgs(int amount) {
            Amount = amount;
        }
    }

    public class RewardTileArgs : EventArgs {
        public Hex RewardHex;

        public RewardTileArgs(Hex rewardHex) {
            RewardHex = rewardHex;
        }
    }
}