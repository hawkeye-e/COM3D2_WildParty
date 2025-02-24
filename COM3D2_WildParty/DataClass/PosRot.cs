using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    class PosRot
    {
        [JsonProperty]
        private string PosString;
        [JsonProperty]
        private string RotString;
        public Vector3 Pos
        {
            get
            {
                var splitPos = PosString.Split(',');
                return new Vector3(float.Parse(splitPos[0].Trim()), float.Parse(splitPos[1].Trim()), float.Parse(splitPos[2].Trim()));
            }
        }
        public Quaternion Rot
        {
            get
            {
                var splitRot = RotString.Split(',');
                return new Quaternion(float.Parse(splitRot[0].Trim()), float.Parse(splitRot[1].Trim()), float.Parse(splitRot[2].Trim()), float.Parse(splitRot[3].Trim()));
            }
        }

    }


    class PosRotVectorFormat
    {
        [JsonProperty]
        private string PosString;
        [JsonProperty]
        private string RotString;
        public Vector3 Pos
        {
            get
            {
                var splitPos = PosString.Split(',');
                return new Vector3(float.Parse(splitPos[0].Trim()), float.Parse(splitPos[1].Trim()), float.Parse(splitPos[2].Trim()));
            }
        }
        public Vector3 Rot
        {
            get
            {
                var splitRot = RotString.Split(',');
                return new Vector3(float.Parse(splitRot[0].Trim()), float.Parse(splitRot[1].Trim()), float.Parse(splitRot[2].Trim()));
            }
        }

    }
}
