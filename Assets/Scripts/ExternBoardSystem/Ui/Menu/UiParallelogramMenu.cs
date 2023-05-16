using ExternBoardSystem.BoardSystem.BoardShape;
using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Menu
{
    public class UiParallelogramMenu : UiParentMenu
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private ParallelogramBoardDataShape dataShape;
        [SerializeField] private Slider height;
        [SerializeField] private Slider width;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(OnConfirm);
        }

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        private void OnConfirm()
        {
            dataShape.width = (int) width.value;
            dataShape.height = (int) height.value;
            // boardController.SetBoarDataAndCreate(dataShape);
        }
    }
}