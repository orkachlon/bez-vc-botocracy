using System;
using Core.EventSystem;
using Core.Utils.Singleton;
using Events.UI;
using UnityEngine;

namespace Core.UI.Tooltip {
    public class TooltipSystem : MSingleton<TooltipSystem> {

        [SerializeField] private Tooltip tooltip;
        [SerializeField] private SEventManager UIEventManager;

        public static void Show(string content, string header = "") {
            Instance.tooltip.SetText(content, header);
            Instance.tooltip.gameObject.SetActive(true);
            Instance.UIEventManager.Raise(UIEvents.OnTooltipShow, EventArgs.Empty);
        }
        
        public static void Hide() {
            var wasShown = Instance.tooltip.gameObject.activeInHierarchy;
            Instance.tooltip.gameObject.SetActive(false);
            if (wasShown) {
                Instance.UIEventManager.Raise(UIEvents.OnTooltipHide, EventArgs.Empty);
            }
        }
    }
}