using Assets.Scripts.Tutorial.Board;
using Assets.Scripts.Tutorial.Neurons.Board;
using Assets.Scripts.Tutorial.StoryPoints;
using Core.EventSystem;
using Core.Utils;
using Events.Board;
using Events.Tutorial;
using Main.Managers;
using Neurons.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events.UI;
using Tutorial.BG;
using Tutorial.Board;
using Tutorial.Message;
using Tutorial.Neurons;
using Tutorial.Traits.Labels;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Trait;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tutorial.Managers {
    public class TutorialSequencer : MonoBehaviour {

        [Header("Messages"), SerializeField] private MTutorialMessage tutorialMessage;

        [TextArea(5, 7), SerializeField] private string introductionMessage;
        [TextArea(5, 7), SerializeField] private string neuronRewardMessage;
        [TextArea(5, 7), SerializeField] private string personalityMessage;
        [TextArea(5, 7), SerializeField] private string boardEffectMessage;
        [TextArea(5, 7), SerializeField] private string decisionMessage;
        [TextArea(5, 7), SerializeField] private string neuronIntroductionMessage;
        [TextArea(5, 7), SerializeField] private string expandNeuronMessage;
        [TextArea(5, 7), SerializeField] private string travellerNeuronMessage;
        [TextArea(5, 7), SerializeField] private string timerNeuronMessage;
        [TextArea(5, 7), SerializeField] private string cullerNeuronMessage;
        [TextArea(5, 7), SerializeField] private string tutorialEndMessage;

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

        private TutorialStage _currentStage;
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
            await Introduction();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(1, 0)).HasData());
            await NeuronRewards();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(0, -2)).HasData());
            await Personalities();
            await AsyncHelpers.WaitUntil(() => _hoverCounter >= 3);
            await Task.Delay(1000);
            await BoardEffects();
            await AsyncHelpers.WaitUntil(() => _neuronsPlaced >= 3);
            await Decisions();
            await AsyncHelpers.WaitUntil(() => _boardModified);
            await NeuronTypeIntro();
            await ExpandNeuron();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(1, 0)).HasData());
            await TravelNeuron();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 2)).HasData());
            await TimerNeuron();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 0)).HasData());
            await CullerNeuron();
            await AsyncHelpers.WaitUntil(() => boardController.Board.GetPosition(new Hex(-1, 1)).HasData());
            await TutorialEnd();
            await AsyncHelpers.WaitUntil(() => boardController.Board.Positions.All(p => p.HasData()));
        }

        private async Task Introduction() {
            _currentStage = TutorialStage.Introdution;
            // give 1 neuron, disable placement and hide
            neuronQueue.neuronPool = new() { Types.Neuron.ENeuronType.Dummy };
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
            _currentStage = TutorialStage.NeuronRewards;
            // refresh message
            // add tiles
            await Task.WhenAll(ExpandBoard(t => t == ETrait.Commander ? 5 : 6), tutorialMessage.AwaitHideAnimation());
            // disable board interaction
            neuronQueue.StopNeurons();
            // reward 2 neurons
            neuronQueue.EnqueueFromPool(2);
            // add reward tile
            neuronRewarder.SelectRewardHex(new Hex(0, -2), 6);
            // show neuron queue
            await Task.WhenAny(neuronQueue.Show(false), DisplayMessage(neuronRewardMessage));
            // disable all tiles but two
            boardController.DisableHexes();
            boardController.EnableHexes(new[]{new Hex(0, -1), new Hex(0, -2) });
            // todo indicate which tiles are available
            // enabled board interaction
            neuronQueue.StartNeurons();
        }

        private async Task Personalities() {
            _currentStage = TutorialStage.Personalities;
            // enable hexes to remove shadow effect
            boardController.DisableHexes();
            // split BG to 3
            bgColorController.EnableLines();
            // show labels
            var labelShowTasks = new List<Task>();
            foreach (var label in labels) {
                label.IsSPEnabled = false;
                labelShowTasks.Add(label.Show());
            }
            await Task.WhenAny(labelShowTasks);
            // display message
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(personalityMessage);
        }

        private async Task BoardEffects() {
            _currentStage = TutorialStage.BoardEffects;
            await tutorialMessage.AwaitHideAnimation();
            // show SP
            // show message
            await Task.WhenAny(
                tutorialStoryPointManager.ShowTutorialSP(),
                DisplayMessage(boardEffectMessage));
            tutorialStoryPointManager.IsSPEnabled = false;
            foreach (var label in labels) {
                label.IsSPEnabled = true;
                label.UpdateColor();
            }
            // enable effect hover
            bgHoverController.EnableHover();
            boardController.EnableHexes(new[] { new Hex(1, -1), new Hex(-1, 0), new Hex(0, 1) });
        }

        private async Task Decisions() {
            _currentStage = TutorialStage.Decisions;
            // refresh message
            await tutorialMessage.AwaitHideAnimation();
            await Task.WhenAny(/*outcomesController.Show(),*/ DisplayMessage(decisionMessage));
            // place reward tiles
            neuronRewarder.RewardRandomTiles();
            // enable SP and let player play
            neuronController.IsSPEnabled = true;
            bgColorController.IsSPEnabled = true;
            tutorialStoryPointManager.IsSPEnabled = true;
            boardController.EnableHexes();
        }

        private async Task NeuronTypeIntro() {
            _currentStage = TutorialStage.NeuronTypeIntro;
            // disable SP and bg effects
            neuronController.IsSPEnabled = false;
            bgColorController.IsSPEnabled = false;
            bgHoverController.DisableHover();
            bgColorController.ToDefaultColors();
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
            _currentStage = TutorialStage.ExpanderType;
            // refresh message
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(expandNeuronMessage);
            // enable only the single hex
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(1, 0) });
            // change queue to have only a single expand neuron
            await neuronQueue.Clear();
            neuronQueue.neuronPool = new(){ ENeuronType.Expanding };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
        }

        private async Task TravelNeuron() {
            _currentStage = TutorialStage.TravellerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(travellerNeuronMessage);
            neuronQueue.neuronPool = null;
            var modifiedTraveller = new TutorialTravelNeuron {
                UnavailableHexes = new[] { new Hex(0, 2), new Hex(-1, 1) }
            };
            neuronQueue.Enqueue(new StackNeuron(modifiedTraveller));
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(-1, 2) });
        }

        private async Task TimerNeuron() {
            _currentStage = TutorialStage.TimerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(timerNeuronMessage);
            neuronQueue.neuronPool = new() { ENeuronType.Decaying };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] {new Hex(-1, 0) });
        }

        private async Task CullerNeuron() {
            _currentStage = TutorialStage.CullerType;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(cullerNeuronMessage);
            neuronQueue.neuronPool = new() { ENeuronType.Exploding };
            neuronQueue.EnqueueFromPool();
            neuronQueue.StartNeurons();
            boardController.DisableHexes();
            boardController.EnableHexes(new Hex[] { new Hex(-1, 1) });
        }

        private async Task TutorialEnd() {
            _currentStage = TutorialStage.End;
            await tutorialMessage.AwaitHideAnimation();
            await DisplayMessage(tutorialEndMessage);
            // provide neurons
            neuronQueue.neuronPool = new() { ENeuronType.Expanding, ENeuronType.Travelling, ENeuronType.Decaying };
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
            foreach (var trait in TutorialConstants.Traits) {
                await boardModifier.RemoveTilesFromTrait(trait, boardController.GetTraitTileCount(trait));
            }
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
            if (_currentStage == TutorialStage.Personalities) {
                _hoverCounter++;
            }
        }

        private void CountEffectHover(EventArgs args) {
            if (_currentStage == TutorialStage.BoardEffects) {
                _hoverCounter++;
            }
        }

        private void OnBoardModified(EventArgs args) {
            if (_currentStage == TutorialStage.Decisions) {
                _boardModified = true;
            }
        }

        private void OnNeuronPlaced(EventArgs args) {
            if (_currentStage == TutorialStage.BoardEffects) {
                _neuronsPlaced++;
            }
        }

        #endregion
    }

    internal enum TutorialStage {
        Introdution,
        NeuronRewards,
        Personalities,
        BoardEffects,
        Decisions,
        NeuronTypeIntro,
        ExpanderType,
        TravellerType,
        TimerType,
        CullerType,
        End
    }
}