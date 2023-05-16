﻿using UnityEngine;
using UnityEngine.UI;

namespace ExternBoardSystem.Ui.Util {
    public class UiTutorial : MonoBehaviour
    {
        [SerializeField] private Button close;
        [SerializeField] private GameObject content;

        private void Awake()
        {
            close.onClick.AddListener(Close);
        }

        private void Close()
        {
            content.SetActive(false);
        }
    }
}