﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Types.StoryPoint;
using UnityEngine;

namespace StoryPoints.SPProviders {
    public class JsonSPProvider : MonoBehaviour, ISPProvider {
        
        [Header("Story Points")]
        [SerializeField] private List<TextAsset> storyTextAssets;

        public int Count => _spQueue.Count;

        private Queue<TextAsset> _spQueue;
        private HashSet<int> _outcomeIDs;

        private void Awake() {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            _spQueue = new Queue<TextAsset>();
        }

        private void Start() {
            EnqueueStoryPoints(storyTextAssets);
        }

        private void EnqueueStoryPoints(IEnumerable<TextAsset> eventsToAdd) {
            foreach (var newEvent in eventsToAdd) {
                _spQueue.Enqueue(newEvent);
            }
        }

        public StoryPointData? Next() {
            return ReadStoryPointFromJson();
        }

        public bool IsEmpty() {
            return Count == 0;
        }

        public void ResetProvider() {
            _spQueue.Clear();
            EnqueueStoryPoints(storyTextAssets);
        }

        public void AddOutcome(int outcomeID) {
            if (_outcomeIDs == null) {
                _outcomeIDs = new HashSet<int> { outcomeID };
                return;
            }
            _outcomeIDs.Add(outcomeID);
        }

        public void RemoveOutcome(int outcomeID) {
            if (_outcomeIDs == null || !_outcomeIDs.Contains(outcomeID)) {
                return;
            }
            _outcomeIDs.Remove(outcomeID);
        }

        private StoryPointData ReadStoryPointFromJson() {
            var eventText = _spQueue.Dequeue().text;
            if (string.IsNullOrEmpty(eventText)) {
                throw new JsonSerializationException("Event text was null or empty");
            }
            var data = JsonConvert.DeserializeObject<StoryPointData>(eventText);

            return data;
        }

    }
}