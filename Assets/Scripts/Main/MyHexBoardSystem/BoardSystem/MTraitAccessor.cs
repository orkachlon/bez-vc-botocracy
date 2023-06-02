using System;
using System.Collections.Generic;
using System.Linq;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements;
using Main.Traits;
using Main.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


namespace Main.MyHexBoardSystem.BoardSystem {
    
    /// <summary>
    ///     A component to be added to objects that need access to the board through the traits
    /// </summary>
    public class MTraitAccessor : MonoBehaviour, ITraitAccessor {

        [SerializeField] private MNeuronBoardController boardController;
        [SerializeField] private MBoardNeuronsController neuronsController;
        
        public ETrait? HexToTrait(Hex hex) {
            foreach (var trait in EnumUtil.GetValues<ETrait>()) {
                var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
                if (traitHexes.Contains(hex)) {
                    return trait;
                }
            }

            return null;
        }

        public ETrait? WorldPosToTrait(Vector3 worldPosition) {
            var hex = boardController.WorldPosToHex(worldPosition);
            return HexToTrait(hex);
        }

        public Hex[] GetTraitHexes(ETrait trait) {
            return boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
        }

        public Color GetColor(ETrait trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            if (traitHexes == null || traitHexes.Length == 0) {
                return Color.magenta;
            }
            return boardController.GetColor(traitHexes[0]);
        }

        public void SetColor(ETrait trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            boardController.SetColor(traitHexes, color);
        }

        public void SetTiles(ETrait trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            boardController.SetTiles(traitHexes, tile, tilemapLayer);
        }

        public IEnumerable<ETrait> GetMaxNeuronsTrait(IEnumerable<ETrait> fromTraits = null) {
            return neuronsController.GetMaxTrait(fromTraits);
        }
    }
}