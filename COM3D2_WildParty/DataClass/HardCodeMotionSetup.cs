using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    
    internal class HardCodeMotionSetup
    {
        public string ID;
        public string Type;             //For reference only??
        public ManWalkSettingInfo ManWalkSetting;
        


        public class ManWalkSettingInfo
        {
            public string StandingAnimationFile;
            [JsonProperty]
            public string StandingMotionOffsetString;
            [JsonProperty]
            public string WalkingMotionOffsetString;
            [JsonProperty]
            public string RotationOffsetString;
            [JsonProperty]
            public string MaidMotionOffsetString;

            public Vector3 StandingMotionOffset
            {
                get { return Util.ParseVector3RawString(StandingMotionOffsetString); }
            }
            public Vector3 WalkingMotionOffset
            {
                get { return Util.ParseVector3RawString(WalkingMotionOffsetString); }
            }

            public Vector3 RotationOffset
            {
                get { return Util.ParseVector3RawString(RotationOffsetString); }
            }

            public Vector3 MaidMotionOffset
            {
                get { return Util.ParseVector3RawString(MaidMotionOffsetString); }
            }
        }


    }
}
