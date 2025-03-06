using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class CameraDataInJson
    {
        [JsonProperty]
        private string PosString;
        [JsonProperty]
        private string TargetPosString;
        [JsonProperty]
        private string AroundAngleString;

        public float Distance;

        public Vector3 Pos
        {
            get { return Util.ParseVector3RawString(PosString); }
        }

        public Vector3 TargetPos
        {
            get { return Util.ParseVector3RawString(TargetPosString); }
        }

        public Vector2 AroundAngle
        {
            get { return Util.ParseVector2RawString(AroundAngleString); }
        }

        
    }
}
