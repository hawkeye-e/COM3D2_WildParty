using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;

namespace COM3D2.WildParty.Plugin
{
    class Config
    {
        private const string GENERAL = "1. General";
        private const string MOTIONSETTING = "2. H Motion Settings";
        private const string SCENARIOSETTING = "3. Scenario Related Setting";
        private const string YOTOGISETTING = "4. Yotogi Related Setting";
        private const string DEVELOPER = "5. Developer Used";

        internal static bool Enabled { get { return _enabled.Value; } }
        private static ConfigEntry<bool> _enabled;

        internal static bool DeveloperMode { get { return _developerMode.Value; } }
        private static ConfigEntry<bool> _developerMode;

        internal static bool DebugLogMotionData { get { return _debugLogMotionData.Value; } }
        private static ConfigEntry<bool> _debugLogMotionData;

        internal static bool DebugCaptureDialogues { get { return _debugCaptureDialogues.Value; } }
        private static ConfigEntry<bool> _debugCaptureDialogues;

        internal static bool DebugIgnoreADVForceTimeWait { get { return _debugIgnoreADVForceTimeWait.Value; } }
        private static ConfigEntry<bool> _debugIgnoreADVForceTimeWait;

        internal static bool DebugLogScriptInfo { get { return _debugLogScriptInfo.Value; } }
        private static ConfigEntry<bool> _debugLogScriptInfo;

        internal static bool DebugLogAnimationInfo { get { return _debugLogAnimationInfo.Value; } }
        private static ConfigEntry<bool> _debugLogAnimationInfo;

        internal static bool DebugLogBackgroundInfo { get { return _debugLogBackgroundInfo.Value; } }
        private static ConfigEntry<bool> _debugLogBackgroundInfo;

        internal static bool DebugLogBGMInfo { get { return _debugLogBGMInfo.Value; } }
        private static ConfigEntry<bool> _debugLogBGMInfo;

        internal static bool DebugLogSEInfo { get { return _debugLogSEInfo.Value; } }
        private static ConfigEntry<bool> _debugLogSEInfo;

        internal static bool DebugLogAudioInfo { get { return _debugLogAudioInfo.Value; } }
        private static ConfigEntry<bool> _debugLogAudioInfo;

        internal static bool DebugLogFaceAnimeInfo { get { return _debugLogFaceAnimeInfo.Value; } }
        private static ConfigEntry<bool> _debugLogFaceAnimeInfo;

        internal static int MaxInitialRandomExciteValue { get { return _maxInitialRandomExciteValue.Value; } }
        private static ConfigEntry<int> _maxInitialRandomExciteValue;

        internal static int MinInitialRandomExciteValue { get { return _minInitialRandomExciteValue.Value; } }
        private static ConfigEntry<int> _minInitialRandomExciteValue;


        internal static int MaxBackgroundGroupReviewTimeInSeconds { get { return _maxBackgroundGroupReviewTimeInSeconds.Value; } }
        private static ConfigEntry<int> _maxBackgroundGroupReviewTimeInSeconds;

        internal static int MinBackgroundGroupReviewTimeInSeconds { get { return _minBackgroundGroupReviewTimeInSeconds.Value; } }
        private static ConfigEntry<int> _minBackgroundGroupReviewTimeInSeconds;


        internal static int MaxExcitementRateIncrease { get { return _maxExcitementRateIncrease.Value; } }
        private static ConfigEntry<int> _maxExcitementRateIncrease;

        internal static int MinExcitementRateIncrease { get { return _minExcitementRateIncrease.Value; } }
        private static ConfigEntry<int> _minExcitementRateIncrease;


        internal static int MaxSensualRateIncrease { get { return _maxSensualRateIncrease.Value; } }
        private static ConfigEntry<int> _maxSensualRateIncrease;

        internal static int MinSensualRateIncrease { get { return _minSensualRateIncrease.Value; } }
        private static ConfigEntry<int> _minSensualRateIncrease;


        internal static int MaxSensualRateDecay { get { return _maxSensualRateDecay.Value; } }
        private static ConfigEntry<int> _maxSensualRateDecay;

        internal static int MinSensualRateDecay { get { return _minSensualRateDecay.Value; } }
        private static ConfigEntry<int> _minSensualRateDecay;


        internal static int MinOrgasmExcitementRate { get { return _minOrgasmExcitementRate.Value; } }
        private static ConfigEntry<int> _minOrgasmExcitementRate;

