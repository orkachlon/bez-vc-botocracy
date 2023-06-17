using System.Collections.Generic;
using System.Threading.Tasks;
using Types.Animation;

namespace Main.Animation {
    public static class AnimationManager {
        
        private static readonly Dictionary<IAnimatable, Task> NeuronAnimation = new();
        
        public static void Register(IAnimatable neuron, Task animation) {
            NeuronAnimation[neuron] = animation;
        }

        public static void Register(Task animation) {
            NeuronAnimation[new DefaultAnimatable()] = animation;
        }

        public static async Task WaitForElement(IAnimatable element) {
            if (!NeuronAnimation.ContainsKey(element)) {
                return;
            }

            await NeuronAnimation[element];
            NeuronAnimation.Remove(element);
        }

        public static async Task WaitForAll() {
            await Task.WhenAll(NeuronAnimation.Values);
            NeuronAnimation.Clear();
        }

        private static async Task PlayAllAnimations() {
            while (NeuronAnimation.Count > 0) {
                
            }
        }
    }

    internal class DefaultAnimatable : IAnimatable { }
}