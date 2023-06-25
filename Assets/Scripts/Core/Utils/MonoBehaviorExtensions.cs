using System.Collections;
using UnityEngine;

namespace Core.Utils {
    public static class MonoBehaviorExtensions {
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component {
            var t = parent.transform;
            foreach (Transform tr in t) {
                if (tr.CompareTag(tag) && tr.GetComponent<T>()) {
                    return tr.GetComponent<T>();
                }

                var componentFound = tr.gameObject.FindComponentInChildWithTag<T>(tag);
                if (componentFound) {
                    return componentFound;
                }
            }
            return null;
        }
    }
}