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

        //Key: Chained Action Code
        public static Dictionary<string, CommandChainedAction> CommandChainedActionList;

        //Key: HardCode Motion Code
        public static Dictionary<string, HardCodeMotionSetup> HardCodeMotionSetupList;

        public static Dictionary<BodyWritingSetupInfo.BodyPart, List<BodyWritingSetupInfo>> BodyWritingBodyTextSetupList;
        public static Dictionary<BodyWritingSetupInfo.BodyPart, List<BodyWritingSetupInfo>> BodyWritingTallyCountSetupList;

        //Key: texture type
        public static Dictionary<string, List<BodyWritingTextureInfo>> BodyWritingTextureList;

        public static List<ScenarioManifest> ScenarioManifestList;
        //Key: Manifest type(Constant.ResourcesFileType), Value: file path
        public static Dictionary<string, string> CommonManifestList;
        //Key: Manifest type(Constant.SexStateRuleDefinition), Value: file path
        public static Dictionary<string, string> SexStateManifestList;
        //Key: Manifest type(Constant.ResourcesFileType), Value: file path
        public static Dictionary<string, string> VoiceManifestList;

        public ModUseData()
        {
        }

        //Read the necessary data from the declared manifest file
        public static void InitManifest()
        {
            ScenarioManifestList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScenarioManifest>>(ResourceLoader.LoadCompressedFile(Constant.ScenarioManifestFile));
            CommonManifestList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ResourceLoader.LoadCompressedFile(Constant.CommonManifestFile));
            SexStateManifestList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ResourceLoader.LoadCompressedFile(Constant.SexStateManifestFile));
            VoiceManifestList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ResourceLoader.LoadCompressedFile(Constant.VoiceManifestFile));
        }

        //Read all the necessary data from resources files
        public static void Init()
        {
            ManBodyInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ManBodyInfo>>(LoadCompressedCommonResources(Constant.ResourcesFileType.RandomizeManSetting));

            ScenarioCategoryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScenarioCategory>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ModScenarioCategory));

            ScenarioList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Scenario>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ModScenario));
            foreach (var item in ScenarioList)
                item.PostLoadProcess();

            MasturbationMotionList = MasturbationMotion.ReadCSVFile(LoadCompressedCommonResources(Constant.ResourcesFileType.MasturbationMotion));

            VoiceFaceList = VoiceFace.ReadCSVFile(LoadCompressedCommonResources(Constant.ResourcesFileType.SexPosFace));

            SemenPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SemenPattern>>(LoadCompressedCommonResources(Constant.ResourcesFileType.SemenPattern));
            foreach (var kvp in SemenPatternList)
            {
                kvp.Value.PostInitDataProcess();
            }

            FetishList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fetish>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ModFetish));

            ModNPCFemaleList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ModNPCFemale>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ModNPCFemale));
            ModNPCMaleList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ModNPCMale>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ModNPCMale));

            YotogiMiscSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<YotogiMiscSetup>>>(LoadCompressedCommonResources(Constant.ResourcesFileType.YotogiMiscHandling));

            ClothesSetList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ClothesSet>>(LoadCompressedCommonResources(Constant.ResourcesFileType.ClothesSet));

            CharacterEffectList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, CharacterEffect>>(LoadCompressedCommonResources(Constant.ResourcesFileType.CharacterEffect));

            IKRectifyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IKRectify>>(LoadCompressedCommonResources(Constant.ResourcesFileType.IKRectify));

            YotogiCommandDataOverrideList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<YotogiCommandDataOverride>>(LoadCompressedCommonResources(Constant.ResourcesFileType.YotogiCommandDataOverride));

            CommandChainedActionList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, CommandChainedAction>>(LoadCompressedCommonResources(Constant.ResourcesFileType.CommandChainedAction));

            HardCodeMotionSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, HardCodeMotionSetup>>(LoadCompressedCommonResources(Constant.ResourcesFileType.HardCodeMotionSetup));

            BodyWritingBodyTextSetupList = BodyWritingSetupInfo.ReadCSVFile(LoadCompressedCommonResources(Constant.ResourcesFileType.BodyWritingBodyTextSetup));

            BodyWritingTallyCountSetupList = BodyWritingSetupInfo.ReadCSVFile(LoadCompressedCommonResources(Constant.ResourcesFileType.BodyWritingTallyCountSetup));

            BodyWritingTextureList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<BodyWritingTextureInfo>>>(LoadCompressedCommonResources(Constant.ResourcesFileType.BodyWritingTextureFileList));
        }

        private static string LoadCompressedCommonResources(string ResourceFileTypeKey)
        {
            return ResourceLoader.LoadCompressedFile(CommonManifestList[ResourceFileTypeKey]);
        }

        public static void InitDataForScenario(int scenarioID)
        {
            ResetModUseData();

            ScenarioManifest scenarioManifestInfo = GetScenarioManifest(scenarioID);
            
            if (scenarioManifestInfo != null)
            {
                InitDataForScenarioWithRes(
                    scenarioID,
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioSexPosList].FirstOrDefault()),
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioSexPosValidLabels].FirstOrDefault()),
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioSexPosSpecialLabels].FirstOrDefault()),
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioMapCoordinates].FirstOrDefault()),
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioPartyGroupSetup].FirstOrDefault()),
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioExtraYotogiComands].FirstOrDefault())
                    );
            }

        }

        public static void ReloadCoordinateData(int scenarioID)
        {
            ScenarioManifest scenarioManifestInfo = GetScenarioManifest(scenarioID);

            if (scenarioManifestInfo != null)
            {   
                MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(
                    ResourceLoader.LoadCompressedFile(scenarioManifestInfo.Files[Constant.ResourcesFileType.ScenarioMapCoordinates].FirstOrDefault())
                    );
            }
        }

        private static void ResetModUseData()
        {
            ADVStepData = new Dictionary<int, Dictionary<string, ADVStep>>();
            BackgroundMotionList = new Dictionary<string, List<BackgroundGroupMotion.MotionItem>>();
            ValidSkillList = new Dictionary<int, Dictionary<string, List<PlayableSkill.SkillItem>>>();
            PersonalityVoiceList = new Dictionary<int, PersonalityVoice>();
            MapCoordinateList = new Dictionary<string, MapCoorindates>();
            ExtraYotogiCommandDataList = new Dictionary<string, ExtraYotogiCommandData>();
        }

        private static void InitDataForScenarioWithRes(int scenarioID, string sexPosList, string sexPosValieLabels, string sexPosSpecialLabels, string mapCoordinates, string partyGroupSetup, string extraYotogiCommands)
        {
            ADVStepData = LoadScenarioManager.LoadScenario(scenarioID);

            InitBackgroundMotionDictionary(sexPosList, sexPosValieLabels, sexPosSpecialLabels);
            ValidSkillList = PlayableSkill.ReadSexPosListCSVFile(sexPosList);
            
            InitAllVoiceDataFromCSV();
            MapCoordinateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MapCoorindates>>(mapCoordinates);
            
            PartyGroupSetupList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PartyGroupSetup>>(partyGroupSetup);
            
            ExtraYotogiCommandDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ExtraYotogiCommandData>>(extraYotogiCommands);
        }

        public static ScenarioManifest GetScenarioManifest(int scenarioID)
        {
            ScenarioManifest scenarioManifestInfo;
            if (scenarioID == ScenarioIDList.LilyBloomingParadiseScenarioID)
            {
                string strapOnKeyword = Config.LilyBloomingParadiseNoStrapOn ? "NoStrapOn" : "StrapOn";
                scenarioManifestInfo = ModUseData.ScenarioManifestList.Where(x => x.ID == scenarioID && x.SpecialFlag == strapOnKeyword).FirstOrDefault();
            }
            else
            {
                scenarioManifestInfo = ModUseData.ScenarioManifestList.Where(x => x.ID == scenarioID).FirstOrDefault();
            }

            return scenarioManifestInfo;
        }

        private static void InitAllVoiceDataFromCSV()
        {
            PersonalityVoiceList = new Dictionary<int, PersonalityVoice>();
            var dictOrgasmScream = PersonalityVoice.ReadOrgasmScreamCSVFile(ResourceLoader.LoadCompressedFile(VoiceManifestList[Constant.ResourcesFileType.VoiceAllOrgasmScream]));
            var dictOrgasmWait = PersonalityVoice.ReadVoiceEntryCSVFile(ResourceLoader.LoadCompressedFile(VoiceManifestList[Constant.ResourcesFileType.VoiceAllOrgasmWait]));
            var dictInsert = PersonalityVoice.ReadVoiceEntryCSVFile(ResourceLoader.LoadCompressedFile(VoiceManifestList[Constant.ResourcesFileType.VoiceAllInsert]));

            InitVoiceDataForPersonality(Constant.PersonalityType.Muku, GetVoiceManifestPerCharacter(Constant.PersonalityType.Muku), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Majime, GetVoiceManifestPerCharacter(Constant.PersonalityType.Majime), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Rindere, GetVoiceManifestPerCharacter(Constant.PersonalityType.Rindere), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Pure, GetVoiceManifestPerCharacter(Constant.PersonalityType.Pure), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Pride, GetVoiceManifestPerCharacter(Constant.PersonalityType.Pride), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Cool, GetVoiceManifestPerCharacter(Constant.PersonalityType.Cool), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Yandere, GetVoiceManifestPerCharacter(Constant.PersonalityType.Yandere), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Anesan, GetVoiceManifestPerCharacter(Constant.PersonalityType.Anesan), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Genki, GetVoiceManifestPerCharacter(Constant.PersonalityType.Genki), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Sadist, GetVoiceManifestPerCharacter(Constant.PersonalityType.Sadist), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Silent, GetVoiceManifestPerCharacter(Constant.PersonalityType.Silent), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Devilish, GetVoiceManifestPerCharacter(Constant.PersonalityType.Devilish), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Ladylike, GetVoiceManifestPerCharacter(Constant.PersonalityType.Ladylike), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Secretary, GetVoiceManifestPerCharacter(Constant.PersonalityType.Secretary), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Sister, GetVoiceManifestPerCharacter(Constant.PersonalityType.Sister), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Curtness, GetVoiceManifestPerCharacter(Constant.PersonalityType.Curtness), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Missy, GetVoiceManifestPerCharacter(Constant.PersonalityType.Missy), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Childhood, GetVoiceManifestPerCharacter(Constant.PersonalityType.Childhood), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Masochist, GetVoiceManifestPerCharacter(Constant.PersonalityType.Masochist), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Cunning, GetVoiceManifestPerCharacter(Constant.PersonalityType.Cunning), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Friendly, GetVoiceManifestPerCharacter(Constant.PersonalityType.Friendly), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Dame, GetVoiceManifestPerCharacter(Constant.PersonalityType.Dame), dictOrgasmScream, dictOrgasmWait, dictInsert);
            InitVoiceDataForPersonality(Constant.PersonalityType.Gyaru, GetVoiceManifestPerCharacter(Constant.PersonalityType.Gyaru), dictOrgasmScream, dictOrgasmWait, dictInsert);

        }

        private static string GetVoiceManifestPerCharacter(int personalityType)
        {
            string pName = Util.GetPersonalityNameByValue(personalityType);
            return ResourceLoader.LoadCompressedFile(VoiceManifestList[string.Format(Constant.ResourcesFileType.VoiceCharacter, pName)]);
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
            SexStateList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SexState>>(ResourceLoader.LoadCompressedFile(SexStateManifestList[ruleName]));
        }
    }
}
