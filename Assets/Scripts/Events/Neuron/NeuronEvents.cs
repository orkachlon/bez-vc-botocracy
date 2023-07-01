using System;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;

namespace Events.Neuron {
    public static class NeuronEvents {
        public const string OnNeuronFinishedRemoveAnimation = "Neurons_OnNeuronFinishedRemoveAnimation";
        public const string OnNeuronFinishedAddAnimation = "Neurons_OnNeuronFinishedAddAnimation";
        public const string OnDisconnectNeurons = "Neurons_OnDisconnectNeurons";
        public const string OnConnectNeurons = "Neurons_OnConnectNeurons";
        public const string OnNoMoreNeurons = "Neurons_OnNoMoreNeurons";
        
        // queue events
        public const string OnEnqueueNeuron = "Neurons_OnEnqueueNeuron";
        public const string OnDequeueNeuron = "Neurons_OnDequeueNeuron";
        public const string OnQueueStateChanged = "Neurons_OnQueueStateChanged";
        
        // rewards
        public const string OnRewardNeurons = "Neurons_OnRewardNeurons";
        public const string OnRewardTileReached = "Neurons_OnRewardTileReached";
        public const string OnRewardTilePicked = "Neurons_OnRewardTilePicked";
        public const string OnRewardTileRemoved = "Neurons_OnRewardTileRemoved";
        
        // travel
        public const string OnTravelNeuronReady = "Neurons_OnTravelNeuronReady";
        public const string OnTravelNeuronStopped = "Neurons_OnTravelNeuronStopped";
        public const string OnTravellersReady = "Neurons_OnTravellersReady";
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
        public int Amount;

        public RewardTileArgs(Hex rewardHex, int amount) {
            RewardHex = rewardHex;
            Amount = amount;
        }
    }

    public class NeuronConnectionArgs : EventArgs {
        public IBoardNeuron Neuron1;
        public IBoardNeuron Neuron2;

        public NeuronConnectionArgs(IBoardNeuron neuron1, IBoardNeuron neuron2) {
            Neuron1 = neuron1;
            Neuron2 = neuron2;
        }
    }
}