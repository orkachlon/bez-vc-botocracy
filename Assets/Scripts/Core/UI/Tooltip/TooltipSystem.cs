using Core.Utils.Singleton;
using UnityEngine;

namespace Core.UI.Tooltip {
    public class TooltipSystem : MSingleton<TooltipSystem> {

        [SerializeField] private Tooltip tooltip;

        public static void Show(string content, string header = "") {
            Instance.tooltip.SetText(content, header);
            Instance.tooltip.gameObject.SetActive(true);
        }
        
        public static void Hide() {
            Instance.tooltip.gameObject.SetActive(false);
        }
    }
}