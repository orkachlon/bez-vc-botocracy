using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.General;
using Events.Neuron;
using Events.SP;
using JetBrains.Annotations;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Runtime;
using Types.GameState;
using Types.Neuron;
using Types.Neuron.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neurons.NeuronQueue {
    public class MNeuronQueue : MonoBehaviour, INeuronQueue, IEnumerable<IStackNeuron> {

        [SerializeField] private int memorySize;
        
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
        private int _memorySize;
        private Queue<ENeuronType> _memory;

        #region UnityMethods

        protected virtual void Awake() {
            Neurons = new Queue<IStackNeuron>();
            IsProviding = true;
            _memory = new Queue<ENeuronType>(memorySize);
        }
        
        protected virtual void OnEnable() {
            storyEventManager.Register(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Register(ExternalBoardEvents.OnAllNeuronsDone, CheckNeuronsDepleted);
            neuronEventManager.Register(NeuronEvents.OnRewardNeurons, OnRewardNeurons);
            modificationsEventManager.Register(GameModificationEvents.OnInfiniteNeurons, OnInfiniteNeurons);
            gmEventManager.Register(GameManagerEvents.OnAfterGameStateChanged, OnGameEnd);
        }

        protected virtual void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnDecrement, OnSPDecrement);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, StartProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementPreActivation, BeforeElementActivation);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementFailed, StartProvidingNeurons);
            boardEventManager.Unregister(ExternalBoardEvents.OnAllNeuronsDone, CheckNeuronsDepleted);
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
            RegisterToMemory(stackNeuron.DataProvider.Type);
            Neurons.Enqueue(stackNeuron);
            neuronEventManager.Raise(NeuronEvents.OnEnqueueNeuron, new NeuronQueueEventArgs(this));
            neuronEventManager.Raise(NeuronEvents.OnQueueStateChanged, new NeuronQueueEventArgs(this));
        }

        public void Enqueue(int amount) {
            for (var i = 0; i < amount; i++) {
                var nextNeuron = GetNextNeuronRandomly();
                Enqueue(new StackNeuron(nextNeuron));
            }
        }
        
        // Returns a random neuron sampled from a distribution favoring neurons that showed up less.
        private BoardNeuron GetNextNeuronRandomly() {
            // map neuron types to their weights: maxValue - currentValue
            // this way smaller values will get a higher probability
            var nTypeToProb = new SortedDictionary<ENeuronType, float>();
            if (_memory.Count > 0) {
                GetNeuronWeightsBasedOnMemory(nTypeToProb);
            }
            else {
                GetFlatNeuronWeights(nTypeToProb);
            }

            var overallSum = nTypeToProb.Values.Sum();
            foreach (var nType in nTypeToProb.Keys.ToArray()) {
                nTypeToProb[nType] = nTypeToProb[nType] / overallSum;
            }

            var rndNeuronType = GetCDF(nTypeToProb).Invoke(Random.value);
            var nextNeuron = NeuronFactory.GetBoardNeuron(rndNeuronType);
            return nextNeuron;
        }

        private void GetNeuronWeightsBasedOnMemory(IDictionary<ENeuronType, float> nTypeToProb) {
            var memoryAsArray = _memory.ToArray();
            foreach (var neuronType in NeuronFactory.PlaceableNeurons) {
                nTypeToProb[neuronType] = 0;
                for (var i = 0; i < _memory.Count; i++) {
                    nTypeToProb[neuronType] += memoryAsArray[i] == neuronType ? i + 1 : 0;
                }
            }
            var max = nTypeToProb.Values.Max();
            foreach (var neuronType in NeuronFactory.PlaceableNeurons) {
                nTypeToProb[neuronType] = max - nTypeToProb[neuronType] + 1;
            }
            
            // this version was only based on the amount neurons appeared in memory,
            // without taking when they appeared into account.
            // var max = _memory.Max(n => _memory.Count(other => other == n));
            // foreach (var neuronType in NeuronFactory.PlaceableNeurons) {
            //     nTypeToProb[neuronType] = max - _memory.Count(other => other == neuronType) + 1f;
            // }
        }

        private static void GetFlatNeuronWeights(IDictionary<ENeuronType, float> nTypeToProb) {
            foreach (var neuronType in NeuronFactory.PlaceableNeurons) {
                nTypeToProb[neuronType] = 1f;
            }
        }

        private void RegisterToMemory(ENeuronType nType) {
            if (_memory.Count >= memorySize) {
                _memory.TryDequeue(out _);
            }
            _memory.Enqueue(nType);
        }

        private Func<float, ENeuronType> GetCDF(SortedDictionary<ENeuronType, float> nTypeToAmount) {
            return rndSample => {
                var amountsCumulative = new float[nTypeToAmount.Count + 1];
                var typesAsArray = nTypeToAmount.Keys.ToArray();
                
                amountsCumulative[0] = 0;
                for (var i = 1; i < typesAsArray.Length; i++) {
                    amountsCumulative[i] = amountsCumulative[i - 1] + nTypeToAmount[typesAsArray[i - 1]];
                }
                amountsCumulative[^1] = 1;

                var nt = typesAsArray[0];
                for (var i = 0; i < amountsCumulative.Length - 1; i++) {
                    if (amountsCumulative[i] <= rndSample && rndSample < amountsCumulative[i + 1]) {
                        nt = typesAsArray[i];
                        break;
                    }
                }

                return nt;
            };
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