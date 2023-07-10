using System;
using Events.Board;
using Events.General;
using Events.Neuron;
using Main.Managers;
using Neurons.Runtime;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;

namespace Tutorial.Managers {
    public class TutorialNeuronManager : NeuronManager {
        
        protected override void Init(EventArgs _) {
            // place the initial neuron
            var invulnerableBoardNeuron = NeuronFactory.GetBoardNeuron(ENeuronType.Invulnerable);
            var firstNeuronEventData = new BoardElementEventArgs<IBoardNeuron>(invulnerableBoardNeuron, new Hex(0, 0));
            boardEventManager.Raise(ExternalBoardEvents.OnSetFirstElement, firstNeuronEventData);
            // add some neurons to the queue
            //neuronEventManager.Raise(NeuronEvents.OnRewardNeurons, new NeuronRewardEventArgs(1));
            // start game loop
            gmEventManager.Raise(GameManagerEvents.OnGameLoopStart, EventArgs.Empty);
        }
    }
}