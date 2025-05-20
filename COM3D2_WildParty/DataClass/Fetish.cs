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
            public int OrgasmCount = 0;             //Requirement of how many times the men have orgasm with the maid. If it is a MMF situation, it will count twice each time orgasm occured.
            public int MaidOrgasmCount = 0;         //Maid version
            public int PositionOrgasmCount = 0;

            public List<int> SexPosRequired;        //If this is not null, the maid must have orgasm at least once for each sex pos in this list
        }
    }
}
