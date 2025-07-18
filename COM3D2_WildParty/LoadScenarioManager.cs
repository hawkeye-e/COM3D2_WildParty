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

        private static readonly string[] ScenarioHappyGBClubResList = {
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVIntro,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVIntroLakeside,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVIntroCafe,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVIntroPoleDanceStage,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVPhase2,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVPhase3,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADVPostYotogi,

            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Muku,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Majime,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Rindere,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Pure,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Cool,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Pride,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Yandere,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Anesan,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Genki,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Sadist,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Silent,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Devilish,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Ladylike,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Secretary,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Sister,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Curtness,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Missy,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Childhood,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Friendly,
            ScenarioResources.ScenarioHappyGBClub.HappyGBClubADV_Dame,

        };

        private static readonly string[] ScenarioAnotherGBDesireResList = {
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADVIntro,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADVPhase2,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADVPhase3,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADVPostYotogi,

            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Muku,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Majime,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Rindere,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Pure,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Cool,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Pride,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Yandere,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Anesan,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Genki,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Sadist,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Silent,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Devilish,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Ladylike,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Secretary,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Sister,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Curtness,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Missy,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Childhood,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Friendly,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Dame,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Masochist,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Cunning,
            ScenarioResources.ScenarioAnotherGBDesire.AnotherGBDesireADV_Gyaru,
        };

        private static readonly string[] ScenarioLilyBloomingParadiseResList = {
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVIntro,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPhase1Hotel,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPhase1Bedroom,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPhase2,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPostYotogi,

            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Muku,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Majime,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Rindere,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Pure,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Cool,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Pride,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Yandere,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Anesan,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Genki,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Sadist,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Silent,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Devilish,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Ladylike,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Secretary,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Sister,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Curtness,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Missy,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Childhood,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Friendly,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Dame,
        };

        private static readonly string[] ScenarioLilyBloomingParadiseNoStrapOnResList = {
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVIntro,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPhase1Hotel,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPhase1Bedroom_SkipPhase2,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADVPostYotogi_NoStrapOn,

            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Muku,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Majime,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Rindere,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Pure,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Cool,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Pride,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Yandere,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Anesan,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Genki,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Sadist,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Silent,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Devilish,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Ladylike,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Secretary,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Sister,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Curtness,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Missy,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Childhood,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Friendly,
            ScenarioResources.ScenarioLilyBloomingParadise.LilyBloomingParadiseADV_Dame,
        };

        private static readonly string[] ScenarioImmoralVillageResList = {
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADVIntroDayTime,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADVIntroNightTime,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADVPhase1,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADVPhase2,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADVPostYotogi,

            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Muku,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Majime,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Rindere,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Pure,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Cool,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Pride,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Yandere,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Anesan,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Genki,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Sadist,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Silent,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Devilish,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Ladylike,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Secretary,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Sister,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Curtness,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Missy,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Childhood,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Friendly,
            ScenarioResources.ScenarioImmoralVillage.ImmoralVillageADV_Dame,
        };

        private static readonly string[] ScenarioLustfulMaidResList = {
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADVIntro,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADVPostYotogi,

            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Muku,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Majime,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Rindere,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Pure,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Cool,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Pride,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Yandere,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Anesan,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Genki,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Sadist,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Silent,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Devilish,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Ladylike,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Secretary,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Sister,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Curtness,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Missy,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Childhood,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Friendly,
            ScenarioResources.ScenarioLustfulMaid.LustfulMaidADV_Dame,
        };


        public static Dictionary<int, Dictionary<string, ADVStep>> LoadScenario(int scenarioID)
        {
            Dictionary<int, Dictionary<string, ADVStep>> result = new Dictionary<int, Dictionary<string, ADVStep>>();

            if (scenarioID == ScenarioIDList.OrgyPartyScenarioID)
                result.Add(ScenarioIDList.OrgyPartyScenarioID, LoadScenarioFromResources(ScenarioOrgyPartyResList));
            else if (scenarioID == ScenarioIDList.HaremKingScenarioID)
                result.Add(ScenarioIDList.HaremKingScenarioID, LoadScenarioFromResources(ScenarioHaremKingResList));
            else if (scenarioID == ScenarioIDList.HappyGBClubScenarioID)
                result.Add(ScenarioIDList.HappyGBClubScenarioID, LoadScenarioFromResources(ScenarioHappyGBClubResList));
            else if (scenarioID == ScenarioIDList.AnotherGBDesireScenarioID)
                result.Add(ScenarioIDList.AnotherGBDesireScenarioID, LoadScenarioFromResources(ScenarioAnotherGBDesireResList));
            else if (scenarioID == ScenarioIDList.LilyBloomingParadiseScenarioID)
            {
                if (Config.LilyBloomingParadiseNoStrapOn)
                    result.Add(ScenarioIDList.LilyBloomingParadiseScenarioID, LoadScenarioFromResources(ScenarioLilyBloomingParadiseNoStrapOnResList));
                else
                    result.Add(ScenarioIDList.LilyBloomingParadiseScenarioID, LoadScenarioFromResources(ScenarioLilyBloomingParadiseResList));
            }
            else if (scenarioID == ScenarioIDList.ImmoralVillageScenarioID)
                result.Add(ScenarioIDList.ImmoralVillageScenarioID, LoadScenarioFromResources(ScenarioImmoralVillageResList));
            else if (scenarioID == ScenarioIDList.LustfulMaidScenarioID)
                result.Add(ScenarioIDList.LustfulMaidScenarioID, LoadScenarioFromResources(ScenarioLustfulMaidResList));

            return result;
        }

        public static Dictionary<string, ADVStep> LoadScenarioFromResources(string[] resArray)
        {
            Dictionary<string, ADVStep> stepData = new Dictionary<string, ADVStep>();

            for (int i = 0; i < resArray.Length; i++)
                LoadResourcesFile(stepData, resArray[i]);

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
