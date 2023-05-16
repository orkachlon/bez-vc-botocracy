using System.Linq;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using ExternBoardSystem.BoardSystem.Position;

namespace MyHexBoardSystem {
    public class MyBoard : IMyBoard {
        public MyBoardController Controller { get; }
        public Orientation Orientation { get; }
        public Position[] Positions { get; private set; }

        public MyBoard(MyBoardController controller, Orientation orientation) {
            Controller = controller;
            Orientation = orientation;
            GeneratePositions();
        }
        
        public bool HasPosition(Hex point) {
            return GetPosition(point) != null;
        }

        public Position GetPosition(Hex point) {
            return Positions.FirstOrDefault(i => i.Point == point);
        }
        
        private void GeneratePositions()
        {
            var points = Controller.GetHexPoints();
            Positions = new Position[points.Length];
            for (var index = 0; index < points.Length; index++)
            {
                var i = points[index];
                Positions[index] = new Position(i);
            }

            OnCreateBoard();
        }
        
        private void OnCreateBoard()
        {
            Controller.DispatchCreateBoard(this);
        }
    }
}