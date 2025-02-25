using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class ModNPCFemale : ModNPC
    {
        public string PresetFile;
        public int Personality;
        public int VoicePitch;
    }

    internal class ModNPCMale : ModNPC
    {
        public string Head;
        public string Clothed;
        public string Nude;
        public int BodySize;        //Range: [0-100]
        public string Color;        //RGB separated by comma
    }

    internal class ModNPC
    {
        public string FirstName;
        public string LastName;
        public string NickName;
        public CallType WayToCall;

        public enum CallType
        {
            FirstName,
            LastName,
            NickName
        }
    }
}
