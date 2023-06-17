using System.Threading.Tasks;
using Core.Utils;
using MyHexBoardSystem.BoardSystem;
using UnityEngine;

namespace Main.Tests {
    public class MHexHashTester : MonoBehaviour {
        [SerializeField] private MNeuronBoardController controller;

        private async void Start() {
            await Task.Run(Test);
        }

        private void Test() { 
            foreach (var hex in controller.GetHexPoints()) {
                foreach (var otherHex in controller.GetHexPoints()) {
                    if (hex == otherHex) {
                        continue;
                    }

                    if (hex.GetHashCode() == otherHex.GetHashCode()) {
                        MLogger.LogEditor($"Hex {hex} -> {hex.GetHashCode()} == {otherHex.GetHashCode()} <- {otherHex}");
                    }
                } 
            }
            MLogger.LogEditor("No hash collisions found!");
        }
    }
}