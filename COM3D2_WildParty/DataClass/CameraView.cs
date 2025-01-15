using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    internal class CameraView
    {
        public Vector3 TargetPosition;
        public float Distance;
        public Vector2 AroundAngle;

        public CameraView()
        {
            TargetPosition = new Vector3();
            Distance = 0;
            AroundAngle = new Vector2();
        }

        public CameraView(Vector3 target, float distance, Vector2 angle)
        {
            TargetPosition = target;
            Distance = distance;
            AroundAngle = angle;
        }
    }
}
