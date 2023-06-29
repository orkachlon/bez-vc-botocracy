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
        protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager boardEventManager;

        [Header("Visuals")] [SerializeField] private MStoryPoint storyPointPrefab;
        
        protected IStoryPoint _currentStory;
        protected ISPProvider _spProvider;

        private readonly List<int> _completedSPs = new();

        #region UnityMethods

        protected virtual void Awake() {
            _spProvider = GetComponent<ISPProvider>();
        }

        protected virtual void OnEnable() {
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, StoryTurn);
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        protected virtual void OnDisable() {
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, StoryTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #endregion

        protected virtual async void Init(EventArgs obj) {
            await NextStoryPoint();
        }

        protected virtual async void StoryTurn(EventArgs eventArgs) {
            //if (eventArgs is not StoryEventArgs storyEventArgs) {
            //    return;
            //}
            // add to completed SPs
            _completedSPs.Add(_currentStory.Id);
            // first SP
            if (_currentStory == null || _currentStory.Evaluated) {
                await NextStoryPoint();
            }
            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        protected virtual async Task NextStoryPoint() {
            if (_spProvider.IsEmpty() && _currentStory.Evaluated) {
                DispatchNoMoreSPs();
                return;
            }

            var storyPointData = _spProvider.Next();
            if (!storyPointData.HasValue) {
                DispatchNoMoreSPs();
                return;
            }

            _currentStory?.Destroy();
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(storyPointData.Value);
            await _currentStory.AwaitInitAnimation();
        }

        protected virtual void DispatchNoMoreSPs() {
            MLogger.LogEditor("No more story points in queue!");
            storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
        }
    }
}