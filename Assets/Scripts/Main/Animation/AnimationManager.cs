using System;
using System.Threading.Tasks;

namespace Main.Animation {
    public static class AnimationManager {

        private static readonly AnimationQueue NeuronAnimations = new();
        private static readonly AnimationQueue TileAnimations = new();
        
        public static void Register(Task animation, EAnimationQueue queue) {
            switch (queue) {
                case EAnimationQueue.Neurons:
                    NeuronAnimations.Enqueue(animation);
                    break;
                case EAnimationQueue.Tiles:
                    TileAnimations.Enqueue(animation);
                    PlayQueueAnimations(TileAnimations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(queue), queue, null);
            }
        }

        public static async Task WaitForQueue(EAnimationQueue eQueue) {
            var queue = eQueue switch {
                EAnimationQueue.Neurons => NeuronAnimations,
                EAnimationQueue.Tiles => TileAnimations,
                _ => throw new ArgumentOutOfRangeException(nameof(eQueue), eQueue, null)
            };
            PlayQueueAnimations(queue);
            await Task.WhenAll(queue);
        }

        private static async void PlayQueueAnimations(AnimationQueue queue) {
            while (queue.Count > 0) {
                queue.TryDequeue(out var animation);
                // check if this status check is correct
                if (animation is not {Status: TaskStatus.WaitingToRun}) {
                    continue;
                }

                await Task.Run(() => animation);
            }
        }
    }

    public enum EAnimationQueue {
        Neurons,
        Tiles
    }
}