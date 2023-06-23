using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.SP;
using Types.StoryPoint;
using UnityEngine;

namespace StoryPoints {
    public class StoryPointManager : MonoBehaviour {

        [Header("Event Managers"), SerializeField]
        private SEventManager storyEventManager;
        [SerializeField] private SEventManager boardEventManager;

        [Header("Visuals")] [SerializeField] private MStoryPoint storyPointPrefab;
        
        private IStoryPoint _currentStory;
        private ISPProvider _spProvider;

        private readonly List<int> _completedSPs = new();

        #region UnityMethods

        private void Awake() {
            _spProvider = GetComponent<ISPProvider>();
        }

        private void OnEnable() {
            // todo change this to happen only after user closed outcome popup
            storyEventManager.Register(StoryEvents.OnEvaluate, StoryTurn);
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, StoryTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #endregion

        private async void Init(EventArgs obj) {
            await NextStoryPoint();
        }

        private async void StoryTurn(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }
            // add to completed SPs
            _completedSPs.Add(storyEventArgs.Story.Id);
            // first SP
            if (_currentStory == null || _currentStory.Evaluated) {
                await NextStoryPoint();
            }
            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        private async Task NextStoryPoint() {
            if (_spProvider.IsEmpty() && _currentStory.Evaluated) {
                DispatchNoMoreSPs();
                return;
            }

            var storyPointData = _spProvider.Next();
            if (!storyPointData.HasValue) {
                DispatchNoMoreSPs();
                return;
            }

            if (_currentStory != null) {
                await _currentStory.AwaitRemoveAnimation();
                _currentStory.Destroy();
            }
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(storyPointData.Value);
            await _currentStory.AwaitInitAnimation();
        }

        private void DispatchNoMoreSPs() {
            MLogger.LogEditor("No more story points in queue!");
            storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
        }
    }
}