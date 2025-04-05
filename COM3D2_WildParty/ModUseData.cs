using System.Collections.Generic;
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
        //Key: body part type, Value: ManBodyPart
        public static Dictionary<string, ManBodyInfo> ManBodyInfoList;
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

        //Key: NPC ID
        public static Dictionary<string, ModNPCFemale> ModNPCFemaleList;

        //Key: NPC ID
        public static Dictionary<string, ModNPCMale> ModNPCMaleList;

        //Key: ScenarioID
        public static Dictionary<int, List<YotogiMiscSetup>> YotogiMiscSetupList;

        //Key: ClothesSetID
        public static Dictionary<string, ClothesSet> ClothesSetList;

        //Key: EffectID
        public static Dictionary<string, CharacterEffect> CharacterEffectList;

        public static List<IKRectify> IKRectifyList;

        public static List<YotogiCommandDataOverride> YotogiCommandDataOverrideList;

        public ModUseData()
        {
        }

        //Read all the necessary data from resources files
        public static void Init()
        {
            ManBodyInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ManBodyInfo>>(ModResources.TextResource.RandomizeManSetting);

            ScenarioCategoryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScenarioCategory>>(ModResources.TextResource.ModScenarioCategory);

            ScenarioList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Scenario>>(ModResources.TextResource.ModScenario);
            foreach (var item in ScenarioList)
                item.PostLoadProcess();

            MasturbationMotionList = MasturbationMotion.ReadCSVFile(ModResources.TextResource.MasturbationMotion);

            VoiceFaceList = VoiceFace.ReadCSVFile(ModResources.TextResource.SexPosFace);

            SemenPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SemenPattern>>(ModResources.TextResource.SemenPattern);
            foreach (var kvp in SemenPatternList)
            {
                kvp.Value.PostInitDataProcess();
            }

            FetishList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fetish>>(ModResources.TextResource.ModFetish);

            ExtraYotogiCommandDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ExtraYotogiCommandData>>(ModResources.TextResource.ExtraYotogiComands);

            ModNPCFemaleList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ModNPCFemale>>(ModResources.TextResource.ModNPCFemale);
            ModNPCMaleList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ModNPCMale>>(ModResources.TextResource.ModNPCMale);

            YotogiMiscSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<YotogiMiscSetup>>>(ModResources.TextResource.YotogiMiscHandling);

            ClothesSetList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ClothesSet>>(ModResources.TextResource.ClothesSet);

            CharacterEffectList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, CharacterEffect>>(ModResources.TextResource.CharacterEffect);

            IKRectifyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IKRectify>>(ModResources.TextResource.IKRectify);
            
            YotogiCommandDataOverrideList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<YotogiCommandDataOverride>>(ModResources.TextResource.YotogiCommandDataOverride);
        }

        public static void InitDataForScenario(int scenarioID)
        {
            ResetModUseData();
            if (scenarioID == ScenarioIDList.OrgyPartyScenarioID)
                InitDataForOrgyParty();
            else if (scenarioID == ScenarioIDList.HaremKingScenarioID)
                InitDataForHaremKing();
            else if (scenarioID == ScenarioIDList.HappyGBClubScenarioID)
                InitDataForHappyGBClub();
            else if (scenarioID == ScenarioIDList.AnotherGBDesireScenarioID)
                InitDataForAnotherGBDesire();
            else if (scenarioID == ScenarioIDList.LilyBloomingParadiseScenarioID)
                InitDataForLilyBloomingParadise();
        }

        public static void ReloadCoordinateData(int scenarioID)
        {
            MapCoordinateList = new Dictionary<string, MapCoorindates>();
            if (scenarioID == ScenarioIDList.OrgyPartyScenarioID)
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_Orgy);
            else if (scenarioID == ScenarioIDList.HaremKingScenarioID)
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_HaremKing);
            else if (scenarioID == ScenarioIDList.HappyGBClubScenarioID)
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_HappyGBClub);
            else if (scenarioID == ScenarioIDList.AnotherGBDesireScenarioID) 
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_AnotherGBDesire);
            else if (scenarioID == ScenarioIDList.LilyBloomingParadiseScenarioID)
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_LilyBloomingParadise);
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

            InitBackgroundMotionDictionary(ModResources.SexPosListResources.SexPosList_OrgyParty, ModResources.SexPosValidLabelsResources.SexPosValidLabels_OrgyParty, ModResources.SexPosSpecialLabelsResources.SexPosSpecialLabels_OrgyParty);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.SexPosListResources.SexPosList_OrgyParty);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_Orgy);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.PartyGroupSetupResources.PartyGroupSetup_OrgyParty);
        }

        private static void InitDataForHaremKing()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.HaremKingScenarioID);

            InitBackgroundMotionDictionary(ModResources.SexPosListResources.SexPosList_HaremKing, ModResources.SexPosValidLabelsResources.SexPosValidLabels_HaremKing, ModResources.SexPosSpecialLabelsResources.SexPosSpecialLabels_HaremKing);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.SexPosListResources.SexPosList_HaremKing);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_HaremKing);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.PartyGroupSetupResources.PartyGroupSetup_HaremKing);
        }

        private static void InitDataForHappyGBClub()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.HappyGBClubScenarioID);

            InitBackgroundMotionDictionary(ModResources.SexPosListResources.SexPosList_HappyGBClub, ModResources.SexPosValidLabelsResources.SexPosValidLabels_HappyGBClub, ModResources.SexPosSpecialLabelsResources.SexPosSpecialLabels_HappyGBClub);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.SexPosListResources.SexPosList_HappyGBClub);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_HappyGBClub);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.PartyGroupSetupResources.PartyGroupSetup_HappyGBClub);
        }

        //This is a scenario that reuse some of the resources of Happy GB Club.
        private static void InitDataForAnotherGBDesire()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.AnotherGBDesireScenarioID);
            
            InitBackgroundMotionDictionary(ModResources.SexPosListResources.SexPosList_HappyGBClub, ModResources.SexPosValidLabelsResources.SexPosValidLabels_HappyGBClub, ModResources.SexPosSpecialLabelsResources.SexPosSpecialLabels_HappyGBClub);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.SexPosListResources.SexPosList_HappyGBClub);
            
            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_AnotherGBDesire);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.PartyGroupSetupResources.PartyGroupSetup_AnotherGBDesire);
        }

        private static void InitDataForLilyBloomingParadise()
        {
            ADVStepData = LoadScenarioManager.LoadScenario(ScenarioIDList.LilyBloomingParadiseScenarioID);

            InitBackgroundMotionDictionary(ModResources.SexPosListResources.SexPosList_LilyBloomingParadise, ModResources.SexPosValidLabelsResources.SexPosValidLabels_LilyBloomingParadise, ModResources.SexPosSpecialLabelsResources.SexPosSpecialLabels_LilyBloomingParadise);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(ModResources.SexPosListResources.SexPosList_LilyBloomingParadise);

            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(ModResources.YotogiMapCoordinatesResources.MapCoordinates_LilyBloomingParadise);

            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(ModResources.PartyGroupSetupResources.PartyGroupSetup_LilyBloomingParadise);
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

        private static void InitBackgroundMotionDictionary(string resSexPos, string resLabel, string specialLabel)
        {
            //Common label
            BackgroundMotionList = BackgroundGroupMotion.ReadSexPosListCSVFile(resSexPos, resLabel);

            //Special label
            if (string.IsNullOrEmpty(specialLabel))
                return;

            List<MotionSpecialLabel> lstAll = MotionSpecialLabel.ReadCSVFile(specialLabel);

            foreach(var kvp in BackgroundMotionList)
            {
                foreach(var motionItem in kvp.Value)
                {
                    var lstFiltered = lstAll.Where(x => x.SexPosID == motionItem.ID).ToList();
                    motionItem.SpecialLabels = lstFiltered;
                }
            }
        }

        public static void LoadSexStateRule(string ruleName)
        {
            SexStateList = new Dictionary<string, SexState>();
            if(ruleName == Constant.SexStateRuleDefinition.GangBangNormal)
                SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ModResources.TextResource.SexStateDescription_GBType);
            else if (ruleName == Constant.SexStateRuleDefinition.GangBangQueued)
                SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ModResources.TextResource.SexStateDescription_GBQueuedType);
            else if (ruleName == Constant.SexStateRuleDefinition.Lesbian)
                SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ModResources.TextResource.SexStateDescription_LesbianType);
            else
                SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ModResources.TextResource.SexStateDescription);
        }
    }
}
