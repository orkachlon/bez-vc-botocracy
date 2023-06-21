using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Types.Animation;

namespace Animation {
    public static class AnimationManager {
        
        private static readonly Dictionary<IAnimatable, List<Task>> Animations = new();
        
        public static void Register(IAnimatable animatable, Task animation) {
            if (!Animations.ContainsKey(animatable)) {
                Animations[animatable] = new List<Task>();
            }
            Animations[animatable].Add(animation);
        }

        public static void Register(Task animation) {
            Register(new DefaultAnimatable(), animation);
        }

        public static async Task WaitForElement(IAnimatable element) {
            if (!Animations.ContainsKey(element)) {
                return;
            }

            await Task.WhenAll(Animations[element]);
            Animations.Remove(element);
        }

        public static async Task WaitForAll() {
            
            await Task.WhenAll(Animations.Values.SelectMany(a => a));
            Animations.Clear();
        }

        public static async Task WaitForAll(IEnumerable<IAnimatable> animatables) {
            var animations = Animations
                .Where(kvp => animatables.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value);
            await Task.WhenAll(animations);
            // clear from dict
            animatables.ToList().ForEach(a => {
                if (Animations.ContainsKey(a)) {
                    Animations.Remove(a);
                }
            });
        }

        public static IAnimatable GetDefaultAnimatable() => new DefaultAnimatable();
    }

    internal class DefaultAnimatable : IAnimatable { }
}