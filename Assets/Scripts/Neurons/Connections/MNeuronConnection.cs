using System.Threading.Tasks;
using Animation;
using DG.Tweening;
using Types.Animation;
using Types.Board;
using Types.Neuron.Runtime;
using Types.Pooling;
using UnityEngine;

namespace Neurons.Connections {
    [RequireComponent(typeof(LineRenderer))]
    public class MNeuronConnection : MonoBehaviour, IPoolable, IAnimatable {
        [SerializeField] private Transform mask;
        [SerializeField] private float offsetFromEnds;
        [SerializeField] private float animationDuration;

        private LineRenderer Line { get; set; }
        public GameObject GO => gameObject;
        public IBoardNeuron N1 { get; private set; }
        public IBoardNeuron N2 { get; private set; }

        private float _maskDefaultWidth;

        private void Awake() {
            Line = GetComponent<LineRenderer>();
            _maskDefaultWidth = mask.localScale.x;
        }

        public Task Connect(INeuronBoardController controller, IBoardNeuron first, IBoardNeuron second) {
            N1 = first;
            N2 = second;
            SetConnectionPositions(controller, first, second);
            SetConnectionGradient(first, second);
            AnimationManager.Register(this, AnimateConnection());
            return Task.CompletedTask;
        }

        private void SetConnectionGradient(IBoardNeuron first, IBoardNeuron second) {
            var grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[]
                    {new(first.ConnectionColor, 0), new(second.ConnectionColor, 1)},
                new GradientAlphaKey[] {new(1, 0), new(1, 1)}
            );
            Line.colorGradient = grad;
        }

        private void SetConnectionPositions(INeuronBoardController controller, IBoardNeuron first, IBoardNeuron second) {
            var worldPos1 = controller.HexToWorldPos(first.Position);
            var worldPos2 = controller.HexToWorldPos(second.Position);
            var withOffset1 = worldPos1 + (worldPos2 - worldPos1).normalized * offsetFromEnds;
            var withOffset2 = worldPos2 + (worldPos1 - worldPos2).normalized * offsetFromEnds;

            Line.SetPosition(0, withOffset1);
            Line.SetPosition(1, withOffset2);
        }

        public Task Disconnect() {
            var animationTask = mask.DOScaleY(1, animationDuration).AsyncWaitForCompletion();
            AnimationManager.Register(this, animationTask);
            return animationTask;
        }

        public void Default() {
            mask.localScale = new Vector3(_maskDefaultWidth, 1, 1);
        }

        private async Task AnimateConnection() {
            var lineVec = Line.GetPosition(1) - Line.GetPosition(0);
            mask.SetPositionAndRotation(Line.GetPosition(0) + lineVec * 0.5f,
                Quaternion.LookRotation(Vector3.forward, lineVec.normalized));
            await mask.DOScaleY(0, animationDuration).AsyncWaitForCompletion();
        }
    }
}