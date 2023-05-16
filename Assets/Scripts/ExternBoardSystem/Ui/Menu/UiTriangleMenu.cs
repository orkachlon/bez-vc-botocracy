using ExternBoardSystem.BoardSystem.BoardShape;
using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Menu
{
    public class UiTriangleMenu : UiParentMenu
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private TriangleBoardDataShape dataShape;
        [SerializeField] private Slider size;

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
            dataShape.size = (int) size.value;
            boardController.SetBoarDataAndCreate(dataShape);
        }
    }
}