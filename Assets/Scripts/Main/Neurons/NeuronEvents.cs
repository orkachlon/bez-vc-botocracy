using System;
using ExternBoardSystem.BoardSystem.Board;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;

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

    public class BoardNeuronEventArgs : EventArgs {
        public readonly BoardNeuron Neuron;

        public BoardNeuronEventArgs(BoardNeuron neuron) {
            Neuron = neuron;
        }
        
        public BoardNeuronEventArgs() {
            Neuron = null;
        }
    }

    public class UINeuronEventArgs : BoardNeuronEventArgs {
        public readonly Neuron UINeuron;

        public UINeuronEventArgs(Neuron uiNeuron) : base(new BoardNeuron(uiNeuron.DataProvider)){
            UINeuron = uiNeuron;
        }

        public UINeuronEventArgs() {
            UINeuron = null;
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

    public class RewardTileArgs : EventArgs {
        public Hex RewardHex;

        public RewardTileArgs(Hex rewardHex) {
            RewardHex = rewardHex;
        }
    }
}