using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardElements.Example.Creature;
using ExternBoardSystem.BoardElements.Example.Item;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.Ui.Board;
using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Menu
{
    public class MUIMenuElements : MonoBehaviour
    {
        [SerializeField] private MBoardElementsController<BoardElement, MUIBoardElement> controller;
        
        [Header("Items"), SerializeField] private SItemData apple;
        [SerializeField] private SItemData banana;
        [SerializeField] private SItemData grape;
        
        [Header("Creatures"), SerializeField] private SCreatureData jellyfish;
        [SerializeField] private SCreatureData octopus;
        [SerializeField] private SCreatureData turtle;

        [Header("Menu Buttons"), SerializeField] private Button jellyfishButton;
        [SerializeField] private Button appleButton;
        [SerializeField] private Button bananaButton;
        [SerializeField] private Button grapeButton;
        [SerializeField] private Button octopusButton;
        [SerializeField] private Button turtleButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button pathButton;

        private void Awake()
        {
            BindClickEvents();
            BindArtwork();
        }

        private void BindArtwork()
        {
            jellyfishButton.image.sprite = jellyfish.GetBoardArtwork();
            octopusButton.image.sprite = octopus.GetBoardArtwork();
            turtleButton.image.sprite = turtle.GetBoardArtwork();
            bananaButton.image.sprite = banana.GetBoardArtwork();
            appleButton.image.sprite = apple.GetBoardArtwork();
            grapeButton.image.sprite = grape.GetBoardArtwork();
        }

        private void BindClickEvents()
        {
            jellyfishButton.onClick.AddListener(() => controller.SetElementProvider(jellyfish));
            octopusButton.onClick.AddListener(() => controller.SetElementProvider(octopus));
            turtleButton.onClick.AddListener(() => controller.SetElementProvider(turtle));
            bananaButton.onClick.AddListener(() => controller.SetElementProvider(banana));
            appleButton.onClick.AddListener(() => controller.SetElementProvider(apple));
            grapeButton.onClick.AddListener(() => controller.SetElementProvider(grape));
            removeButton.onClick.AddListener(() => controller.SetElementProvider(null));
        }
    }
}