using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class Scenario
    {
        public int ScenarioID;
        public int CategoryID;
        public string DisplayName;
        public Schedule.ScheduleCSVData.YotogiType YotogiType;
        public int CondSalonGrade;
        public string Information;
        public string Icon;
        public bool IsNetorare;
        public bool IsCommu;
        public Schedule.ScheduleCSVData.ScheduleBase.Mode Mode;
        public ScheduleTaskCtrl.TaskType Type;
        public List<MaidStatus.Contract> Contract;
        public List<MaidStatus.Seikeiken> SexExperience;
        public List<MaidStatus.Relation> Relation;
        public List<MaidStatus.SpecialRelation> SpecialRelation;
        public List<string> ConditionInfoText;
        public List<int> Personality;
        public List<string> MaidRequireFlag;
        public List<string> MaidExcludeFlag;
        public List<string> MasterRequireFlag;
        public List<string> MasterExcludeFlag;

        
        public List<YotogiSetupInfo> YotogiSetup;           //A list to allow multiple yotogi scene in a single scenario
        public string ADVEntryStep;
        public bool CanGainSkillExp;
        public bool LockParameters;
        public bool UnlimitedMind;
        public bool IgnoreEjaculationSE = false;
        public bool IsGroupEvent;
        public MaidCountRequirement MaidCount;
        public List<ExtraScenarioFlag> SetScenarioFlag;
        public List<ExtraScenarioFlag> SetMaidFlag;

        [JsonProperty]
        private string YotogiTypeEnumString;
        [JsonProperty]
        private string ModeEnumString;
        [JsonProperty]
        private string TypeEnumString;
        [JsonProperty]
        private List<string> ContractEnumString;
        [JsonProperty]
        private List<string> SexExperienceEnumString;
        [JsonProperty]
        private List<string> RelationEnumString;
        [JsonProperty]
        private List<string> SpecialRelationEnumString;

        internal class YotogiSetupInfo
        {
            public int Phase = 1;
            public List<MapInfo> AllowMap;
            public DefaultMapInfo DefaultMap;
            public List<string> ExtraYotogiCommands;
            
            public bool FlexibleManCountInYotogi = false;
            public bool ForceChangeManWhenOrgasm = false;
            public bool IsMainManOwner;

            public string SexStateRule;
        }

        internal class MapInfo
        {
            public int MapID;
            public List<string> FormationOption;
            public string DefaultFormation;
        }

        internal class DefaultMapInfo
        {
            public string DayMapID;
            public string NightMapID;
            public string BGM;
            public List<string> FormationOption;
            public string DefaultFormation;
        }

        internal class MaidCountRequirement
        {
            public int Min;
            public int Max;
            public bool IsEven = false;
        }

        internal class ExtraScenarioFlag
        {
            public string ID;
            public int value;
        }

        public void PostLoadProcess()
        {
            if (ConditionInfoText == null) ConditionInfoText = new List<string>();
            if (Personality == null) Personality = new List<int>();
            if (MaidRequireFlag == null) MaidRequireFlag = new List<string>();
            if (MaidExcludeFlag == null) MaidExcludeFlag = new List<string>();
            if (MasterRequireFlag == null) MasterRequireFlag = new List<string>();
            if (MasterExcludeFlag == null) MasterExcludeFlag = new List<string>();

            Contract = new List<MaidStatus.Contract>();
            SexExperience = new List<MaidStatus.Seikeiken>();
            Relation = new List<MaidStatus.Relation>();
            SpecialRelation = new List<MaidStatus.SpecialRelation>();

            //convert all the enum string to enum
            YotogiType = (Schedule.ScheduleCSVData.YotogiType)Enum.Parse(typeof(Schedule.ScheduleCSVData.YotogiType), YotogiTypeEnumString, true);
            Mode = (Schedule.ScheduleCSVData.ScheduleBase.Mode)Enum.Parse(typeof(Schedule.ScheduleCSVData.ScheduleBase.Mode), ModeEnumString, true);
            Type = (ScheduleTaskCtrl.TaskType)Enum.Parse(typeof(ScheduleTaskCtrl.TaskType), TypeEnumString, true);

            if(ContractEnumString != null)
                foreach(string s in ContractEnumString)
                    Contract.Add((MaidStatus.Contract)Enum.Parse(typeof(MaidStatus.Contract), s, true));


            if (SexExperienceEnumString != null)
                foreach (string s in SexExperienceEnumString)
                    SexExperience.Add((MaidStatus.Seikeiken)Enum.Parse(typeof(MaidStatus.Seikeiken), s, true));


            if (RelationEnumString != null)
                foreach (string s in RelationEnumString)
                    Relation.Add((MaidStatus.Relation)Enum.Parse(typeof(MaidStatus.Relation), s, true));

            if (SpecialRelationEnumString != null)
                foreach (string s in SpecialRelationEnumString)
                    SpecialRelation.Add((MaidStatus.SpecialRelation)Enum.Parse(typeof(MaidStatus.SpecialRelation), s, true));
            
        }
    }
}
