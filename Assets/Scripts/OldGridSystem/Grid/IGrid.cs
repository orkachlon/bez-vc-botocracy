using System.Collections.Generic;
using Main.Traits;
using OldGridSystem.Tiles;

namespace OldGridSystem.Grid {
    public interface IGrid {
        public void CreateGrid();
        public int CountNeurons(ETraitType trait);
        public float CountNeuronsNormalized(ETraitType trait);
        public int MaxNeuronsPerTrait();
        public int CountNeurons();
        public IEnumerable<Tile> GetNeighbors(Tile tile);
    }
}