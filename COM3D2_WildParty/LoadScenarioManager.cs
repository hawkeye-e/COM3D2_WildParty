using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //For handling the scenario resources data so that the ModUseData class is not in a mess
    //If the scenario steps data are split into different files, need to update this class accordingly    
    class LoadScenarioManager
    {
        
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

            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADVSetup);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADVIntro);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADVWelcomeGuest);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADVPostYotogi);

            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Muku);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Majime);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Rindere);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Pure);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Pride);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Cool);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Yandere);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Anesan);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Genki);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Sadist);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Silent);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Devilish);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Ladylike);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Secretary);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Sister);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Curtness);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Missy);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Childhood);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Friendly);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Dame);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Masochist);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Cunning);
            LoadResourcesFile(stepData, ScenarioResources.ScenarioOrgyParty.OrgyADV_Gyaru);

            return stepData;
        }

        public static Dictionary<string, ADVStep> LoadHaremKingScenario()
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            LoadResourcesFile(stepData, ScenarioResources.ScenarioHaremKing.HaremKingADVIntro);
            

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
