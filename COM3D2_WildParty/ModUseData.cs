﻿using System.Collections.Generic;
using System.Linq;

namespace COM3D2.WildParty.Plugin
{
    class ModUseData
    {
        //Outer Key: ScenarioID, Inner Key: StepID
        public static Dictionary<int, Dictionary<string, ADVStep>> ADVStepData;

        //Key: Group Type (MF, MMF, FFM)
        public static Dictionary<string, List<BackgroundGroupMotion.MotionItem>> BackgroundMotionList;
        //Key: personality id, Inner Key: Group Type
        public static Dictionary<int, Dictionary<string, List<PlayableSkill.SkillItem>>> ValidSkillList;
        //Key: personality id
        //public static Dictionary<int, List<PersonalityVoice.VoiceEntry>> PersonalityVoiceList;
        public static Dictionary<int, PersonalityVoice> PersonalityVoiceList;
        //Key: body part type, Value: file name
        public static Dictionary<string, List<string>> ManBodyPartList;
        //Level1 Key: Voice Type, Level2 Key: Excitement Level, 
        public static Dictionary<string, Dictionary<int, VoiceFace.VoiceFaceEntry>> VoiceFaceList;
        //Key: Formation ID ("Theatre_Scattered" etc)
        public static Dictionary<string, MapCoorindates> MapCoordinateList;
        //Key: MotionType
        public static Dictionary<string, MasturbationMotion> MasturbationMotionList;

        public static List<ScenarioCategory> ScenarioCategoryList;

        public static List<Scenario> ScenarioList;

        //Key: State ID
        public static Dictionary<string, SexState> SexStateList;

        //Key: Pattern ID
        public static Dictionary<string, SemenPattern> SemenPatternList;

        public static List<Fetish> FetishList;

        //Key: Command Data ID
        public static Dictionary<string, ExtraYotogiCommandData> ExtraYotogiCommandDataList;

        //Key: Formation ID
        public static Dictionary<string, PartyGroupSetup> PartyGroupSetupList;

        public ModUseData()
        {
        }

        //Read all the necessary data from resources files
        public static void Init()
        {
            ManBodyPartList = ReadManBodyPartCSVFile(ModResources.TextResource.ManBodyOptions);
            
            ScenarioCategoryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScenarioCategory>>(ModResources.TextResource.ModScenarioCategory);

            ScenarioList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Scenario>>(ModResources.TextResource.ModScenario);
            foreach (var item in ScenarioList)
                item.PostLoadProcess();

            MasturbationMotionList = MasturbationMotion.ReadCSVFile(ModResources.TextResource.MasturbationMotion);

            VoiceFaceList = VoiceFace.ReadCSVFile(ModResources.TextResource.SexPosFace);
            SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ModResources.TextResource.SexStateDescription);

            SemenPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SemenPattern>>(ModResources.TextResource.SemenPattern);
            foreach (var kvp in SemenPatternList)
            {
                kvp.Value.PostInitDataProcess();
            }

            FetishList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fetish>>(ModResources.TextResource.ModFetish);

            ExtraYotogiCommandDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ExtraYotogiCommandData>>(ModResources.TextResource.ExtraYotogiComands);
        }

        public static void InitDataForScenario(int scenarioID)
        {
            ResetModUseData();
            if (scenarioID == ScenarioIDList.OrgyPartyScenarioID)
                InitDataForOrgyParty();
            else if (scenarioID == ScenarioIDList.HaremKingScenarioID)
                InitDataForHaremKing();
        }

        private static void ResetModUseData()
        {
            ADVStepData = new Dictionary<int, Dictionary<string, ADVStep>>();
            BackgroundMotionList = new Dictionary<string, List<BackgroundGroupMotion.MotionItem>>();
            ValidSkillList = new Dictionary<int, Dictionary<string, List<PlayableSkill.SkillItem>>>();
            PersonalityVoiceList = new Dictionary<int, PersonalityVoice>();
            MapCoordinateList = new Dictionary<string, MapCoorindates>();
        }

