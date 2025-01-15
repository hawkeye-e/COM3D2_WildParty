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
        
        public static Dictionary<int, Dictionary<string, ADVStep>> LoadScenario()
        {
            Dictionary<int, Dictionary<string, ADVStep>> result = new Dictionary<int, Dictionary<string, ADVStep>>();
            result.Add(ScenarioIDList.OrgyPartyScenarioID, LoadOrgyPartyScenario());

            return result;
        }

        public static Dictionary<string, ADVStep> LoadOrgyPartyScenario()
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADVSetup);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADVIntro);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADVWelcomeGuest);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADVPostYotogi);

            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Muku);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Majime);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Rindere);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Pure);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Pride);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Cool);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Yandere);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Anesan);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Genki);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Sadist);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Silent);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Devilish);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Ladylike);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Secretary);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Sister);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Curtness);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Missy);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Childhood);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Friendly);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Dame);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Masochist);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Cunning);
            LoadResourcesFile(stepData, ModResources.ScenarioDetailResource.OrgyADV_Gyaru);

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
