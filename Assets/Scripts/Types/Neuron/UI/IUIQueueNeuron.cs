using System.Threading.Tasks;
using Types.Pooling;

namespace Types.Neuron.UI {
    public interface IUIQueueNeuron : IPoolable {

        void SetRuntimeElementData(Types.Neuron.Runtime.IStackNeuron data);
        void SetQueuePosition(float height);

        Task AnimateDequeue();
        Task AnimateQueueShift(int queueIndex, int stackShiftAmount, int Top3ShiftAmount);
        Task PlayAnimation();
        void StopAnimation();
    }
}