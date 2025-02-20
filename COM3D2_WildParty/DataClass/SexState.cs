using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class SexState
    {
        public List<string> NextStates;

        internal static class StateType
        {
            internal const string Waiting = "Waiting";          
            internal const string Insert = "Insert";
            internal const string InsertEnd = "InsertEnd";
            internal const string Orgasm = "Orgasm";
            internal const string OrgasmEnd = "OrgasmEnd";
            internal const string OrgasmWait = "OrgasmWait";    //This is different from "Waiting". "Waiting" is the status when the group just change position and this one is after orgasm
            internal const string ChangePosition = "ChangePosition";
            internal const string NormalPlay = "NormalPlay";

            internal const string ChangeMan = "ChangeMan";
        }
    }
}
