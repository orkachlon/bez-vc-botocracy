﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events.Board;
using Events.Neuron;
using ExternBoardSystem.BoardSystem.Board;
using MyHexBoardSystem.BoardElements.Neuron.Runtime;
using Neurons.Data;
using Neurons.UI;
using Types.Board;
using Types.Board.UI;
using Types.Hex.Coordinates;
using Types.Neuron;
using Types.Neuron.Connections;
using Types.Neuron.Data;
using Types.Neuron.Runtime;
using UnityEngine;

namespace Neurons.Runtime {
    public class ExpandNeuron : BoardNeuron {


        public override Color ConnectionColor { get => DataProvider.ConnectionColor; }

        public override INeuronDataBase DataProvider { get; }
        protected sealed override IBoardNeuronConnector Connector { get; set; }

        private MUIExpandNeuron UIExpandNeuron => UINeuron as MUIExpandNeuron;

        public ExpandNeuron() {
            DataProvider = MNeuronTypeToBoardData.GetNeuronData(ENeuronType.Expanding);
            Connector = NeuronFactory.GetConnector();
        }

        public override async Task Activate() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position);
            var spawnTasks = new List<Task>();
            var i = 0;
            foreach (var neighbour in neighbours) {
                if (!Controller.Board.HasPosition(neighbour) || Controller.Board.GetPosition(neighbour).HasData())
                    continue;
                // expand to this hex
                spawnTasks.Add(SpawnNeighbour(neighbour, i * 50));
                i++;
            }

            await Task.WhenAll(spawnTasks);
            ReportTurnDone();
        }

        public override IUIBoardNeuron Pool() {
            base.Pool();
            UIExpandNeuron.SetRuntimeElementData(this);
            return UIExpandNeuron;
        }

        public override async Task AwaitAddition() {
            await Task.Yield();
            UIExpandNeuron.PlayAddSound();
            UIExpandNeuron.PlayAddAnimation();
            await Connect();
            NeuronEventManager.Raise(NeuronEvents.OnNeuronFinishedAddAnimation, new BoardElementEventArgs<IBoardNeuron>(this, Position));
        }

        public override Hex[] GetAffectedTiles(Hex hex, INeuronBoardController controller = null) {
            if (controller != null) {
                return controller.Manipulator.GetNeighbours(hex).Where(n => !controller.Board.GetPosition(n).HasData()).ToArray();
            }

            return Controller != null ? 
                Controller.Manipulator.GetNeighbours(hex).Where(n => !Controller.Board.GetPosition(n).HasData()).ToArray() : 
                BoardManipulationOddR<IBoardNeuron>.GetNeighboursStatic(hex);
        }

        private async Task SpawnNeighbour(Hex neighbour, int delay = 0) {
            await Task.Delay(delay);
            UIExpandNeuron.PlaySpawnSound();
            var dummy = NeuronFactory.GetBoardNeuron(ENeuronType.Dummy) as DummyNeuron;
            dummy.Tint = DataProvider.ConnectionColor;
            await Controller.AddElement(dummy, neighbour);
        }
    }
}