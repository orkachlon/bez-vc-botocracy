using System.Collections.Generic;
using System.Linq;
using Main.StoryPoints.Interfaces;
using Newtonsoft.Json;
using UnityEngine;

namespace Main.StoryPoints.SPProviders {
    public class JsonSPProvider : MonoBehaviour, ISPProvider {
        
        [Header("Story Points")]
        [SerializeField] private List<TextAsset> storyTextAssets;

        public int Count => _spQueue.Count;

        private Queue<TextAsset> _spQueue;
        private int[] _outcomeIDs;

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

        public void Reset() {
            _spQueue.Clear();
            EnqueueStoryPoints(storyTextAssets);
        }

        public void AddOutcome(int outcomeID) {
            _outcomeIDs ??= _outcomeIDs.Append(outcomeID) as int[];
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