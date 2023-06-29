using System;
using System.Linq;
using Core.EventSystem;
using Events.Board;
using Events.Tutorial;
using MyHexBoardSystem.BoardSystem;
using Types.Hex.Coordinates;
using Types.Trait;
using UnityEngine;

namespace Tutorial.Traits {
    public class MTutorialTraitAccessor : MTraitAccessor {

        [SerializeField] private SEventManager tutorialEventManager;
        
        protected override void CheckForFullBoard(EventArgs obj) {
            if (TraitHexes.Keys
                .All(t => TraitHexes[t]
                    .All(h => neuronsController.Board.HasPosition(h) &&
                              neuronsController.Board.GetPosition(h).HasData()))) {
                tutorialEventManager.Raise(TutorialEvents.OnBoardFull, EventArgs.Empty);
            }
        }

        public override ETrait? DirectionToTrait(Hex hex) {
            if (hex == new Hex(1, 0) || hex == new Hex(1, -1)) {
                return ETrait.Commander;
            }
            if (hex == new Hex(-1, 0) || hex == new Hex(0, -1)) {
                return ETrait.Entrepreneur;
            }
            if (hex == new Hex(-1, 1) || hex == new Hex(0, 1)) {
                return ETrait.Mediator;
            }
            if (hex == Hex.zero) {
                return null;
            }

            throw new ArgumentOutOfRangeException(nameof(hex), hex, null);
        }

        public override Hex TraitToDirection(ETrait trait) {
            return trait switch {
                ETrait.Commander => new Hex(1, 0),
                ETrait.Entrepreneur => new Hex(0, -1),
                ETrait.Mediator => new Hex(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }

        public Hex[] TraitToDirections(ETrait trait) {
            return trait switch {
                ETrait.Commander => new Hex[] { new Hex(1, 0), new Hex(1, -1) },
                ETrait.Entrepreneur => new Hex[] { new Hex(-1, 0), new Hex(0, -1) },
                ETrait.Mediator => new Hex[] { new Hex(-1, 1), new Hex(0, 1) },
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }

        public override Vector3 TraitToVectorDirection(ETrait trait) {
            return trait switch {
                ETrait.Commander => Quaternion.AngleAxis(60, Vector3.forward) * Vector3.up,
                ETrait.Entrepreneur => Quaternion.AngleAxis(180, Vector3.forward) * Vector3.up,
                ETrait.Mediator => Quaternion.AngleAxis(-60, Vector3.forward) * Vector3.up,
                _ => throw new ArgumentOutOfRangeException(nameof(trait), trait, null)
            };
        }
    }
}