using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class ManBodyInfo
    {
        public List<string> Head;
        public List<BodyInfo> Body;

        internal class BodyInfo
        {
            public string Clothed;
            public string Nude;
            public int Min;
            public int Max;
        }
    }

}
