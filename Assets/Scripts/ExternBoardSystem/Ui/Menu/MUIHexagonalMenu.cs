using ExternBoardSystem.BoardSystem.BoardShape;
using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Menu
{
    public class MUIHexagonalMenu : MUIParentMenu
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private SHexagonalBoardDataShape dataShape;
        [SerializeField] private Slider slider;

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
            dataShape.radius = (int) slider.value;
            // boardController.SetBoarDataAndCreate(dataShape);
        }
    }
}