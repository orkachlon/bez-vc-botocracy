using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyHexBoardSystem.Tiles {
    [CreateAssetMenu(fileName = "NewPressableTile", menuName = "Custom Tiles/Pressable Tile")]
    public class PressableTile : HexagonalRuleTile<PressableTile.Neighbor> {
        public bool isPressed;

        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor {
            public const int Pressed = 3;
            public const int NotPressed = 4;
        }

        public override bool RuleMatch(int neighbor, TileBase tile) {
            var other = tile as PressableTile;
            switch (neighbor) {
                case Neighbor.Pressed: return other && other.isPressed;
                case Neighbor.NotPressed: return other && !other.isPressed;
            }
            return base.RuleMatch(neighbor, tile);
        }
    }
}