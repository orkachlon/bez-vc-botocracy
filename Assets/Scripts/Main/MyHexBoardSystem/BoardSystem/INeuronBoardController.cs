using System;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;
using UnityEngine;

namespace Main.MyHexBoardSystem.BoardSystem {
    public interface INeuronBoardController : IBoardController<BoardNeuron> {
        int GetTraitTileCount(ETraitType trait);

        void SetColor(Hex[] hexTiles, Color color);
        void SetColor(Hex hexTile, Color color);
        Color GetColor(Hex tile);

        static Hex TraitToDirection(ETraitType trait) {
            var direction = trait switch {
                ETraitType.Entropist => new Hex(0, 1),
                ETraitType.Commander => new Hex(1, 0),
                ETraitType.Entrepreneur => new Hex(1, -1),
                ETraitType.Logistician => new Hex(0, -1),
                ETraitType.Defender => new Hex(-1, 0),
                ETraitType.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
            return direction;
        }
    }
}