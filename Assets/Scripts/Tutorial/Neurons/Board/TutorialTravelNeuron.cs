using Neurons.Runtime;
using System.Collections;
using System.Linq;
using Types.Hex.Coordinates;
using UnityEngine;

namespace Assets.Scripts.Tutorial.Neurons.Board {
    public class TutorialTravelNeuron : TravelNeuron {

        public Hex[] UnavailableHexes { get; set; }

        protected override Hex[] GetEmptyNeighbors() {
            var neighbours = Controller.Manipulator.GetNeighbours(Position)
                .Where(h => !UnavailableHexes.Contains(h) && !Controller.Board.GetPosition(h).HasData() && !PickedPositions.ContainsKey(h))
                .ToArray();
            return neighbours;
        }
    }
}