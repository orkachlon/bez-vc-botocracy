using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Utils;
using Types.Animation;

namespace Animation {
    public static class AnimationManager {
        
        private static readonly ConcurrentDictionary<IAnimatable, List<AnimationTask>> Animations = new();

        public static Task Register(IAnimatable animatable, Task animation, string description) {
            if (!Animations.ContainsKey(animatable)) {
                Animations[animatable] = new List<AnimationTask>();
            }
            Animations[animatable].Add(new AnimationTask(animation, description));
            return WaitForElement(animatable);
        }
        
        public static Task Register(IAnimatable animatable, Task animation) {
            return Register(animatable, animation, string.Empty);
        }

        public static Task Register(Task animation) {
            return Register(GetDefaultAnimatable(), animation);
        }

        public static Task WaitForElement(IAnimatable element) {
            if (!Animations.ContainsKey(element)) {
                return Task.CompletedTask;
            }
            
            // MLogger.LogEditor($"Waiting for {Animations[element].Count} {element} animations...");
            return Task.WhenAll(Animations[element].Select(at => at.Animation)).ContinueWith(_ => {
                return Task.FromResult<Action>(() => {
                    try {
                        Animations.TryRemove(element, out var _);
                    }
                    catch (KeyNotFoundException) { }
                });
            });
        }

        public static async Task WaitForAll() {
            MLogger.LogEditor($"Waiting for {Animations.Count} animations...");
            await Task.WhenAll(Animations.Values.SelectMany(a => a.Select(at => at.Animation)));
            Animations.Clear();
        }

        public static async Task WaitForAll(IEnumerable<IAnimatable> animatables) {
            var animations = Animations
                .Where(kvp => animatables.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value)
                .ToList();
            MLogger.LogEditor($"Waiting for {animations.Count} animations...");
            await Task.WhenAll(animations.Select(at => at.Animation));
            // clear from dict
            animatables.ToList().ForEach(a => {
                if (Animations.ContainsKey(a)) {
                    Animations.TryRemove(a, out _);
                }
            });
        }

        private static IAnimatable GetDefaultAnimatable() => new DefaultAnimatable();
    }

    internal class DefaultAnimatable : IAnimatable { }

    internal struct AnimationTask {
        public Task Animation;
        public string Description;

        public AnimationTask(Task animation, string description) {
            Animation = animation;
            Description = description;
        }
    }
}