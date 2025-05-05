using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    //Base command which will be added to YotogiCommandFactory
    internal class ExtraYotogiCommandData
    {
        public string Name;
        public string CommandCode;                          //For identify the function to be called
        public CommandType Type;
        public int FetishID;
        public List<ConditionCheck> ConditionCheckTexts;
        public List<ConstraintSetting> Constraint = null;   //If this list is null or empty, there is no restriction. Otherwise it contains the list of formation that allows to use this button.
        public OrgasmSettingInfo OrgasmSetting = null;
        public string ChainedActionCode = "";

        public class ConditionCheck
        {
            public string Field;
            public string DisplayText;
        }

        public class ConstraintSetting
        {
            public int EventID;
            public List<int> SexPosIDs;
        }

        public class OrgasmSettingInfo
        {
            public string Type;
            public int MinExcite;
            public int ExciteDecay;
        }

        public enum CommandType
        {
            Common,
            Fetish,
            Orgasm
        }
    }
}