        internal static int BaseOrgasmChanceInPercentage { get { return _baseOrgasmChanceInPercentage.Value; } }
        private static ConfigEntry<int> _baseOrgasmChanceInPercentage;

        internal static int OrgasmChanceCapInPercentage { get { return _orgasmChanceCapInPercentage.Value; } }
        private static ConfigEntry<int> _orgasmChanceCapInPercentage;


        internal static int ChangeMotionRateInPercentage { get { return _changeMotionRateInPercentage.Value; } }
        private static ConfigEntry<int> _changeMotionRateInPercentage;

        internal static int MaxTimeToResumeSexAfterOrgasm { get { return _maxTimeToResumeSexAfterOrgasm.Value; } }
        private static ConfigEntry<int> _maxTimeToResumeSexAfterOrgasm;

        internal static bool IsTriggerConditionOn { get { return _isTriggerConditionOn.Value; } }
        private static ConfigEntry<bool> _isTriggerConditionOn;

        internal static ManTypeOption OrgyPartyManType { get { return _orgyPartyManType.Value; } }
        private static ConfigEntry<ManTypeOption> _orgyPartyManType;

        internal static ManTypeOption HappyGBClubManType { get { return _happyGBClubManType.Value; } }
        private static ConfigEntry<ManTypeOption> _happyGBClubManType;

        internal static ManTypeOption ImmoralVillageManType { get { return _immoralVillageManType.Value; } }
        private static ConfigEntry<ManTypeOption> _immoralVillageManType;

        internal static void Init(BaseUnityPlugin plugin)
        {
            AddGeneralConfigs(plugin);
            AddMotionSettingConfigs(plugin);
            AddScenarioRelatedConfigs(plugin);
            AddYotogiSettingConfigs(plugin);
            AddDeveloperRelatedConfigs(plugin);
        }

        private static void AddGeneralConfigs(BaseUnityPlugin plugin)
        {
            _enabled = plugin.Config.Bind(GENERAL, "Enable this plugin", true, "If false, this plugin will do nothing (requires game restart)");
        }

