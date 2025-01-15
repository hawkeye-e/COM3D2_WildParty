using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    internal class DebugViewData
    {
        private static ManualLogSource Log = WildParty.Log;

        public static void PrintAllPropensity()
        {
            var propen = MaidStatus.Propensity.GetAllDatas(false);
            foreach (var item in propen)
            {
                Log.LogInfo("Propensity id: " + item.id + ", uniqueName: " + item.uniqueName + ", drawName: " + item.drawName + ", termName: " + item.termName + ", colorType: " + item.colorType);
                if (item.bonusCorrection != null)
                    foreach (var kvp in item.bonusCorrection.dataPacks)
                        Log.LogInfo("Type: " + kvp.Key + ", value: " + kvp.Value);
            }
        }

        public static void PrintMaidFlags(string maidName)
        {
            Maid maid = GameMain.Instance.CharacterMgr.GetStockMaidList().Where(x => x.status.fullNameJpStyle.Contains(maidName)).FirstOrDefault();
            //Maid maid = GameMain.Instance.CharacterMgr.GetStockMaidList().First();
            if (maid != null)
            {
                foreach (var kvp in maid.status.propensitys)
                {
                    Log.LogInfo("propensitys Key: " + kvp.Key + ", Value: " + kvp.Value.id + ", " + kvp.Value.termName);
                }

                foreach (var kvp in maid.status.flags)
                {
                    Log.LogInfo("maid flags Key: " + kvp.Key + ", Value: " + kvp.Value);
                }
                foreach (var kvp in maid.status.eventEndFlags)
                {
                    Log.LogInfo("maid eventEndFlags Key: " + kvp.Key + ", Value: " + kvp.Value);
                }
            }
        }

        public static void ResetMarriageStatus(string maidName)
        {
            Maid maid = GameMain.Instance.CharacterMgr.GetStockMaidList().Where(x => x.status.fullNameJpStyle.Contains(maidName)).FirstOrDefault();
            if (maid != null)
            {
                //to reset the wedding event
                if (DebugHelper.DebugState.Instance.DebugOneOffResetMarriageFlag)
                {
                    DebugHelper.DebugState.Instance.DebugOneOffResetMarriageFlag = false;
                    maid.status.specialRelation = MaidStatus.SpecialRelation.Null; // = MaidStatus.Relation.Lover
                    maid.status.RemoveEventEndFlagAll();
                    //maid.status.RemoveFlagAll();
                }
            }
        }

        public static void PrintMasterFlags()
        {
            foreach (var kvp in GameMain.Instance.CharacterMgr.status.flags)
            {
                Log.LogInfo("master flags Key: " + kvp.Key + ", Value: " + kvp.Value);
            }
        }

        public static void PrintYotogiData()
        {
            foreach (var kvp in Schedule.ScheduleCSVData.YotogiData)
            {
                Log.LogInfo("ID: " + kvp.Value.id + ", name: " + kvp.Value.name);
                //foreach (var a in Schedule.ScheduleCSVData.YotogiData[kvp.Value.id].condFlag1)
                //{
                //    Log.LogInfo("condFlag1: " + a);
                //}
                //foreach (var a in Schedule.ScheduleCSVData.YotogiData[kvp.Value.id].condSeikeiken)
                //{
                //    Log.LogInfo("condSeikeiken: " + a);
                //}
            }
        }

        public static void PrintWorkData(int id)
        {
            foreach (var a in Schedule.ScheduleCSVData.WorkData[id].condContract)
            {
                Log.LogInfo("condContract: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.WorkData[id].condMaidClass)
            {
                Log.LogInfo("condMaidClass: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.WorkData[id].condPackage)
            {
                Log.LogInfo("condPackage: " + a);
            }
        }

        public static void PrintYotogiData(int id)
        {
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condSkill)
            {
                Log.LogInfo("condSkill: " + a.Key + ", value: " + a.Value);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condContract)
            {
                Log.LogInfo("condContract: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condMaidClass)
            {
                Log.LogInfo("condMaidClass: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condYotogiClass)
            {
                Log.LogInfo("condYotogiClass: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condPropensity)
            {
                Log.LogInfo("condPropensity: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condFlag0)
            {
                Log.LogInfo("condFlag0: " + a);
            }

            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condRelation)
            {
                Log.LogInfo("condRelation: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condAdditionalRelation)
            {
                Log.LogInfo("condAdditionalRelation: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condSeikeiken)
            {
                Log.LogInfo("condSeikeiken: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condSpecialRelation)
            {
                Log.LogInfo("condSpecialRelation: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condPackage)
            {
                Log.LogInfo("condPackage: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condManVisibleFlag1)
            {
                Log.LogInfo("condManVisibleFlag1: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condManFlag0)
            {
                Log.LogInfo("condManFlag0: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condManFlag1)
            {
                Log.LogInfo("condManFlag1: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condPersonal)
            {
                Log.LogInfo("condPersonal: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condFacilityID)
            {
                Log.LogInfo("condFacilityID: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condInfo)
            {
                Log.LogInfo("condInfo: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].condRelationOld)
            {
                Log.LogInfo("condRelationOld: " + a);
            }
            foreach (var a in Schedule.ScheduleCSVData.YotogiData[id].pairCondPersonal)
            {
                Log.LogInfo("pairCondPersonal: " + a);
            }
        }


        public static void PrintManModelOption()
        {
            var manEditManager = new PhotoManEditManager();
            foreach (var m in manEditManager.man_head_menu_list)
            {
                Log.LogInfo("File Name:" + m.m_strMenuFileName + ", RID: " + m.m_nMenuFileRID);
            }
            foreach (var m in manEditManager.man_body_menu_list)
            {
                Log.LogInfo("File Name:" + m.m_strMenuFileName + ", RID: " + m.m_nMenuFileRID);
            }
        }

        public static void ViewTagData(KagTagSupport tag_data)
        {
            if (tag_data != null)
            {
                Log.LogInfo($"tag_data.GetTagKey(): " + tag_data.GetTagKey());
                var list = tag_data.GetTagList();
                if (list != null)
                {
                    foreach (var kvp in list)
                    {
                        Log.LogInfo($"tag list key: " + kvp.Key + ", value: " + kvp.Value);
                    }
                }
            }
        }
    }
}
