using System;
using System.Collections;
using System.Collections.Generic;
using Core.EventSystem;
using Events.Board;
using Events.General;
using Events.Neuron;
using Events.SP;
using JetBrains.Annotations;
using Neurons.Runtime;
using Types.GameState;
using Types.Neuron;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.NeuronQueue {
    public class MNeuronQueue : MonoBehaviour, INeuronQueue, IEnumerable<IStackNeuron> {
        
        [Header("Event Managers"), SerializeField] protected SEventManager neuronEventManager;
        [SerializeField] protected SEventManager boardEventManager;
        [SerializeField] protected SEventManager modificationsEventManager;
        [SerializeField] protected SEventManager gmEventManager;
        [SerializeField] protected SEventManager storyEventManager;

        public int Count => IsInfinite ? int.MaxValue : Neurons.Count;
        public bool IsInfinite { get; private set; }
        public IBoardNeuron NextBoardNeuron => Count > 0 && IsProviding ? Peek().BoardNeuron : null;

        protected Queue<IStackNeuron> Neurons;
        protected bool IsProviding;
        protected bool GameEnded;

        #region UnityMethods

        protected virtual void Awake() {
            Neurons = new Queue<IStackNeuron>();
            IsProviding = true;
        }
        
        protected virtual void OnEnable() {
            storyEventManager.Register(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementTurnDone, CheckNeuronsDepleted);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnGameEnd);
        }

        protected virtual void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementTurnDone, CheckNeuronsDepleted);
            neuronEventManager.Unregister(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Unregister(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
            gmEventManager.Unregister(GameManagerEvents.OnAfterGameStateChanged, OnGameEnd);
        }

        #endregion

        public void Enqueue(IEnumerable<IStackNeuron> neurons) {
            foreach (var neuron in neurons) {
                Enqueue(neuron);
            }
        }

        public void Enqueue(IStackNeuron stackNeuron) {
            Neurons.Enqueue(stackNeuron);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronQueueEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(new StackNeuron(NeuronFactory.GetRandomPlaceableNeuron()));
            }
        }

        public virtual IStackNeuron Dequeue() {
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
            return prevNeuron;
        }

        public IStackNeuron Peek() {
            return Count == 0 ? null : Neurons.Peek();
        }

        public IStackNeuron PeekLast() {
            return Neurons.ToArray()[Neurons.Count - 1];
        }

        public IStackNeuron[] PeekFirst(int number) {
            if (number > Neurons.Count) {
                number = Neurons.Count;
            }

            return Neurons.ToArray()[Range.EndAt(number)];
        }

        [CanBeNull]
        public IStackNeuron Peek(int index) {
            if (0 <= index && index < Neurons.Count) {
                return Neurons.ToArray()[index];
            }

            return null;
        }

        #region EventHandlers
        
        protected virtual void OnRewardNeurons(EventArgs eventArgs) {
            if (eventArgs is NeuronRewardEventArgs reward) {
                Enqueue(reward.Amount);
            }
        }

        private void OnInfiniteNeurons(EventArgs eventArgs) {
            if (eventArgs is not IsInfiniteNeuronsEventArgs infiniteNeurons) {
                return;
            }

            IsInfinite = infiniteNeurons.IsInfinite;
        }

        private void OnGameEnd(EventArgs obj) {
            if (obj is not GameStateEventArgs { State: EGameState.Win } && obj is not GameStateEventArgs {State: EGameState.Lose}) {
                return;
            }

            GameEnded = true;
            StopProvidingNeurons();
        }

        protected void BeforeElementActivation(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron>) {
                return;
            }
            StopProvidingNeurons();
            Dequeue();
        }

        protected void OnSPDecrement(EventArgs args) {
            if (args is not StoryEventArgs spArgs) {
                return;
            }

            if (spArgs.Story.TurnsToEvaluation > 0) {
                StartProvidingNeurons();
            }
        }

        private void CheckNeuronsDepleted(EventArgs args) {
            if (Neurons.Count > 0) {
                return;
            }
            StopProvidingNeurons();
            neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronQueueEventArgs(this));
        }

        #endregion

        protected void StopProvidingNeurons(EventArgs args = null) {
            IsProviding = false;
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        protected void StartProvidingNeurons(EventArgs args = null) {
            if (GameEnded) {
                return;
            }
            IsProviding = true;
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        public IEnumerator<IStackNeuron> GetEnumerator() => Neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}