        private static void AddMotionSettingConfigs(BaseUnityPlugin plugin)
        {
            _maxInitialRandomExciteValue = plugin.Config.Bind(MOTIONSETTING, "Initial Excite Value Upper Limit", ConfigurableValue.YotogiSimulation.MaxInitialRandomExciteValue,
            new ConfigDescription("The upper limit value of initial excite value for a group.\n\n Initial excite value is a random number assigned when the group is newly created or the group has just reached orgasm.\n\n Must be larger than the corresponding Lower Limit value.", new AcceptableValueRange<int>(-100, 300))
            );

            _minInitialRandomExciteValue = plugin.Config.Bind(MOTIONSETTING, "Initial Excite Value Lower Limit", ConfigurableValue.YotogiSimulation.MinInitialRandomExciteValue,
            new ConfigDescription("The lower limit value of initial excite value for a group.\n\n Initial excite value is a random number assigned when the group is newly created or the group has just reached orgasm.\n\n Must be smaller than the corresponding Upper Limit value.", new AcceptableValueRange<int>(-100, 300))
            );


            _maxBackgroundGroupReviewTimeInSeconds = plugin.Config.Bind(MOTIONSETTING, "Background Group Motion Review Time Upper Limit", ConfigurableValue.YotogiSimulation.MaxBackgroundGroupReviewTimeInSeconds,
            new ConfigDescription("The upper limit value of motion review time for a group.\n\n Each non player controlled group will be assigned a review time to add the excitement rate, or change motion. A smaller review time would means more frequent update.\n\n Must be larger than the corresponding Lower Limit value.", new AcceptableValueRange<int>(5, 60))
            );

            _minBackgroundGroupReviewTimeInSeconds = plugin.Config.Bind(MOTIONSETTING, "Background Group Motion Review Time Lower Limit", ConfigurableValue.YotogiSimulation.MinBackgroundGroupReviewTimeInSeconds,
            new ConfigDescription("The lower limit value of motion review time for a group.\n\n Each non player controlled group will be assigned a review time to add the excitement rate, or change motion. A smaller review time would means more frequent update.\n\n Must be smaller than the corresponding Upper Limit value.", new AcceptableValueRange<int>(5, 60))
            );


            _maxExcitementRateIncrease = plugin.Config.Bind(MOTIONSETTING, "Excitement Rate Increase Upper Limit", ConfigurableValue.YotogiSimulation.MaxExcitementRateIncrease,
            new ConfigDescription("The upper limit value of excitement rate increase during a motion review.\n\n A higher value would means the groups would reach orgasm much faster.\n\n Must be larger than the corresponding Lower Limit value.", new AcceptableValueRange<int>(5, 100))
            );

            _minExcitementRateIncrease = plugin.Config.Bind(MOTIONSETTING, "Excitement Rate Increase Lower Limit", ConfigurableValue.YotogiSimulation.MinExcitementRateIncrease,
            new ConfigDescription("The lower limit value of excitement rate increase during a motion review.\n\n A higher value would means the groups would reach orgasm much faster.\n\n Must be smaller than the corresponding Upper Limit value.", new AcceptableValueRange<int>(5, 100))
            );


            _maxSensualRateIncrease = plugin.Config.Bind(MOTIONSETTING, "Sensual Rate Increase Upper Limit", ConfigurableValue.YotogiSimulation.MaxSensualRateIncrease,
            new ConfigDescription("The upper limit value of sensual rate increase during a motion review if the group is NOT in estrus mode.\n\n A higher value would means the groups would enter estrus mode much faster.\n\n Must be larger than the corresponding Lower Limit value.", new AcceptableValueRange<int>(5, 100))
            );

            _minSensualRateIncrease = plugin.Config.Bind(MOTIONSETTING, "Sensual Rate Increase Lower Limit", ConfigurableValue.YotogiSimulation.MinSensualRateIncrease,
            new ConfigDescription("The lower limit value of sensual rate increase during a motion review if the group is NOT in estrus mode.\n\n A higher value would means the groups would enter estrus mode much faster.\n\n Must be smaller than the corresponding Upper Limit value.", new AcceptableValueRange<int>(5, 100))
            );


            _maxSensualRateDecay = plugin.Config.Bind(MOTIONSETTING, "Sensual Rate Decay Upper Limit", ConfigurableValue.YotogiSimulation.MaxSensualRateDecay,
            new ConfigDescription("The upper limit value of sensual rate decay during a motion review if the group IS in estrus mode.\n\n A higher value would means the groups would exit estrus mode much faster.\n\n Must be larger than the corresponding Lower Limit value.", new AcceptableValueRange<int>(5, 100))
            );

            _minSensualRateDecay = plugin.Config.Bind(MOTIONSETTING, "Sensual Rate Decay Lower Limit", ConfigurableValue.YotogiSimulation.MinSensualRateDecay,
            new ConfigDescription("The lower limit value of sensual rate decay during a motion review if the group IS in estrus mode.\n\n A higher value would means the groups would exit estrus mode much faster.\n\n Must be smaller than the corresponding Upper Limit value.", new AcceptableValueRange<int>(5, 100))
            );


            _minOrgasmExcitementRate = plugin.Config.Bind(MOTIONSETTING, "Minimum Excitement Rate Required for Orgasm", ConfigurableValue.YotogiSimulation.MinOrgasmExcitementRate,
            new ConfigDescription("The minimum excitement value for a group to have a chance to trigger orgasm during a motion review.\n\n For example if this value is set to 200, every group having 200+ excitement value would have a certain percentage to reach orgasm during a motion review.", new AcceptableValueRange<int>(80, 300))
            );

            _baseOrgasmChanceInPercentage = plugin.Config.Bind(MOTIONSETTING, "Base Orgasm Chance in %", ConfigurableValue.YotogiSimulation.BaseOrgasmChanceInPercentage,
            new ConfigDescription("The base chance for a group to trigger orgasm during a motion review.\n\n If the group has reached the minimum excitement rate for orgasm value, the group would have this base chance % to orgasm, and the overall percentage increases as the excitement level keep increasing. ", new AcceptableValueRange<int>(0, 100))
            );

            _orgasmChanceCapInPercentage = plugin.Config.Bind(MOTIONSETTING, "Max Orgasm Chance in %", ConfigurableValue.YotogiSimulation.OrgasmChanceCapInPercentage,
            new ConfigDescription("The maximum chance for a group to trigger orgasm during a motion review.\n\n When a group reach excitement rate 300, this value would be the final percentage value of triggering orgasm.", new AcceptableValueRange<int>(0, 100))
            );


            _changeMotionRateInPercentage = plugin.Config.Bind(MOTIONSETTING, "Change Motion Rate in %", ConfigurableValue.YotogiSimulation.ChangeMotionRateInPercentage,
            new ConfigDescription("The chance for a group to change motion within the same sex position during a motion review.", new AcceptableValueRange<int>(0, 100))
            );

            _maxTimeToResumeSexAfterOrgasm = plugin.Config.Bind(MOTIONSETTING, "Maximum time in second to resume Sex after orgasm", ConfigurableValue.YotogiSimulation.MaxTimeToResumeSexAfterOrgasm,
            new ConfigDescription("Number of second that a group could have to rest after reaching orgasm.", new AcceptableValueRange<int>(0, 60))
            );
        }

