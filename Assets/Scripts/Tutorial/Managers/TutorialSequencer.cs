using Assets.Scripts.Tutorial.Neurons.Board;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Main.Managers;
using Neurons.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events.Tutorial;
using Events.UI;
using Tutorial.BG;
using Tutorial.Board;
using Tutorial.Message;
using Tutorial.Neurons;
using Tutorial.StoryPoints;
using Tutorial.Traits.Labels;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Trait;
using Types.Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tutorial.Managers {
    public class TutorialSequencer : MonoBehaviour {

        [Header("Messages"), SerializeField] private MTutorialMessage tutorialMessage;

        [TextArea(5, 15), SerializeField] private string introductionMessage;
        [TextArea(5, 15), SerializeField] private string neuronRewardMessage;
        [TextArea(5, 15), SerializeField] private string personalityMessage;
        [TextArea(5, 15), SerializeField] private string boardEffectMessage;
        [TextArea(5, 15), SerializeField] private string decisionMessage;
        [TextArea(5, 15), SerializeField] private string neuronIntroductionMessage;
        [TextArea(5, 15), SerializeField] private string expandNeuronMessage;
        [TextArea(5, 15), SerializeField] private string travellerNeuronMessage;
        [TextArea(5, 15), SerializeField] private string timerNeuronMessage;
        [TextArea(5, 15), SerializeField] private string cullerNeuronMessage;
        [TextArea(5, 15), SerializeField] private string tutorialEndMessage;

        [Header("Event Managers"), SerializeField] private SEventManager tutorialEventManager;
        [SerializeField] private SEventManager uiEventManager;
        [SerializeField] private SEventManager boardEventManager;

        [Header("Dependencies"), SerializeField]
        private MTutorialNeuronQueue neuronQueue;
        [SerializeField] private MBGTutorialColorController bgColorController;
        [SerializeField] private MBGHoverController bgHoverController;
        [SerializeField] private MTutorialOutcomesController outcomesController;
        [SerializeField] private MTutorialBoardModifier boardModifier;
        [SerializeField] private MTutorialNeuronRewarder neuronRewarder;
        [SerializeField] private MTutorialBoardController boardController;
        [SerializeField] private MTutorialNeuronController neuronController;
        [SerializeField] TutorialStoryPointManager tutorialStoryPointManager;

        [SerializeField] private List<MTutorialTraitLabelController> labels;
        [SerializeField] private string nextSceneName;

        private ETutorialStage _currentStage;
        private int _hoverCounter = 0;
        private int _neuronsPlaced = 0;
        private bool _boardModified = false;

        private async void Start() {
            await TutorialSequence();
            MEventManagerSceneBinder.ResetAllEventManagers();
            await Task.Delay(1000);
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnEnable() {
            uiEventManager.Register(UIEvents.OnTooltipHide, CountTraitHover);
            boardEventManager.Register(ExternalBoardEvents.OnTraitCompassEnter, CountEffectHover);
            boardEventManager.Register(ExternalBoardEvents.OnPlaceElementTurnDone, OnNeuronPlaced);
            boardEventManager.Register(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        private void OnDisable() {
            uiEventManager.Unregister(UIEvents.OnTooltipHide, CountTraitHover);
            boardEventManager.Unregister(ExternalBoardEvents.OnTraitCompassEnter, CountEffectHover);
            boardEventManager.Unregister(ExternalBoardEvents.OnPlaceElementTurnDone, OnNeuronPlaced);
            boardEventManager.Unregister(ExternalBoardEvents.OnBoardModified, OnBoardModified);
        }

        private async Task TutorialSequence() {
            await AwaitStage(Introduction(), ETutorialStage.Introduction);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(1, 0)).HasData());
            await AwaitStage(NeuronRewards(), ETutorialStage.NeuronRewards);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(0, -2)).HasData());
            await Task.Delay(500);
            await AwaitStage(Personalities(), ETutorialStage.Personalities);
            await AsyncHelpers.WaitUntil(() => _hoverCounter >= 3);
            await AwaitStage(BoardEffects(), ETutorialStage.BoardEffects);
            await AsyncHelpers.WaitUntil(() => _neuronsPlaced >= 3);
            await AwaitStage(Decisions(), ETutorialStage.Decisions);
            await AsyncHelpers.WaitUntil(() => _boardModified);
            await AwaitStage(NeuronTypeIntro(), ETutorialStage.NeuronTypeIntro);
            await AwaitStage(ExpandNeuron(), ETutorialStage.ExpanderType);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(1, 0)).HasData());
            await AwaitStage(TravelNeuron(), ETutorialStage.TravellerType);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 2)).HasData());
            await AwaitStage(TimerNeuron(), ETutorialStage.TimerType);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 0)).HasData());
            await AwaitStage(CullerNeuron(), ETutorialStage.CullerType);
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 1)).HasData());
            await AwaitStage(TutorialEnd(), ETutorialStage.End);
            await AsyncHelpers.WaitUntil(() => boardController.Board.Positions.All(p => p.HasData()));
        }

        private async Task AwaitStage(Task stageFunc, ETutorialStage stage) {
            tutorialEventManager.Raise(TutorialEvents.OnBeforeStage, new TutorialStageEventArgs(stage));
            await stageFunc;
            tutorialEventManager.Raise(TutorialEvents.OnAfterStage, new TutorialStageEventArgs(stage));
        }

        private async Task Introduction() {
            _currentStage = ETutorialStage.Introduction;
            // give 1 neuron, disable placement and hide
            neuronQueue.NeuronPool = new HashSet<ENeuronType> { ENeuronType.Dummy };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StopNeurons();
            await neuronQueue.Hide(true);
            // disable BG
            bgColorController.DisableLines(true);
            bgHoverController.DisableHover();
            // hide outcomes
            await outcomesController.Hide(true);
            // introduction - place a neuron
            await DisplayMessage(introductionMessage);
            // enable placement
            neuronQueue.StartNeurons();
        }

        private async Task NeuronRewards() {
            _currentStage = ETutorialStage.NeuronRewards;
            // refresh message
            // add tiles
            await Task.WhenAll(ExpandBoard(t => t == ETrait.Commander ? 5 : 6), tutorialMessage.AwaitHideAnimation());
            // disable board interaction
            neuronQueue.StopNeurons();
            // add reward tile
            neuronRewarder.SelectRewardHex(new Hex(0, -2), 6);
            // show neuron queue
            await Task.WhenAny(neuronQueue.Show(false), DisplayMessage(neuronRewardMessage));
            // reward 2 neurons
            neuronQueue.EnqueueFromPool(2);
            // disable all tiles but two
            boardController.DisableHexes();
            boardController.EnableHexes(new[]{new Hex(0, -1), new Hex(0, -2) });
            // enabled board interaction
            neuronQueue.StartNeurons();
        }

        private async Task Personalities() {
            _currentStage = ETutorialStage.Personalities;
            // enable hexes to remove shadow effect
            boardController.DisableHexes(new[]{new Hex(0, -1), new Hex(0, -2) });
            // split BG to 3
            bgColorController.EnableLines();
            // enable bg colors
            bgColorController.IsSPEnabled = true;
            tutorialStoryPointManager.InitTutorialSP();
            // show labels
            var labelShowTasks = new List<Task>();
            foreach (var label in labels) {
                label.IsSPEnabled = true;
                labelShowTasks.Add(label.Show());
                label.UpdateColor();
            }
            await Task.WhenAny(labelShowTasks);
            // display message
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(personalityMessage);
        }

        private async Task BoardEffects() {
            _currentStage = ETutorialStage.BoardEffects;
            await tutorialMessage.AwaitHideAnimation();
            // show SP
            // show message
            await Task.WhenAny(
                tutorialStoryPointManager.ShowTutorialSP(),
                DisplayMessage(boardEffectMessage));
            tutorialStoryPointManager.IsSPEnabled = false;
            // foreach (var label in labels) {
            //     label.IsSPEnabled = true;
            //     label.UpdateColor();
            // }
            // enable effect hover
            bgHoverController.EnableHover();
            boardController.EnableHexes(new[] { new Hex(1, -1), new Hex(-1, 0), new Hex(0, 1) });
        }

        private async Task Decisions() {
            _currentStage = ETutorialStage.Decisions;
            // refresh message
            await tutorialMessage.AwaitHideAnimation();
            await Task.WhenAny(/*outcomesController.Show(),*/ DisplayMessage(decisionMessage));
            // place reward tiles
            neuronRewarder.RewardRandomTiles();
            // enable SP and let player play
            neuronController.IsSPEnabled = true;
            neuronQueue.IsSPEnabled = true;
            tutorialStoryPointManager.IsSPEnabled = true;
            boardController.EnableHexes();
        }

        private async Task NeuronTypeIntro() {
            _currentStage = ETutorialStage.NeuronTypeIntro;
            // disable SP and bg effects
            neuronController.IsSPEnabled = false;
            bgColorController.IsSPEnabled = false;
            neuronQueue.IsSPEnabled = false;
            bgHoverController.DisableHover();
            bgColorController.ToDefaultColors();
            labels.ForEach(l => {
                l.IsSPEnabled = false;
                l.UpdateColor();
            });
            // refresh message
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(neuronIntroductionMessage);
            // reward enough neurons to fill board
            neuronQueue.EnqueueFromPool(boardController.Board.Positions.Count(p => !p.HasData()) - neuronQueue.Count);
            // wait till board full
            await AsyncHelpers.WaitUntil(() => boardController.Board.Positions.All(p => p.HasData()));
            // hide anything SP related
            await Task.WhenAny(neuronQueue.Clear() , outcomesController.Hide());
            labels.ForEach(l => l.IsSPEnabled = false);
            // remove all tiles and recreate board
            await RemoveAllTraitTiles();
            await ExpandBoard(t => 6);
        }

        private async Task ExpandNeuron() {
            _currentStage = ETutorialStage.ExpanderType;
            // refresh message
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(expandNeuronMessage);
            // enable only the single hex
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(1, 0) });
            // change queue to have only a single expand neuron
            await neuronQueue.Clear();
            neuronQueue.NeuronPool = new(){ ENeuronType.Expanding };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
        }

        private async Task TravelNeuron() {
            _currentStage = ETutorialStage.TravellerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(travellerNeuronMessage);
            neuronQueue.NeuronPool = null;
            var modifiedTraveller = new TutorialTravelNeuron {
                UnavailableHexes = new[] { new Hex(0, 2), new Hex(-1, 1) }
            };
            neuronQueue.Enqueue(new StackNeuron(modifiedTraveller));
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(-1, 2) });
        }

        private async Task TimerNeuron() {
            _currentStage = ETutorialStage.TimerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(timerNeuronMessage);
            neuronQueue.NeuronPool = new() { ENeuronType.Decaying };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] {new Hex(-1, 0) });
        }

        private async Task CullerNeuron() {
            _currentStage = ETutorialStage.CullerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(cullerNeuronMessage);
            neuronQueue.NeuronPool = new() { ENeuronType.Exploding };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(-1, 1) });
        }

        private async Task TutorialEnd() {
            _currentStage = ETutorialStage.End;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(tutorialEndMessage);
            // provide neurons
            neuronQueue.NeuronPool = new() { ENeuronType.Expanding, ENeuronType.Travelling, ENeuronType.Decaying };
            neuronQueue.EnqueueFromPool(20);
            // enable board
            boardController.EnableHexes();
        }


        #region UIElements

        private async Task DisplayMessage(string msgText) {
            tutorialMessage.SetText(msgText);
            await tutorialMessage.AwaitShowAnimation();
        }

        #endregion
        
        #region BoardModification
        
        private async Task RemoveAllTraitTiles() {
            var removeTasks = TutorialConstants.Traits
                .Select(trait => 
                    boardModifier.RemoveTilesFromTrait(trait, boardController.GetTraitTileCount(trait)));

            await Task.WhenAll(removeTasks);
            neuronRewarder.Clear();
        }

        private async Task ExpandBoard(Func<ETrait, int> amountGiver) {
            foreach (var trait in TutorialConstants.Traits) {
                await boardModifier.AddTilesToTrait(trait, amountGiver.Invoke(trait));
            }
        }

        #endregion

        #region EventHandlers
        
        private void CountTraitHover(EventArgs args) {
            if (_currentStage == ETutorialStage.Personalities) {
                _hoverCounter++;
            }
        }

        private void CountEffectHover(EventArgs args) {
            if (_currentStage == ETutorialStage.BoardEffects) {
                _hoverCounter++;
            }
        }

        private void OnBoardModified(EventArgs args) {
            if (_currentStage == ETutorialStage.Decisions) {
                _boardModified = true;
            }
        }

        private void OnNeuronPlaced(EventArgs args) {
            if (_currentStage == ETutorialStage.BoardEffects) {
                _neuronsPlaced++;
            }
        }

        #endregion
    }
}