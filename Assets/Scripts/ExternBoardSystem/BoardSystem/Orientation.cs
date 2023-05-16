using System;
using UnityEngine;

namespace ExternBoardSystem.BoardSystem
{
    public enum Orientation
    {
        PointyTop,
        FlatTop
    }

    public struct Orient {
        public readonly float f0, f1, f2, f3;
        public readonly float b0, b1, b2, b3;
        public readonly float StartAngle;

        private static readonly float sqrt3 = Mathf.Sqrt(3);

        public Orient(Orientation orientation) {
            switch (orientation) {
                case Orientation.PointyTop:
                    f0 = sqrt3;
                    f1 = sqrt3 * 0.5f;
                    f2 = 0f;
                    f3 = 3f * 0.5f;
                    b0 = sqrt3 / 3f;
                    b1 = -1f / 3f;
                    b2 = 0f;
                    b3 = 2f / 3f;
                    StartAngle = 0.5f;
                    break;
                case Orientation.FlatTop:
                    f0 = 3f * 0.5f;
                    f1 = 0f;
                    f2 = sqrt3 * 0.5f;
                    f3 = sqrt3;
                    b0 = 2f / 3f;
                    b1 = 0f;
                    b2 = -1f / 3f;
                    b3 = sqrt3 / 3f;
                    StartAngle = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}