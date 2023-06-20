using MyHexBoardSystem.BoardElements.Neuron.Data;
using Neurons.Runtime;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Data {
    [CreateAssetMenu(fileName = "ExplodeNeuronData", menuName = "Neurons/Explode Neuron Data")]
    public class SExplodeNeuronData : SNeuronDataBase {

        [Header("Explode Neuron"), SerializeField] private AudioClip killSound;
        [SerializeField] private Sprite faceSprite;
        
        public override IBoardNeuron GetNewElement() => NeuronFactory.GetBoardNeuron(Type);

        public Sprite GetFaceSprite() => faceSprite;
        
        public AudioClip GetKillSound() => killSound;
    }
}