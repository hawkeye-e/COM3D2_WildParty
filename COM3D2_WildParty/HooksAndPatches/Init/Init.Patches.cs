using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.Init
{
    internal class Patches
    {
        internal static bool IsCreatePropensityData(int id)
        {
            Fetish newItem = ModUseData.FetishList.Where(x => x.ID == id).FirstOrDefault();
            if (newItem != null)
            {
                //If the incoming ID is an ID we try to add, simply do nothing. We just want an empty object and do not need the system to attempt to read csv
                return false;
            }

            return true;
        }

        //Inject our mod defined fetish into the game
        //Those field names are unlikely to be reused elsewhere so just leave it here
        internal static void InjectCustomFetish()
        {
            if (!StateManager.Instance.IsInjectPropensityFinish)
            {
                var commonIdManager = Traverse.Create(typeof(MaidStatus.Propensity)).Field("commonIdManager").GetValue<wf.CsvCommonIdManager>();

                var propensityList = Traverse.Create(typeof(MaidStatus.Propensity)).Field("basicDatas").GetValue<Dictionary<int, MaidStatus.Propensity.Data>>();

                foreach (var dataToInject in ModUseData.FetishList)
                {
                    //All 3 lists need to be altered in order to make the game recognise the fetish record
                    commonIdManager.enabledIdList.Add(dataToInject.ID);
                    commonIdManager.idMap.Add(dataToInject.ID, new KeyValuePair<string, string>(dataToInject.DrawName, dataToInject.DrawName));

                    MaidStatus.Propensity.Data newItem = new MaidStatus.Propensity.Data(dataToInject.ID, null, null);
                    Traverse newItemReflection = Traverse.Create(newItem);
                    newItemReflection.Field("id").SetValue(dataToInject.ID);
                    newItemReflection.Field("drawName").SetValue(dataToInject.DrawName);
                    newItemReflection.Field("uniqueName").SetValue(dataToInject.UniqueName);
                    newItemReflection.Field("colorType").SetValue(dataToInject.ColorType);

                    propensityList.Add(dataToInject.ID, newItem);
                }


                StateManager.Instance.IsInjectPropensityFinish = true;
            }
        }
    }
}
