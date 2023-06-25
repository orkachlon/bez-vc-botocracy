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
        
        [Header("Event Managers"), SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager modificationsEventManager;
        [SerializeField] private SEventManager gmEventManager;
        [SerializeField] private SEventManager storyEventManager;

        public int Count => IsInfinite ? int.MaxValue : _neurons.Count;
        public bool IsInfinite { get; private set; }
        public IBoardNeuron NextBoardNeuron => Count > 0 && _isProviding ? Peek().BoardNeuron : null;

        private Queue<IStackNeuron> _neurons;
        private bool _isProviding;

        #region UnityMethods

        private void Awake() {
            _neurons = new Queue<IStackNeuron>();
            _isProviding = true;
        }
        
        private void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            storyEventManager.Register(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Register(ExternalBoardEvents.OnAddElementPreActivation, StopProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElement, OnNeuronPlaced);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnGameEnd);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            storyEventManager.Unregister(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddElementPreActivation, StopProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElement, OnNeuronPlaced);
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
            _neurons.Enqueue(stackNeuron);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronQueueEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                // todo actually implement a neuron providing system
                Enqueue(new StackNeuron(NeuronFactory.GetRandomPlaceableNeuron()));
            }
        }

        public IStackNeuron Dequeue() {
            // we dequeue the neuron that was placed just now.
            // the next neuron is the one after the dequeued one
            _neurons.TryDequeue(out var prevNeuron);
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

            if (_neurons.Count > 0) {
                return prevNeuron;
            }
            StopProvidingNeurons();
            neuronEventManager.Raise(NeuronEvents.OnNoMoreNeurons, new NeuronQueueEventArgs(this));
            return null;
        }

        public IStackNeuron Peek() {
            return Count == 0 ? null : _neurons.Peek();
        }

        public IStackNeuron PeekLast() {
            return _neurons.ToArray()[_neurons.Count - 1];
        }

        public IStackNeuron[] PeekFirst(int number) {
            if (number > _neurons.Count) {
                number = _neurons.Count;
            }

            return _neurons.ToArray()[Range.EndAt(number)];
        }

        [CanBeNull]
        public IStackNeuron Peek(int index) {
            if (0 <= index && index < _neurons.Count) {
                return _neurons.ToArray()[index];
            }

            return null;
        }

        #region EventHandlers
        
        private void OnRewardNeurons(EventArgs eventArgs) {
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

            StopProvidingNeurons();
        }

        private void OnNeuronPlaced(EventArgs args) {
            if (args is not BoardElementEventArgs<IBoardNeuron>) {
                return;
            }
            Dequeue();
        }

        private void OnSPDecrement(EventArgs args) {
            if (args is not StoryEventArgs spArgs) {
                return;
            }

            if (spArgs.Story.TurnsToEvaluation > 0) {
                StartProvidingNeurons();
            }
        }

        #endregion

        private void StopProvidingNeurons(EventArgs args = null) {
            _isProviding = false;
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        private void StartProvidingNeurons(EventArgs args = null) {
            _isProviding = true;
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        public IEnumerator<IStackNeuron> GetEnumerator() => _neurons.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}