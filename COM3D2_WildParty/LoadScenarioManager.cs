﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //For handling the scenario resources data so that the ModUseData class is not in a mess
    //If the scenario steps data are split into different files, need to update this class accordingly    
    class LoadScenarioManager
    {
        private static readonly string[] ScenarioOrgyPartyResList = { 
            ScenarioResources.ScenarioOrgyParty.OrgyADVSetup,
            ScenarioResources.ScenarioOrgyParty.OrgyADVIntro,
            ScenarioResources.ScenarioOrgyParty.OrgyADVWelcomeGuest,
            ScenarioResources.ScenarioOrgyParty.OrgyADVPostYotogi,

            ScenarioResources.ScenarioOrgyParty.OrgyADV_Muku,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Majime,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Rindere,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Pure,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Pride,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Cool,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Yandere,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Anesan,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Genki,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Sadist,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Silent,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Devilish,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Ladylike,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Secretary,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Sister,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Curtness,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Missy,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Childhood,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Friendly,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Dame,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Masochist,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Cunning,
            ScenarioResources.ScenarioOrgyParty.OrgyADV_Gyaru
        };

        private static readonly string[] ScenarioHaremKingResList = {
            ScenarioResources.ScenarioHaremKing.HaremKingADVIntro,
            ScenarioResources.ScenarioHaremKing.HaremKingADVPrivateRoom,
            ScenarioResources.ScenarioHaremKing.HaremKingADVPostYotogi,

            ScenarioResources.ScenarioHaremKing.HaremKingADV_Muku,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Majime,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Rindere,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Pure,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Cool,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Pride,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Yandere,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Anesan,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Genki,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Sadist,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Silent,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Devilish,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Ladylike,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Secretary,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Sister,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Curtness,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Missy,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Childhood,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Masochist,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Cunning,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Friendly,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Dame,
            ScenarioResources.ScenarioHaremKing.HaremKingADV_Gyaru,
        };

        public static Dictionary<int, Dictionary<string, ADVStep>> LoadScenario(int scenarioID)
        {
            Dictionary<int, Dictionary<string, ADVStep>> result = new Dictionary<int, Dictionary<string, ADVStep>>();

            if(scenarioID == ScenarioIDList.OrgyPartyScenarioID)
                result.Add(ScenarioIDList.OrgyPartyScenarioID, LoadOrgyPartyScenario());
            else if(scenarioID == ScenarioIDList.HaremKingScenarioID)
            result.Add(ScenarioIDList.HaremKingScenarioID, LoadHaremKingScenario());

            return result;
        }

        public static Dictionary<string, ADVStep> LoadOrgyPartyScenario()
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            for(int i=0; i< ScenarioOrgyPartyResList.Length; i++)
                LoadResourcesFile(stepData, ScenarioOrgyPartyResList[i]);


            return stepData;
        }

        public static Dictionary<string, ADVStep> LoadHaremKingScenario()
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            for (int i = 0; i < ScenarioHaremKingResList.Length; i++)
                LoadResourcesFile(stepData, ScenarioHaremKingResList[i]);

            return stepData;
        }

        private static void LoadResourcesFile(Dictionary<string, ADVStep> masterList,string res)
        {
            Dictionary<string, ADVStep> newData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ADVStep>>(res);
            foreach(var kvp in newData)
            {
                masterList.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
