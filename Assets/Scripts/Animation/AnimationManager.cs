using System.Collections.Generic;
using System.Threading.Tasks;
using Types.Animation;

namespace Animation {
    public static class AnimationManager {
        
        private static readonly Dictionary<IAnimatable, Task> Animations = new();
        
        public static void Register(IAnimatable animatable, Task animation) {
            Animations[animatable] = animation;
        }

        public static void Register(Task animation) {
            Animations[new DefaultAnimatable()] = animation;
        }

        public static async Task WaitForElement(IAnimatable element) {
            if (!Animations.ContainsKey(element)) {
                return;
            }

            await Animations[element];
            Animations.Remove(element);
        }

        public static async Task WaitForAll() {
            await Task.WhenAll(Animations.Values);
            Animations.Clear();
        }
    }

    internal class DefaultAnimatable : IAnimatable { }
}