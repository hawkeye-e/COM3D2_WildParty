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
        public List<CommandParameters> Parameters;              //Use in ChainedAction
        public TriggerConditionInfo TriggerConditions;    //If this is not null, the button will only be enabled if conditions are met

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

        public class CommandParameters
        {
            public string Name;             //Parameter name
            public object Value;                          //Need data conversion
        }

        public class SpecialParameterNames
        {
            public const string PrimaryGroup = "PrimaryGroup";
            public const string NextExtraMan_Group = "NextExtraMan_Group";
        }

        public class TriggerConditionInfo
        {
            public List<int> CurrentCommandIDs;      //This button is going to be enabled only if the ID of the most recent command clicked matches any member of this list
            public List<RequireCommandClickInfo> RequireCommandClicks;
            public List<string> Texts;               //Tooltip display
            public ExciteRequirementInfo ExciteSetting;

            public class RequireCommandClickInfo{
                public List<int> CommandIDs;
                public int Count;
            }

            public class ExciteRequirementInfo
            {
                public int MaxExcite = 300;         //The current excite need to stay below the Max value to show the button
                public int MinExcite = -100;        //The current excite need to stay above the Min value to show the button
            }
        }
    }
}
