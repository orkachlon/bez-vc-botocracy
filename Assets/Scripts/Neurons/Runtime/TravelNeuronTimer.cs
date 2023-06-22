
using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.Neuron;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Runtime {
    public class TravelNeuronTimer : MonoBehaviour {
        [SerializeField] private SEventManager neuronEventManager;
        [SerializeField] private SEventManager boardEventManager;

        private readonly Dictionary<TravelNeuron, Hex> _travellers = new ();

        private void OnEnable() {
            neuronEventManager.Register(NeuronEvents.OnTravelNeuronReady, CountTravellers);
            neuronEventManager.Register(NeuronEvents.OnTravelNeuronStopped, RemoveTraveller);
            boardEventManager.Register(ExternalBoardEvents.OnAddElement, StoreTraveller);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveElement, RemoveTraveller);
        }

        private void OnDisable() {
            neuronEventManager.Unregister(NeuronEvents.OnTravelNeuronReady, CountTravellers);
            neuronEventManager.Unregister(NeuronEvents.OnTravelNeuronStopped, RemoveTraveller);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddElement, StoreTraveller);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveElement, RemoveTraveller);
        }

        private void RemoveTraveller(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronArgs) {
                return;
            }

            if (neuronArgs.Element.DataProvider.Type == ENeuronType.Travelling) {
                _travellers.Remove((TravelNeuron) neuronArgs.Element);
            }
        }

        private void StoreTraveller(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronArgs) {
                return;
            }

            if (neuronArgs.Element.DataProvider.Type == ENeuronType.Travelling) {
                _travellers[(TravelNeuron) neuronArgs.Element] = Hex.zero;
            }
        }

        private void CountTravellers(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> neuronArgs) {
                return;
            }

            _travellers[(TravelNeuron) neuronArgs.Element] = neuronArgs.Hex;
            if (_travellers.Where(thp => !thp.Key.TurnDone).Select(thp => thp.Value).All(h => h != Hex.zero)) {
                MLogger.LogEditor($" {_travellers.Count} Travelling!");
                neuronEventManager.Raise(NeuronEvents.OnTravellersReady, EventArgs.Empty);
                foreach (var traveller in _travellers.Keys.ToArray()) {
                    _travellers[traveller] = Hex.zero;
                } 
            }
        }
    }
}