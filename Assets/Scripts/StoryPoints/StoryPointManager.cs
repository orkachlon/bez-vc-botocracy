using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.EventSystem;
using Core.EventSystem.EventBus;
using Core.Utils;
using Events.Board;
using Events.EventBindings;
using Events.SP;
using StoryPoints.Types;
using Types.StoryPoint;
using UnityEngine;

namespace StoryPoints {
    public class StoryPointManager : MonoBehaviour {

        [SerializeField] private int spAmountToWinGame;
        
        [Header("Event Managers"), SerializeField]
        protected SEventManager storyEventManager;
        [SerializeField] protected SEventManager boardEventManager;

        [Header("Visuals")] [SerializeField] private MStoryPoint storyPointPrefab;
        
        protected IStoryPoint CurrentStory;
        
        protected ISPProvider SPProvider;
        
        private readonly List<int> _completedSPs = new();

        #region UnityMethods

        protected virtual void Awake() {
            SPProvider = GetComponent<ISPProvider>();
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
            _completedSPs.Add(CurrentStory.Id);
            // first SP
            if (CurrentStory == null || CurrentStory.Evaluated) {
                await NextStoryPoint();
            }
            storyEventManager.Raise(StoryEvents.OnStoryTurn, EventArgs.Empty);
        }

        protected virtual async Task NextStoryPoint() {
            if (CurrentStory is {Evaluated: true}) {
                CurrentStory.RegisterOutcome(SPProvider);
            }
            if (_completedSPs.Count == spAmountToWinGame) {
                DispatchNoMoreSPs();
                return;
            }

            var storyPointData = SPProvider.Next();
            if (storyPointData == null) {
                DispatchNoMoreSPs();
                return;
            }

            CurrentStory?.Destroy();
            InitNewSP(storyPointData);
            await CurrentStory.AwaitInitAnimation();
        }

        protected void InitNewSP(IStoryPointData storyPointData) {
            CurrentStory = Instantiate(storyPointPrefab, Vector3.zero, Quaternion.identity, transform);
            CurrentStory.InitData(storyPointData);
        }

        protected virtual void DispatchNoMoreSPs() {
            MLogger.LogEditor("No more story points in queue!");
            storyEventManager.Raise(StoryEvents.OnNoMoreStoryPoints, EventArgs.Empty);
            EventBus<OnNoMoreStoryPoints>.Raise(new OnNoMoreStoryPoints());
        }
    }
}