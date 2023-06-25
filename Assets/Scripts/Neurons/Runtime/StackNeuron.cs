using Core.Tools.Pooling;
using Core.Utils;
using System.Threading.Tasks;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using Types.Neuron.UI;
using UnityEngine;

namespace Neurons.Runtime {
    public class StackNeuron : IStackNeuron {

        public IBoardNeuron BoardNeuron { get; }
        public INeuronDataBase DataProvider => BoardNeuron.DataProvider;

        public int PlaceInQueue { get; private set; }

        public void SetPlaceInQueue(int value) {
            PlaceInQueue = value;
            if (PlaceInQueue < 3) {
                MLogger.LogEditor("Animating on enqueue!");
                UIQueueNeuron.PlayAnimation();
            }
        }

        protected IUIQueueNeuron UIQueueNeuron { get; set; }
        public int Holders { get; private set; }

        public StackNeuron(IBoardNeuron boardNeuron) {
            BoardNeuron = boardNeuron;
        }

        public IUIQueueNeuron Pool(Transform parent = null) {
            if (Holders == 0) {
                UIQueueNeuron = MObjectPooler.Instance.GetPoolable(DataProvider.GetUIModel(), parent);
            }

            UIQueueNeuron.SetRuntimeElementData(this);
            Holders++;
            return UIQueueNeuron;
        }

        public void Release() {
            Holders--;
            if (Holders != 0) {
                return;
            }
            MObjectPooler.Instance.Release(UIQueueNeuron.GO);
        }

        public Task PlayStackAnimation() {
            return UIQueueNeuron.PlayAnimation();
        }
    }
}