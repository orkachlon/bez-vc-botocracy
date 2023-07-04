using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Core.Utils {
    public static class EnumUtil {
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T GetRandom<T>() {
            var allValues = GetValues<T>().ToArray();
            var rndSelection = Random.Range(0, allValues.Length);
            return allValues[rndSelection];
        }

        public static int Count<T>() {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}