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

            ScenarioManifest scenarioManifestInfo = ModUseData.GetScenarioManifest(scenarioID);

            
            if (scenarioManifestInfo != null) {
                result.Add(scenarioID, LoadScenarioFromCompressedResources(scenarioManifestInfo.Files[Constant.ResourcesFileType.Scenario]));
            }
            
            return result;
        }

        public static Dictionary<string, ADVStep> LoadScenarioFromCompressedResources(List<string> resArray)
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            foreach(string res in resArray) 
            {
                string resJson = ResourceLoader.LoadCompressedFile(res);
                LoadResourcesFile(stepData, resJson);
            }

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
