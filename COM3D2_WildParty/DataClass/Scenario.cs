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
        public List<string> ConditionInfoText;
        public List<int> Personality;
        public List<string> RequireFlag;
        public List<string> ExcludeFlag;

        public List<MapInfo> AllowMap;
        public List<string> ExtraYotogiCommands;
        public string ADVEntryStep;
        public bool CanGainSkillExp;
        public bool LockParameters;
        public bool UnlimitedMind;
        public bool IsGroupEvent;
        public MaidCountRequirement MaidCount;
        public List<ExtraScenarioFlag> SetScenarioFlag;

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

        

        internal class MapInfo
        {
            public int MapID;
            public List<string> FormationOption;
            public string DefaultFormation;
        }

        internal class MaidCountRequirement
        {
            public int Min;
            public int Max;
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
            if (RequireFlag == null) RequireFlag = new List<string>();
            if (ExcludeFlag == null) ExcludeFlag = new List<string>();

            Contract = new List<MaidStatus.Contract>();
            SexExperience = new List<MaidStatus.Seikeiken>();
            Relation = new List<MaidStatus.Relation>();

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
        }
    }
}
