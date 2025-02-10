using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.DailyScreen
{
    internal class Patches
    {
        internal static void PrepareExtraCommandWindow()
        {
            //Prepare the yotogi command windows by reusing the existing main category window in the schedule screen.
            var mainCategoryWindow = Util.FindInActiveObjectByName(Constant.DefinedGameObjectNames.MainCategoryPanel);
            CustomGameObject.YotogiExtraCommandWindow cmdWindow = new CustomGameObject.YotogiExtraCommandWindow(mainCategoryWindow, false);
            StateManager.Instance.ExtraCommandWindowMasterCopy = cmdWindow;
        }

        internal static void InjectScheduleOptions()
        {
            if (StateManager.Instance.IsInjectScheduleOptionsFinish)
                return;


            //Inject the category
            foreach (var category in ModUseData.ScenarioCategoryList)
            {
                Schedule.ScheduleCSVData.TaskCategoryNameMap.Add(category.CategoryID, category.DisplayName);
            }

            //Inject the scenario item
            foreach (var scenario in ModUseData.ScenarioList)
            {
                //A part of the conditions below leave as empty list as it is not used by this mod at this moment.
                Schedule.ScheduleCSVData.Yotogi newScenario = new Schedule.ScheduleCSVData.Yotogi();
                newScenario.id = scenario.ScenarioID;
                newScenario.categoryID = scenario.CategoryID;
                newScenario.yotogiType = scenario.YotogiType;
                newScenario.condSalonGrade = scenario.CondSalonGrade;
                newScenario.name = scenario.DisplayName;
                newScenario.information = scenario.Information;
                newScenario.icon = scenario.Icon;
                newScenario.netorareFlag = scenario.IsNetorare;
                newScenario.isCommu = scenario.IsCommu;
                newScenario.mode = scenario.Mode;
                newScenario.type = scenario.Type;

                newScenario.condSkill = new Dictionary<int, int>();
                newScenario.condContract = scenario.Contract;

                newScenario.condMaidClass = new List<int>();
                newScenario.condYotogiClass = new List<int>();
                newScenario.condPropensity = new List<int>();           //性癖, need to be careful on this flag as the 3 cheap price classes dont have full access of all propensity type

                newScenario.condSeikeiken = scenario.SexExperience;

                newScenario.condFlag0 = scenario.ExcludeFlag;
                newScenario.condFlag1 = scenario.RequireFlag;
                newScenario.condRelation = scenario.Relation;

                newScenario.condAdditionalRelation = new List<MaidStatus.AdditionalRelation>();
                newScenario.condSpecialRelation = new List<MaidStatus.SpecialRelation>();
                newScenario.condPackage = new List<string>();
                newScenario.condManVisibleFlag1 = new List<string>();
                newScenario.condManFlag0 = new List<string>();
                newScenario.condManFlag1 = new List<string>();
                newScenario.condPersonal = new List<int>();
                newScenario.condInfo = scenario.ConditionInfoText;
                newScenario.condRelationOld = new List<MaidStatus.Old.Relation>();
                newScenario.pairCondPersonal = scenario.Personality;
                newScenario.condFacilityID = new List<List<int>>();

                Schedule.ScheduleCSVData.YotogiData.Add(scenario.ScenarioID, newScenario);

                //Add the id to NTR flag list so that the NTR blocking would recognise this item
                if (scenario.IsNetorare)
                    Schedule.ScheduleCSVData.NetorareFlag.Add(scenario.ScenarioID);
            }

            StateManager.Instance.IsInjectScheduleOptionsFinish = true;
        }

#if COM3D2_5
#if UNITY_2022_3
        internal static Texture2D LoadModScenarioIcon(string fileName)
        {
            if (Constant.ModIconNames.Contains(fileName))
            {
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                if (fileName == ModResources.ImageResources.OrgyPartyIconFileName)
                    ModResources.ImageResources.icon_orgy.Save(mStream, ModResources.ImageResources.icon_orgy.RawFormat);
                else if (fileName == ModResources.ImageResources.HaremKingIconFileName)
                    ModResources.ImageResources.icon_harem_king.Save(mStream, ModResources.ImageResources.icon_harem_king.RawFormat);

                Texture2D tex = new Texture2D(64, 64);
                ImageConversion.LoadImage(tex, mStream.ToArray());

                return tex;
            }


            return null;
        }
#endif
#endif

#if COM3D2
        internal static Texture2D LoadModScenarioIcon(string fileName)
        {
            if (Constant.ModIconNames.Contains(fileName))
            {
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                if (fileName == ModResources.ImageResources.OrgyPartyIconFileName)
                    ModResources.ImageResources.icon_orgy.Save(mStream, ModResources.ImageResources.icon_orgy.RawFormat);
                else if (fileName == ModResources.ImageResources.HaremKingIconFileName)
                    ModResources.ImageResources.icon_harem_king.Save(mStream, ModResources.ImageResources.icon_harem_king.RawFormat);

                Texture2D tex = new Texture2D(64, 64);
                tex.LoadImage(mStream.ToArray());

                return tex;
            }

            return null;
        }
#endif


        internal static void SpoofFileExistence(string fileName, ref bool isExist)
        {
            if (Constant.ModIconNames.Contains(fileName))
                isExist = true;
        }

        internal static void GetModDefinedFlagResult(string flagName, ref int result, MaidStatus.Status status)
        {
            if (flagName == Constant.ModUsedFlag.Maid.RequireCategorySwappingOrOrgy)
            {
                //Flag for checking if this maid has performed any swapping or orgy category yotogi
                result = Math.Max(status.GetFlag(Constant.GameDefinedFlag.Maid.YotogiCategorySwappingExecuteTimes), status.GetFlag(Constant.GameDefinedFlag.Maid.YotogiCategoryOrgyExecuteTimes));
            }
        }

        internal static bool CheckScheduleTaskValid()
        {
            List<int> moddedScenarioIDList = ModUseData.ScenarioList.Where(x => x.MaidCount != null).Select(x => x.ScenarioID).ToList();
            foreach (int id in moddedScenarioIDList)
            {
                var moddedScenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == id).First();

                //check for both daytime
                int maidCountDay = Core.CharacterHandling.GetMaidCountForEventID(id, true);
                int maidCountNight = Core.CharacterHandling.GetMaidCountForEventID(id, false);

                if (!IsScenarioMaidCountValid(id, maidCountDay) || !IsScenarioMaidCountValid(id, maidCountNight))
                {
                    string warningMessage = ModResources.TextResource.MaidCountRequirementWarning
                        .Replace(Constant.ResourcesTextReplaceLabel.EventName, moddedScenario.DisplayName)
                        .Replace(Constant.ResourcesTextReplaceLabel.MinMaidCount, moddedScenario.MaidCount.Min.ToString())
                        .Replace(Constant.ResourcesTextReplaceLabel.MaxMaidCount, moddedScenario.MaidCount.Max.ToString());
                    GameMain.Instance.SysDlg.Show(warningMessage, SystemDialog.TYPE.OK);
                    return false;
                }
            }

            return true;
        }

        internal static void ApplyCrossMarkIfInvalid(OnHoverTaskIcon icon)
        {
            var warningIconTraverse = Traverse.Create(icon);
            var yotogiData = warningIconTraverse.Field("yotogiData").GetValue<Schedule.ScheduleCSVData.Yotogi>();
            if (yotogiData != null)
            {
                var moddedScenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == yotogiData.id).FirstOrDefault();
                if (moddedScenario != null)
                {
                    if (moddedScenario.MaidCount != null)
                    {
                        bool isDayTime = warningIconTraverse.Field("time").GetValue<ScheduleMgr.ScheduleTime>() == ScheduleMgr.ScheduleTime.DayTime;
                        int maidCount = Core.CharacterHandling.GetMaidCountForEventID(moddedScenario.ScenarioID, isDayTime);

                        bool isOK = IsScenarioMaidCountValid(yotogiData.id, maidCount);

                        icon.WariningFacilty.gameObject.SetActive(!isOK);
                    }

                }

            }
        }

        private static bool IsScenarioMaidCountValid(int scenarioID, int maidCount)
        {
            if (maidCount <= 0)
                return true;

            var moddedScenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == scenarioID).First();
            return moddedScenario.MaidCount.Max >= maidCount && moddedScenario.MaidCount.Min <= maidCount;
        }
    }
}
