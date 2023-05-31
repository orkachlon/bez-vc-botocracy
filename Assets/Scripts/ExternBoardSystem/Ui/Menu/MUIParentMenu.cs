using ExternBoardSystem.BoardElements;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.Ui.Util;
using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Menu
{
    public abstract class MUIParentMenu : MonoBehaviour, IBackHandler
    {
        [Header("Dependencies"), SerializeField]
        protected MBoardController<BoardElement> boardController;

        [Header("Internal"), SerializeField] protected GameObject content;

        [SerializeField] protected Button hideButton;
        [SerializeField] protected Button showButton;
        [SerializeField] protected Button xButton;

        public void Show()
        {
            MBackButton.Instance.Push(this);
            content.SetActive(true);
            OnShow();
        }

        public void Back()
        {
            Hide();
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            if (showButton)
                showButton.onClick.AddListener(Show);
            if (hideButton)
                hideButton.onClick.AddListener(MBackButton.Instance.Pop);
            if (xButton)
                xButton.onClick.AddListener(MBackButton.Instance.Pop);
        }

        protected void Hide()
        {
            content.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnShow()
        {
        }
    }
}