using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

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
                return new Vector3(float.Parse(splitPos[0].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[1].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[2].Trim(), CultureInfo.InvariantCulture));
            }
        }
        public Quaternion Rot
        {
            get
            {
                var splitRot = RotString.Split(',');
                return new Quaternion(float.Parse(splitRot[0].Trim(), CultureInfo.InvariantCulture), float.Parse(splitRot[1].Trim(), CultureInfo.InvariantCulture), float.Parse(splitRot[2].Trim(), CultureInfo.InvariantCulture), float.Parse(splitRot[3].Trim(), CultureInfo.InvariantCulture));
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
                return new Vector3(float.Parse(splitPos[0].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[1].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[2].Trim(), CultureInfo.InvariantCulture));
            }
        }
        public Vector3 Rot
        {
            get
            {
                if (!string.IsNullOrEmpty(RotString))
                {
                    var splitRot = RotString.Split(',');
                    return new Vector3(float.Parse(splitRot[0].Trim(), CultureInfo.InvariantCulture), float.Parse(splitRot[1].Trim(), CultureInfo.InvariantCulture), float.Parse(splitRot[2].Trim(), CultureInfo.InvariantCulture));
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

    }
}
