using Core.EventSystem;
using Events.Tutorial;
using System;
using System.Threading.Tasks;
using Tutorial.Message;
using UnityEngine;

namespace Tutorial.Managers {
    public class TutorialSequencer : MonoBehaviour {

        [SerializeField] private MTutorialMessage tutorialMessagePrefab;

        [Header("Messages"), TextArea(5, 7), SerializeField] private string introductionMessage;

        [Header("Event Managers"), SerializeField] private SEventManager tutorialEventManager;


        private int _currentStage;

        private async void Start() {
            await TutorialSequence();
        }

        private async Task TutorialSequence() {
            _currentStage = 0;
            DisableBGInteraction();
            DisableBoardInteraction();
            HideNeuronQueue();
            // introduction - place a neuron
            await DisplayMessage(introductionMessage);
            EnableBoardInteraction();
        }

        private async Task DisplayMessage(string msgText) {
            var msg = Instantiate(tutorialMessagePrefab);
            msg.SetText(msgText);
            await msg.AwaitEntranceAnimation();
        }

        #region Board Interaction

        private void DisableBoardInteraction() {
            tutorialEventManager.Raise(TutorialEvents.OnDisableBoard, EventArgs.Empty);
        }

        private void EnableBoardInteraction() {
            tutorialEventManager.Raise(TutorialEvents.OnEnableBoard, EventArgs.Empty);
        }

        #endregion

        #region BackGround

        private void DisableBGInteraction() {
            tutorialEventManager.Raise(TutorialEvents.OnDisableBGInteraction, EventArgs.Empty);
        }

        private void EnableBGInteraction() {
            tutorialEventManager.Raise(TutorialEvents.OnEnableBGInteraction, EventArgs.Empty);
        }

        #endregion

        #region UIElements

        private void HideNeuronQueue() {
            tutorialEventManager.Raise(TutorialEvents.OnHideNeuronQueue, EventArgs.Empty);
        }

        private void ShowNeuronQueue() {
            tutorialEventManager.Raise(TutorialEvents.OnShowNeuronQueue, EventArgs.Empty);
        }

        #endregion

    }
}