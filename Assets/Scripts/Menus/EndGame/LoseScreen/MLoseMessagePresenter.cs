﻿using System;
using DG.Tweening;
using Events.General;
using TMPro;
using UnityEngine;

namespace Menus.EndGame.LoseScreen {
    public class MLoseMessagePresenter : MonoBehaviour {
        [Header("Animation"), SerializeField] private TextMeshProUGUI loseMessageTextfield;
        [SerializeField, Range(0, 0.5f)] private float singleCharAnimationDuration;
        
        [Header("Messages"), SerializeField, TextArea(5, 15)] private string noMoreNeuronsMessage;
        [SerializeField, TextArea(5, 15)] private string traitOutOfTilesMessage;
        [SerializeField, TextArea(5, 15)] private string boardFullMessage;
        [SerializeField, TextArea(5, 15)] private string SPFailedMessage;


        public void ShowMessage(ELoseReason reason) {
            var message = GetMessageText(reason);
            DOVirtual.Int(0, message.Length, singleCharAnimationDuration * message.Length, i => loseMessageTextfield.text = message[..i])
                .SetEase(Ease.Linear);
        }

        private string GetMessageText(ELoseReason reason) {
            return reason switch {
                ELoseReason.NoMoreNeurons => noMoreNeuronsMessage,
                ELoseReason.BoardFull => boardFullMessage,
                ELoseReason.TraitOutOfTiles => traitOutOfTilesMessage,
                ELoseReason.FromSP => SPFailedMessage,
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
        }
    }
}