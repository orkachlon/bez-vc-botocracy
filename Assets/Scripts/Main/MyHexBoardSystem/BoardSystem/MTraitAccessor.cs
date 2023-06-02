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
        
        public ETraitType? HexToTrait(Hex hex) {
            foreach (var trait in EnumUtil.GetValues<ETraitType>()) {
                var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
                if (traitHexes.Contains(hex)) {
                    return trait;
                }
            }

            return null;
        }

        public ETraitType? WorldPosToTrait(Vector3 worldPosition) {
            var hex = boardController.WorldPosToHex(worldPosition);
            return HexToTrait(hex);
        }

        public Hex[] GetTraitHexes(ETraitType trait) {
            return boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
        }

        public Color GetColor(ETraitType trait, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            if (traitHexes == null || traitHexes.Length == 0) {
                return Color.magenta;
            }
            return boardController.GetColor(traitHexes[0]);
        }

        public void SetColor(ETraitType trait, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            boardController.SetColor(traitHexes, color);
        }

        public void SetTiles(ETraitType trait, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer) {
            var traitHexes = boardController.Manipulator.GetTriangle(ITraitAccessor.TraitToDirection(trait));
            boardController.SetTiles(traitHexes, tile, tilemapLayer);
        }

        public IEnumerable<ETraitType> GetMaxNeuronsTrait(IEnumerable<ETraitType> fromTraits = null) {
            return neuronsController.GetMaxTrait(fromTraits);
        }
    }
}