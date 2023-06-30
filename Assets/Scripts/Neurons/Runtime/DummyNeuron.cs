using System.Threading.Tasks;
using Animation;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board.UI;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using UnityEngine;

namespace Neurons.Runtime {
    public class DummyNeuron : BoardNeuron {


        public Color Tint { get; set; } = Color.white;

        public override Color ConnectionColor { get => Tint * DataProvider.ConnectionColor; }

        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }

        private MUIDummyNeuron UIDummyNeuron => UINeuron as MUIDummyNeuron;
        private SDummyNeuronData DummyData => DataProvider as SDummyNeuronData;

        public DummyNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Dummy);
            Connector = NeuronFactory.GetConnector();
        }

        public override async Task Activate() {
            await AnimationManager.WaitForElement(this);
            ReportTurnDone();
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UIDummyNeuron.SetRuntimeElementData(this);
            UIDummyNeuron.Tint(Tint);
            return UIDummyNeuron;
        }
    }
}