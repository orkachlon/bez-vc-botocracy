using Assets.Scripts.Tutorial.Neurons;
using Core.EventSystem;
using Events.Board;
using Events.Neuron;
using Events.Tutorial;
using Neurons.NeuronQueue;
using Neurons.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Types.Neuron;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Tutorial.Neurons {
    public class MTutorialNeuronQueue : MNeuronQueue {

        [SerializeField] private MUITutorialNeuronQueue uiQueue;

        [SerializeField] private SEventManager tutorialEventManager;

        public HashSet<ENeuronType> neuronPool = new();

        protected override void Awake() {
            base.Awake();
            uiQueue = GetComponent<MUITutorialNeuronQueue>();
        }

        protected override void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, StartProvidingNeurons);
        }

        protected override void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, StartProvidingNeurons);
        }

        public void EnqueueFromPool(int amount = 1) {
            for (int _ = 0; _ < amount; _++) {
                Enqueue(new StackNeuron(NeuronFactory.GetRandomNeuron(neuronPool)));
            }
        }

        public override IStackNeuron Dequeue() {
            // we dequeue the neuron that was placed just now.
            // the next neuron is the one after the dequeued one
            Neurons.TryDequeue(out var prevNeuron);
            if (prevNeuron == null) {
                StopProvidingNeurons(); // shouldn't happen, just to be sure
                return null;
            }
            // never run out of neurons but keep visibility and functionality the same
            if (IsInfinite) {
                Enqueue(1);
            }

            neuronEventManager.Raise(NeuronEvents.OnDequeueNeuron, new NeuronQueueEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));

            if (Neurons.Count > 0) {
                return prevNeuron;
            }
            StopProvidingNeurons();
            tutorialEventManager.Raise(TutorialEvents.OnQueueDepleted, new NeuronQueueEventArgs(this));
            return null;
        }

        public void StopNeurons() {
            StopProvidingNeurons();
        }

        public void StartNeurons() {
            StartProvidingNeurons();
        }

        public async Task Hide(bool immediate) {
            await uiQueue.Hide(immediate);
        }

        public async Task Show(bool immediate) {
            await uiQueue.Show(immediate);
        }

        public async Task Clear() {
            while (!IsInfinite && Count > 0) {
                Dequeue();
                await Task.Delay(500);
            }
        }

        protected override void OnRewardNeurons(EventArgs eventArgs) {
            if (eventArgs is NeuronRewardEventArgs reward) {
                EnqueueFromPool(reward.Amount);
            }
        }
    }
}