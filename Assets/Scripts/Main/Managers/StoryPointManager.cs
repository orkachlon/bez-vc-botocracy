using System;
using System.Collections.Generic;
using Core.EventSystem;
using Core.Utils;
using Events.SP;
using Main.StoryPoints;
using MyHexBoardSystem.Events;
using Types.StoryPoint;
using UnityEngine;

namespace Main.Managers {
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
            storyEventManager.Register(StoryEvents.OnEvaluate, StoryTurn);
            boardEventManager.Register(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        private void OnDisable() {
            storyEventManager.Unregister(StoryEvents.OnEvaluate, StoryTurn);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardSetupComplete, Init);
        }

        #endregion

        private void Init(EventArgs obj) {
            NextStoryPoint();
        }

        private void StoryTurn(EventArgs eventArgs) {
            if (eventArgs is not StoryEventArgs storyEventArgs) {
                return;
            }
            // add to completed SPs
            _completedSPs.Add(storyEventArgs.Story.Id);
            // first SP
            if (_currentStory == null || _currentStory.Evaluated) {
                NextStoryPoint();
            }
            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        private void NextStoryPoint() {
            if (_spProvider.IsEmpty() && _currentStory.Evaluated) {
                DispatchNoMoreSPs();
                return;
            }

            // var storyPointData = ReadStoryPointFromJson();
            var storyPointData = _spProvider.Next();
            if (!storyPointData.HasValue) {
                DispatchNoMoreSPs();
                return;
            }

            _currentStory?.Destroy();
            _currentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentStory.InitData(storyPointData.Value);
        }

        private void DispatchNoMoreSPs() {
            MLogger.LogEditor("No more story points in queue!");
            storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
        }
    }
}