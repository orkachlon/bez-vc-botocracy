using System.Collections.Generic;
using Core.Utils.Singleton;
using ExternBoardSystem.Ui.Menu;
using UnityEngine;

namespace ExternBoardSystem.Ui.Util {
    public interface IBackHandler
    {
        void Show();
        void Back();
    }

    public class MBackButton : MSingleton<MBackButton>
    {
        private readonly Stack<IBackHandler> _windows = new Stack<IBackHandler>();

        [SerializeField] private MUIMenu uiMenu;

        public void Push(IBackHandler window)
        {
            _windows?.Push(window);
        }

        public void Clear()
        {
            _windows.Clear();
        }

        public void Pop()
        {
            if (_windows.Count < 1)
                return;
            var window = _windows.Pop();
            window.Back();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_windows.Count < 1)
                    uiMenu.Show();
                else
                    Pop();
            }
        }
    }
}