using System.Threading.Tasks;
using Types.Neuron.Data;
using Types.Neuron.UI;
using UnityEngine;

namespace Types.Neuron.Runtime {
    public interface IStackNeuron {
        public IBoardNeuron BoardNeuron { get; }
        public INeuronDataBase DataProvider { get; }

        public int PlaceInQueue { get; }
        void SetPlaceInQueue(int value);

        public IUIQueueNeuron Pool(Transform parent = null);
        public Task PlayStackAnimation();
    }
}