        private static void InitDataForOrgyParty()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.OrgyPartyScenarioID);

            InitBackgroundMotionDictionary(ModResources.TextResource.SexPosList_OrgyParty, ModResources.TextResource.SexPosValidLabels_OrgyParty);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.TextResource.SexPosList_OrgyParty);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.TextResource.OrgyYotogiMapCoordinates);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.TextResource.PartyGroupSetup_OrgyParty);
        }

        private static void InitDataForHaremKing()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.HaremKingScenarioID);

            InitBackgroundMotionDictionary(ModResources.TextResource.SexPosList_HaremKing, ModResources.TextResource.SexPosValidLabels_HaremKing);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.TextResource.SexPosList_HaremKing);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.TextResource.HaremKingYotogiMapCoordinates);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.TextResource.PartyGroupSetup_HaremKing);
        }


        private static void InitAllVoiceDataFromCSV()
        {
            PersonalityVoiceList = new Dictionary<int, PersonalityVoice>();
            var dictOrgasmScream = PersonalityVoice.ReadOrgasmScreamCSVFile(ModResources.Voice.Voice_All_OrgasmScream);
            var dictOrgasmWait = PersonalityVoice.ReadVoiceEntryCSVFile(ModResources.Voice.Voice_All_OrgasmWait);
            var dictInsert = PersonalityVoice.ReadVoiceEntryCSVFile(ModResources.Voice.Voice_All_Insert);

            InitVoiceDataForPersonality(Constant.PersonalityType.Muku, ModResources.Voice.Voice_Muku, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Majime, ModResources.Voice.Voice_Majime, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Rindere, ModResources.Voice.Voice_Rindere, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Pure, ModResources.Voice.Voice_Pure, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Pride, ModResources.Voice.Voice_Pride, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Cool, ModResources.Voice.Voice_Cool, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Yandere, ModResources.Voice.Voice_Yandere, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Anesan, ModResources.Voice.Voice_Anesan, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Genki, ModResources.Voice.Voice_Genki, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Sadist, ModResources.Voice.Voice_Sadist, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Silent, ModResources.Voice.Voice_Silent, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Devilish, ModResources.Voice.Voice_Devilish, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Ladylike, ModResources.Voice.Voice_Ladylike, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Secretary, ModResources.Voice.Voice_Secretary, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Sister, ModResources.Voice.Voice_Sister, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Curtness, ModResources.Voice.Voice_Curtness, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Missy, ModResources.Voice.Voice_Missy, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Childhood, ModResources.Voice.Voice_Childhood, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Masochist, ModResources.Voice.Voice_Masochist, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Cunning, ModResources.Voice.Voice_Cunning, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Friendly, ModResources.Voice.Voice_Friendly, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Dame, ModResources.Voice.Voice_Dame, dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Gyaru, ModResources.Voice.Voice_Gyaru, dictOrgasmScream, dictOrgasmWait, dictInsert);

        }

        private static void InitVoiceDataForPersonality(int personalityID, string normalPlayResString, List<PersonalityVoice.OrgasmScreamEntry> screamFullList, 
            List<PersonalityVoice.VoiceEntry> orgasmWaitFullList, List<PersonalityVoice.VoiceEntry> insertFullList)
        {
            List<PersonalityVoice.OrgasmScreamEntry> screamByPersonality = screamFullList.Where(x => x.Personality == Util.GetPersonalityNameByValue(personalityID)).ToList();
            List<PersonalityVoice.VoiceEntry> orgasmWaitByPersonality = orgasmWaitFullList.Where(x => x.Personality == Util.GetPersonalityNameByValue(personalityID)).ToList();
            List<PersonalityVoice.VoiceEntry> InsertByPersonality = insertFullList.Where(x => x.Personality == Util.GetPersonalityNameByValue(personalityID)).ToList();

            PersonalityVoice voiceData = new PersonalityVoice();
            voiceData.NormalPlayVoice = PersonalityVoice.ReadVoiceEntryCSVFile(normalPlayResString);
            voiceData.OrgasmScream = screamByPersonality;
            voiceData.OrgasmWait = orgasmWaitByPersonality;
            voiceData.InsertVoice = InsertByPersonality;

            PersonalityVoiceList.Add(personalityID, voiceData);
        }

        private static void InitBackgroundMotionDictionary(string resSexPos, string resLabel)
        {
            BackgroundMotionList = BackgroundGroupMotion.ReadSexPosListCSVFile(resSexPos, resLabel);
            List<MotionSpecialLabel> lstAll = MotionSpecialLabel.ReadCSVFile(ModResources.TextResource.SexPosSpecialLabels);

            foreach(var kvp in BackgroundMotionList)
            {
                foreach(var motionItem in kvp.Value)
                {
                    var lstFiltered = lstAll.Where(x => x.SexPosID == motionItem.ID).ToList();
                    motionItem.SpecialLabels = lstFiltered;
                }
            }
        }

        private static Dictionary<string, List<string>> ReadManBodyPartCSVFile(string resFile)
        {

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            string[] csvMan = resFile.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvMan.Length; i++)
            {
                if (csvMan[i] == "") continue;      //in case empty row
                string[] rowData = csvMan[i].Split(',');

                if (!result.ContainsKey(rowData[0]))
                    result.Add(rowData[0], new List<string>());


                result[rowData[0]].Add(rowData[1]);
            }

            return result;
        }

    }
}
