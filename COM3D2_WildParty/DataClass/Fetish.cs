using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    //In the code Kiss named this as "Propensity"
    internal class Fetish
    {
        public int ID;
        public string DrawName;
        public string UniqueName;
        public MaidStatus.Propensity.Data.ColorType ColorType;

        public ConditionInfo Conditions;

        public class ConditionInfo
        {
            public int ManCount = 0;
            public int OrgasmCount = 0;
        }
    }
}
