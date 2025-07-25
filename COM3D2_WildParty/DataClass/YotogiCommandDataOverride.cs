using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class YotogiCommandDataOverride
    {
        public int ScenarioID;
        public List<OverrideData> Override;
        
        internal class OverrideData
        {
            public int SkillID;
            public int CommandID;
            public string DisplayName = "";
            public bool Enabled = true;
            public bool IgnoreDefaultTJSRequestScript = false;
        }


    }


}
