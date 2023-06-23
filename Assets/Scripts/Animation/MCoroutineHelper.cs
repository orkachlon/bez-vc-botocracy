using System;
using System.Collections;
using Core.Utils.Singleton;

namespace Animation {
    public class MCoroutineHelper : MSingleton<MCoroutineHelper> {

        public static void InvokeAfterNextFrame(Action callback) {
            Instance.StartCoroutine(Instance.InvokeAfterSingleFrame(callback));
        }

        private IEnumerator InvokeAfterSingleFrame(Action callback) {
            yield return null;
            callback?.Invoke();
        }
    }
}