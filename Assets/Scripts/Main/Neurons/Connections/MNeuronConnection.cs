using Main.MyHexBoardSystem.BoardSystem.Interfaces;
using Main.Neurons.Runtime;
using UnityEngine;

namespace Main.Neurons.Connections {
    [RequireComponent(typeof(LineRenderer))]
    public class MNeuronConnection : MonoBehaviour {
        [SerializeField] private float offsetFromEnds;
        
        private LineRenderer Line { get; set; }

        private void Awake() {
            Line = GetComponent<LineRenderer>();
        }

        public void Connect(INeuronBoardController controller, BoardNeuron first, BoardNeuron second) {
            var worldPos1 = controller.HexToWorldPos(first.Position);
            var worldPos2 = controller.HexToWorldPos(second.Position);
            var withOffset1 = worldPos1 + (worldPos2 - worldPos1).normalized * offsetFromEnds;
            var withOffset2 = worldPos2 + (worldPos1 - worldPos2).normalized * offsetFromEnds;

            Line.SetPosition(0, withOffset1);
            Line.SetPosition(1, withOffset2);
            var grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new(first.DataProvider.ConnectionColor, 0), new(second.DataProvider.ConnectionColor, 1)},
                new GradientAlphaKey[] { new(1, 0), new(1, 1)}
            );
            Line.colorGradient = grad;
        }
    }
}