        private static void AddScenarioRelatedConfigs(BaseUnityPlugin plugin)
        {
            _orgyPartyManType = plugin.Config.Bind(SCENARIOSETTING, "Empire Club Thanksgiving Event - Man Types", 
                ManTypeOption.YoungMan | ManTypeOption.Yanki | ManTypeOption.MiddleAged | ManTypeOption.Otaku | ManTypeOption.Shota,
                "The type of man that will be used in the scenaio Empire Club Thanksgiving event. If all items are unchecked, the system will use the default man types.");

            _happyGBClubManType = plugin.Config.Bind(SCENARIOSETTING, "Happy Gang Bang Club Event - Man Types",
                ManTypeOption.YoungMan | ManTypeOption.Yanki | ManTypeOption.MiddleAged | ManTypeOption.Otaku,
                "The type of man that will be used in the scenaio [Happy Gang Bang Club] and [Another Gang Bang Desire] event. If all items are unchecked, the system will use the default man types.");

            _immoralVillageManType = plugin.Config.Bind(SCENARIOSETTING, "Summer Festival of Immoral Village Event - Man Types",
                ManTypeOption.Shota,
                "The type of man that will be used in the scenaio [Summer Festival of Immoral Village] event. If all items are unchecked, the system will use the default man types.");
        }

        private static void AddDeveloperRelatedConfigs(BaseUnityPlugin plugin)
        {
            _developerMode = plugin.Config.Bind(DEVELOPER, "Developer Mode", false, "Leave this unchecked if you have no idea what it is (requires game restart)");

            _debugLogMotionData = plugin.Config.Bind(DEVELOPER, "Log Motion Data", false, "Leave this unchecked if you have no idea what it is");

            _debugCaptureDialogues = plugin.Config.Bind(DEVELOPER, "Log All Dialogues", false, "Leave this unchecked if you have no idea what it is");

            _debugIgnoreADVForceTimeWait = plugin.Config.Bind(DEVELOPER, "Ignore ADV Time Wait Setting", false, "Skip all those time wait setting in ADV to speed up the debug process. Leave this unchecked if you have no idea what it is");

            _debugLogScriptInfo = plugin.Config.Bind(DEVELOPER, "Log Load Script Info", false, "Log the script info whenever it is loaded in the game. Leave this unchecked if you have no idea what it is");

            _debugLogAnimationInfo = plugin.Config.Bind(DEVELOPER, "Log Load Animation Info", false, "Log the animation info whenever an animation file is loaded for a object in the game. Leave this unchecked if you have no idea what it is");

            _debugLogAudioInfo = plugin.Config.Bind(DEVELOPER, "Log Load Audio Info", false, "Log the audio info whenever an audio file is loaded for a maid in the game. Leave this unchecked if you have no idea what it is");

            _debugLogFaceAnimeInfo = plugin.Config.Bind(DEVELOPER, "Log Load Face Anime Info", false, "Log the face anime info whenever the facial expression changed for a maid in the game. Leave this unchecked if you have no idea what it is");

            _debugLogBackgroundInfo = plugin.Config.Bind(DEVELOPER, "Log Load Background Info", false, "Log the background info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");

            _debugLogBGMInfo = plugin.Config.Bind(DEVELOPER, "Log Load BGM Info", false, "Log the BGM info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");

            _debugLogSEInfo = plugin.Config.Bind(DEVELOPER, "Log Load SE Info", false, "Log the sound effect info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");
        }

        private static void AddYotogiSettingConfigs(BaseUnityPlugin plugin)
        {
            _isTriggerConditionOn = plugin.Config.Bind(YOTOGISETTING, "Command Trigger Condition", true, "On: The player is required to do achieve some small goals in order to click certain command buttons in the yotogi scene. \n Off: The trigger conditions are ignored and the player can click the command buttons freely.");
        }


        //Refer to RandomizeManSetting.json
        [Flags]
        internal enum ManTypeOption
        {
            Default     = 0,
            YoungMan    = 1 << 0,
            Yanki       = 1 << 1,
            MiddleAged  = 1 << 2,
            Otaku       = 1 << 3,
            Shota       = 1 << 4,
        }
    }
}
