using System;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.SP;
using Types.Neuron;
using Types.Neuron.Runtime;
using Types.Trait;
using UnityEngine;

namespace Menus.EndGame {
    public class MStatCollector : MonoBehaviour {
        [SerializeField] private SEventManager boardEventManager;
        [SerializeField] private SEventManager storyEventManager;

        public Dictionary<ENeuronType, int> NeuronsPlaced { get; private set; }
        public int DummiesSpawned { get; set; }
        public int NeuronsExploded { get; set; }
        public int SPCounter { get; private set; }
        public List<int> TraitCounter { get; private set; }
        public int TilesAdded { get; set; }
        public int TilesRemoved { get; set; }

        private void Awake() {
            NeuronsPlaced = new Dictionary<ENeuronType, int>();
            TraitCounter = Enumerable.Repeat(0, EnumUtil.Count<ETrait>()).ToList();
        }

        private void OnEnable() {
            // neuron counting
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementTurnDone, CountNeuronPlaced);
            boardEventManager.Register(ExternalBoardEvents.OnDummySpawned, CountDummies);
            boardEventManager.Register(ExternalBoardEvents.OnNeuronExploded, CountExploded);
            // tile counting
            boardEventManager.Register(ExternalBoardEvents.OnAddTile, CountTileAdd);
            boardEventManager.Register(ExternalBoardEvents.OnRemoveTile, CountTileRemove);
            // trait & sp counting
            storyEventManager.Register(StoryEvents.OnEvaluate, CountTraitsAndSP);
        }

        private void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementTurnDone, CountNeuronPlaced);
            boardEventManager.Unregister(ExternalBoardEvents.OnDummySpawned, CountDummies);
            boardEventManager.Unregister(ExternalBoardEvents.OnNeuronExploded, CountExploded);
            boardEventManager.Unregister(ExternalBoardEvents.OnAddTile, CountTileAdd);
            boardEventManager.Unregister(ExternalBoardEvents.OnRemoveTile, CountTileRemove);
            storyEventManager.Unregister(StoryEvents.OnEvaluate, CountTraitsAndSP);
        }

        private void CountNeuronPlaced(EventArgs obj) {
            if (obj is not BoardElementEventArgs<IBoardNeuron> placedArgs) {
                return;
            }

            if (!NeuronsPlaced.ContainsKey(placedArgs.Element.DataProvider.Type)) {
                NeuronsPlaced[placedArgs.Element.DataProvider.Type] = 0;
            }
            NeuronsPlaced[placedArgs.Element.DataProvider.Type]++;
        }

        private void CountDummies(EventArgs obj) {
            DummiesSpawned++;
        }

        private void CountExploded(EventArgs obj) {
            NeuronsExploded++;
        }

        private void CountTraitsAndSP(EventArgs obj) {
            if (obj is not StoryEventArgs spArgs) {
                return;
            }

            SPCounter++;
            TraitCounter[(int) spArgs.Story.DecisionEffects.DecidingTrait]++;
        }

        private void CountTileRemove(EventArgs obj) {
            TilesRemoved++;
        }
        
        private void CountTileAdd(EventArgs obj) {
            TilesAdded++;
        }
    }
}