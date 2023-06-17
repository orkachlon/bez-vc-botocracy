using System.Collections.Generic;
using OldGridSystem.Tiles;
using Types.Trait;

namespace OldGridSystem.Grid {
    public interface IGrid {
        public void CreateGrid();
        public int CountNeurons(ETrait trait);
        public float CountNeuronsNormalized(ETrait trait);
        public int MaxNeuronsPerTrait();
        public int CountNeurons();
        public IEnumerable<Tile> GetNeighbors(Tile tile);
    }
}