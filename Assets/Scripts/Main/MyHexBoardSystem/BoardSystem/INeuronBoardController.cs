﻿using System;
using ExternBoardSystem.BoardSystem;
using ExternBoardSystem.BoardSystem.Coordinates;
using Main.MyHexBoardSystem.BoardElements.Neuron;
using Main.Traits;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.MyHexBoardSystem.BoardSystem {
    public interface INeuronBoardController : IBoardController<BoardNeuron> {
        int GetTraitTileCount(ETraitType trait);

        Color GetColor(Hex tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex[] hexTiles, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetColor(Hex hexTile, Color color, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTile(Hex hexTile, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);
        void SetTiles(Hex[] hexTiles, TileBase tile, string tilemapLayer = BoardConstants.BaseTilemapLayer);

        Hex WorldPosToHex(Vector3 position);
        
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
        
        static ETraitType DirectionToTrait(Hex hex) {
            if (hex == new Hex(0, 1)) {
                return ETraitType.Entropist;
            }
            if (hex == new Hex(1, 0)) {
                return ETraitType.Commander;
            }
            if (hex == new Hex(1, -1)) {
                return ETraitType.Entrepreneur;
            }
            if (hex == new Hex(0, -1)) {
                return ETraitType.Logistician;
            }
            if (hex == new Hex(-1, 0)) {
                return ETraitType.Defender;
            }
            if (hex == new Hex(-1, 1)) {
                return ETraitType.Mediator;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }
    }
}