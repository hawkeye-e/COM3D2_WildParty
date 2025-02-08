using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    internal class Constant
    {
        internal const int ScheduleSlotsCount = 40;
        internal const int PersonalityCount = 23;
        internal static readonly int[] EXPackPersonality = { PersonalityType.Masochist, PersonalityType.Cunning, PersonalityType.Gyaru };
        internal const int ExcitementRateMaxCap = 300;
        internal const int SensualRateMaxCap = 300;
        internal const int YotogiExcitementLevelStep = 100;

        internal static readonly string[] FellatioLabel = { "Fellatio", "DeepThroat", "PainFellatio", "FellatioExcite", "FellatioDrunk" };

        internal const string MainEventScriptFile = "AllMain.ks";

        internal static readonly string[] ModIconNames = { ModResources.ImageResources.OrgyPartyIconFileName, ModResources.ImageResources.HaremKingIconFileName };

        internal enum CharacterType
        {
            Maid,
            Man,
            NPC
        }

        //Name follows the dlc code
        internal static class PersonalityType
        {
            internal const int Muku = 80;       //無垢
            internal const int Majime = 90;     //真面目
            internal const int Rindere = 100;   //凛デレ
            internal const int Pure = 10;       //純真
            internal const int Pride = 30;      //ツンデレ
            internal const int Cool = 20;       //クーデレ
            internal const int Yandere = 40;    //ヤンデレ
            internal const int Anesan = 50;     //お姉ちゃん
            internal const int Genki = 60;      //ボクっ娘
            internal const int Sadist = 70;     //ドＳ
            internal const int Silent = 110;    //文学少女
            internal const int Devilish = 120;  //小悪魔
            internal const int Ladylike = 130;  //おしとやか
            internal const int Secretary = 140; //メイド秘書
            internal const int Sister = 150;    //ふわふわ妹
            internal const int Curtness = 160;  //無愛想
            internal const int Missy = 170;     //お嬢様
            internal const int Childhood = 180; //幼馴染
            internal const int Masochist = 190; //ド変態ドＭ
            internal const int Cunning = 200;   //腹黒
            internal const int Friendly = 210;  //気さく
            internal const int Dame = 220;      //淑女
            internal const int Gyaru = 230;     //ギャル
        }

        internal static class MapType
        {
            internal const int Theater = 10;            //劇場
            internal const int MasterRoom = 40;         //主人公部屋
            internal const int GuestBedroom = 70;       //宿泊施設・寝室
            internal const int ShoppingMall = 50;       //ショッピングモール
            internal const int GuestWashroom = 100;     //宿泊施設・洗面所
            internal const int GuestLivingRoom = 80;    //宿泊施設・リビング
            internal const int Soapland = 20;           //ソープランド
            internal const int SMRoom = 30;             //SMクラブ
            internal const int GuestToilet = 90;        //宿泊施設・トイレ
            internal const int MaidRoom = 60;           //メイド部屋
            internal const int Basement = 110;          //地下室
            internal const int LockerRoom = 120;        //ロッカールーム
            internal const int PrivateRoom = 130;       //プライベートルーム
            internal const int PublicToilet = 140;      //トイレ
            internal const int AdultShop = 150;         //アダルトショップ
            internal const int JapanesePub = 160;       //居酒屋
            internal const int FilmingStudio = 170;     //撮影スタジオ
            internal const int LoveHotel = 180;         //ラブホテル
            internal const int NewOffice = 190;         //新執務室
            internal const int Boutique = 200;          //ブティック
            internal const int FestivalShrine = 210;    //祭りの神社
            internal const int KaraokeRoom = 220;       //カラオケルーム
            internal const int LoveHotel2 = 230;        //ラブホテル２
            internal const int HoneymoonHotel = 300;    //ハネムーンホテル
            internal const int OceanCottage = 310;      //海上コテージ
            internal const int HiddenCove = 320;        //隠れ入り江
            internal const int ShowerRoom = 330;        //シャワールーム
            internal const int DanceBar = 340;          //ダンスバー
            internal const int MasterRoomDirty = 350;   //主人公部屋IF
            internal const int DiscipliningBasementDirty = 360; //調教地下室
            internal const int DiscipliningBasement = 370;      //調教地下室２
            internal const int Nightpool = 380;         //ナイトプール
            internal const int EmpireApartments = 400;  //帝国荘
            internal const int Train = 410;             //電車
            internal const int Classroom = 420;         //教室
            internal const int JapaneseHotel = 430;     //和風旅館
            internal const int CosplayVenue = 440;      //コスプレ会場
            internal const int NightpoolGreen = 450;    //ナイトプールグリーン
            internal const int EmpireApartmentsSauna = 460;     //帝国荘サウナ
            internal const int EmpireApartmentsBBQ = 470;       //帝国荘BBQ
            internal const int EmpireApartmentsOpenAirBath = 480;   //帝国荘露天風呂
            internal const int EmpireApartments2 = 490; //帝国荘２
        }

        internal static class OrgySceneLabel
        {
            internal const string StageSelected = "mod_orgy_stage_selected";

            
        }

        internal static class SceneType
        {
            internal const string Logo = "SceneLogo";
            internal const string Warning = "SceneWarning";
            internal const string ADV = "SceneADV";
            internal const string Title = "SceneTitle";
            internal const string Daily = "SceneDaily";
            internal const string CharacterSelect = "SceneCharacterSelect";
            internal const string Yotogi = "SceneYotogi";
        }

        //I have no idea what the real names of these BGM are. I try to give them a name by impression
        //Not full list. Can be easily found in studio mode
        internal static class BGM
        {
            internal const string MainTheme = "BGM004.ogg";
            internal const string GuestMode = "BGM007.ogg";
            internal const string Soap = "BGM011.ogg";
            internal const string DailyLife = "BGM015.ogg";
            internal const string Yotogi = "BGM022.ogg";
            internal const string NightSky = "BGM019.ogg";
        }

        internal static class SE
        {
            internal const string Beep = "SE024.ogg";
            internal const string PhoneRing = "SE023.ogg";
            internal const string TakePhoto = "SE022.ogg";
            internal const string CameraShutter = "SE074.ogg";
            internal const string KnockDoor = "mmlSE_042.ogg";
            internal const string OpenDoor = "mmlSE_065.ogg";
        }


        //Probably for reference only. The value string is used in json file
        //Not full list, Can be easily found in studio mode
        internal static class BackgroundType
        {
            internal const string MasterOffice_ViewFromChair = "ShinShitsumu_ChairRot";     //新執務室, chair rotated
            internal const string MasterOffice = "ShinShitsumu";                            //新執務室
            internal const string EmpireClub_Rotary = "EmpireClub_Rotary";                  //The outside view of empire club
            internal const string EmpireClub_Entrance = "EmpireClub_Entrance";              //The lobby

            internal const string Theater = "Theater";
            internal const string HotelBedroom = "Shukuhakubeya_BedRoom";                   //宿泊部屋
            internal const string Barlounge = "Barlounge";
            
        }

        //Not full list
        internal static class FaceAnimeType
        {
            internal const string Angry = "怒り";
            internal const string CloseEye = "閉じ目";
            internal const string CloseMouthAndEye = "目口閉じ";
            internal const string Normal = "通常";
            internal const string BitterSmile = "苦笑い";
            internal const string GentleSmile = "優しさ";
            internal const string Thinking = "思案伏せ目";
            internal const string Sigh = "ためいき";
            internal const string Angry_Mu = "むー";
            internal const string Smile = "微笑み";
            internal const string Puzzled = "きょとん";
            internal const string Question = "疑問";
            internal const string SmileWithEyeClosed = "にっこり";
            internal const string Embarrassed = "恥ずかしい";
            internal const string Seduce = "誘惑";
            internal const string Scornful = "ジト目";
            internal const string Cry = "泣き";
            internal const string CloseEyes = "まぶたギュ";
            internal const string Shy = "照れ";
            internal const string TightSmile = "引きつり笑顔";
            internal const string Surprise = "びっくり";
            internal const string Smug = "ドヤ顔";
            

            internal const string EroticAnticipate = "エロ期待";
            internal const string EroticNervous = "エロ緊張";
            internal const string EroticNormal1 = "エロ通常１";
            internal const string EroticNormal2 = "エロ通常２";
            internal const string EroticNormal3 = "エロ通常３";
            internal const string EroticExcited0 = "エロ興奮０";
            internal const string EroticExcited1 = "エロ興奮１";
            internal const string EroticExcited2 = "エロ興奮２";
            internal const string EroticExcited3 = "エロ興奮３";
            internal const string EroticShy1 = "エロ羞恥１";
            internal const string EroticShy2 = "エロ羞恥２";
            internal const string EroticShy3 = "エロ羞恥３";
            internal const string EroticDisgust1 = "エロ嫌悪１";
            internal const string EroticDisgust2 = "エロ嫌悪２";
            internal const string EroticLikeable1 = "エロ好感１";
            internal const string EroticLikeable2 = "エロ好感２";
            internal const string Estrus = "発情";
            internal const string Aftertaste = "余韻弱";

            internal const string AfterOrgasm1 = "絶頂射精後１";
            internal const string AfterOrgasm2 = "絶頂射精後２";
            internal const string AfterOrgasmExcite1 = "興奮射精後１";
            internal const string AfterOrgasmExcite2 = "興奮射精後２";

            

        }

        internal static readonly string[] FaceAnimeRandomSmileList = { FaceAnimeType.GentleSmile, FaceAnimeType.Smile, FaceAnimeType.SmileWithEyeClosed };
        internal static readonly string[] FaceAnimeRandomOhList = { FaceAnimeType.Thinking, FaceAnimeType.Thinking, FaceAnimeType.Puzzled };
        internal static readonly string[] FaceAnimeRandomHornyList =
            { FaceAnimeType.EroticExcited0, FaceAnimeType.EroticExcited1, FaceAnimeType.EroticExcited2, FaceAnimeType.EroticExcited3, 
              FaceAnimeType.EroticAnticipate, FaceAnimeType.Aftertaste
            };
        internal static readonly string[] FaceAnimeRandomRestList =
            { FaceAnimeType.Sigh, FaceAnimeType.CloseEye, FaceAnimeType.AfterOrgasm1, FaceAnimeType.AfterOrgasm2,
              FaceAnimeType.AfterOrgasmExcite1, FaceAnimeType.AfterOrgasmExcite2
            };
        internal static readonly MotionInfo[] RandomMotionFemaleRestList =
        {
            new MotionInfo("work_001.ks", "*しゃがみ＿待機", "syagami_pose_f.anm", "syagami_pose_f.anm"),
            new MotionInfo("ero_scene_001.ks", "*気絶", "ero_scene_kizetu_f.anm", "ero_scene_kizetu_f.anm"),
            new MotionInfo("ero_scene_001.ks", "*ピロトーク_一人", "pillow_talk_f.anm", "pillow_talk_f.anm")
        };

        internal static readonly MotionInfo[] RandomMotionMaleRestList =
        {
            //new MotionInfo("work_001.ks", "*しゃがみ＿待機", "syagami_pose_f.anm", "syagami_pose_f.anm"),
            //new MotionInfo("ero_scene_001.ks", "*気絶", "ero_scene_kizetu_f.anm", "ero_scene_kizetu_f.anm"),
            //new MotionInfo("ero_scene_001.ks", "*ピロトーク_一人", "pillow_talk_f.anm", "pillow_talk_f.anm")
            new MotionInfo("", "", "mp_arai_taiki_m.anm", "mp_arai_taiki_m.anm"),
            new MotionInfo("", "", "taimenzai3_taiki_m.anm", "taimenzai3_taiki_m.anm"),
            new MotionInfo("", "", "om_kousoku_aibu_taiki_m.anm", "om_kousoku_aibu_taiki_m.anm")
        };

        //For coding use
        internal static class RandomFaceAnimeCode
        {
            internal const string RandomSmile = "RandomSmile";
            internal const string RandomOh = "RandomOh";
            internal const string RandomHorny = "RandomHorny";
            internal const string RandomRest = "RandomRest";
        }

        internal static class RandomMotionCode
        {
            internal const string RandomRest = "RandomRest";
        }

        //////internal static class ADVSpecialValues
        //////{
        //////    internal static string ModScenarioEndID = "<<<END>>>";   //In the scenario json file, if the next step id is same with this string, the mod scenario will end and return to the normal flow
        //////}

        internal static class ADVType
        {
            internal const string Chara = "Chara";
            internal const string Group = "Group";
            internal const string Talk = "Talk";
            internal const string ChangeBGM = "BGM";
            internal const string PlaySE = "SE";
            internal const string ChangeBackground = "BG";
            internal const string ChangeScene = "Scene";
            internal const string ChangeCamera = "Camera";
            internal const string ShowChoiceList = "List";

            internal const string CloseMsgPanel = "CloseMsgPanel";
            internal const string LoadScene = "LoadScene";

            internal const string CharaInit = "CharaInit";
            internal const string BranchByPersonality = "BranchByPersonality";
            internal const string BranchByMap = "BranchByMap";
            
            internal const string BranchByPlace = "BranchByPlace";
            internal const string Special = "Special";  //Indicate the step require some special handling and need extra coding

            internal const string Pick = "Pick";
            internal const string MakeGroup = "MakeGroup";  //Assign characters into a group in order to set group motion etc
            internal const string DismissGroup = "DismissGroup";  //Assign characters into a group in order to set group motion etc

            internal const string RemoveTexture = "RemoveTexture";

            internal const string Shuffle = "Shuffle";

            internal const string ADVEnd = "ADVEnd";    //End the scenario and return to normal flow
        }

        internal static class ADVTalkSpearkerType
        {
            internal const string Narrative = "Narrative";
            internal const string Owner = "Owner";
            internal const string SelectedMaid = "SelectedMaid";        //Maid 0
            internal const string Maid = "Maid";                        //Require Index position
            internal const string RandomMaid = "RandomMaid";
            internal const string NPC = "NPC";
            internal const string All = "All";
        }

        internal static class WaitingType
        {
            internal const string Auto = "Auto";    //player no need to do anything and the adv scene will proceed to next step
            internal const string Click = "Click";
            internal const string InputChoice = "InputChoice";
            internal const string FadeOut = "FadeOut";
            internal const string CameraPan = "CameraPan";

            internal const string Special = "Special";  //Need special handling to proceed to next step (eg. doing some branching). 
        }

        internal static class JsonReplaceTextLabels
        {
            internal const string MaidZeroName = "[=Maid0Name]";
            internal const string MaidOneName = "[=Maid1Name]";
            internal const string MaidTwoName = "[=Maid2Name]";
            internal const string ClubName = "[=ClubName]";

            internal const string PersonalityType = "[=PType]";
            internal const string MapType = "[=MType]";

            internal const string ManCount = "[=ManCount]";
            internal const string OrgasmCount = "[=OrgasmCount]";

            internal const string RandomGroupRegex = @"\[\=RandomGroup,(\d{1,2}),([a-zA-Z0-9]+)\]";
        }

        internal static class ResourcesTextReplaceLabel
        {
            internal const string EventName = "[=EventName]";
            internal const string MinMaidCount = "[=Min]";
            internal const string MaxMaidCount = "[=Max]";

        }

        //While there are more different types such as lesbian play or 3M1F etc, they are omitted due to we dont cater them.
        internal static class GroupType
        {
            internal const string MF = "MF";
            internal const string MMF = "MMF";
            internal const string FFM = "FFM";
        }

        internal static class TargetType
        {
            internal const string SingleMan = "M";
            internal const string SingleMaid = "F";
            internal const string AllMen = "M_ALL";
            internal const string AllMaids = "F_ALL";
            internal const string NPC = "NPC";
        }

        internal static class EventProgress
        {
            internal const string None = "NONE";
            internal const string Init = "INIT";
            internal const string IntroADV = "INTRO_ADV";
            internal const string YotogiInit = "YOTOGI_INIT";
            internal const string YotogiPlay = "YOTOGI_PLAY";
            internal const string YotogiEnd = "YOTOGI_END";

            internal const string PostYotogiADV = "POST_YOTOGI_ADV";

            internal const string EndingADV = "ENDING_ADV";
        }

        internal static class ManBodyOptionType
        {
            internal const string Head = "head";
            internal const string Body = "body";
        }

        internal static class CommandButtonColor
        {
            internal static UnityEngine.Color Normal { get { return new UnityEngine.Color(1f,1f,1f, 0.671f); } }
            internal static UnityEngine.Color Hover { get { return new UnityEngine.Color(1f, 1f, 1f, 1f); } }
            internal static UnityEngine.Color Pressed { get { return new UnityEngine.Color(1f, 1f, 1f, 0.784f); } }
            internal static UnityEngine.Color Disabled { get { return new UnityEngine.Color(0.275f, 0.275f, 0.275f, 1f); } }
        }

        internal static class CommandButtonTextColor
        {
            internal static UnityEngine.Color Normal { get { return new UnityEngine.Color(0f, 0f, 0f, 1f); } }
            
        }

        internal static class DefinedGameObjectNames
        {
            internal const string ModButtonPrefix = "mod_btn_";
            internal const string ModAddedManGameObjectPrefix = "Man_";

            internal const string MainCategoryPanel = "MainCategoryViewer";
            internal const string SkillViewerPanel = "SkillViewer";

            internal const string YotogiPlayManagerCommandExecConditionPanelBackground = "BG";
            

            internal const string MaidHeadBoneName = "Bip01 Head";
            internal const string ManHeadBoneName = "ManBip Head";

            internal const string YotogiCommandCategoryTitle = "CommandCategoryTitle(Clone)";
        }

        internal static class DefinedClassFieldNames
        {
            internal const string YotogiManagerMaidField = "maid_";
            internal const string YotogiPlayManagerMaidField = "maid_";
            internal const string YotogiPlayManagerCommandExecConditionList = "commandExecConditionList";
            internal const string YotogiPlayManagerCommandExecConditionPanel = "commandExecConditionsPanel";
            internal const string YotogiPlayManagerEstrusMode = "estrusMode";

            internal const string YotogiPlayManagerPositionChanger = "positionChanger";

            internal const string YotogiPlayManagerPositionChangerButton = "m_AppearButton";

            internal const string WfScreenChildrenFadeStatus = "fade_status_";

            internal const string CharacterSelectMainSelectedMaid = "select_maid_";

            internal const string UndressingManagerAllMaidArray = "all_maid_data_";
        }

        internal static class DefinedSearchPath
        {
            internal const string MaskGroupPathFromCommandViewer = "SkillViewer/MaskGroup";
        }

        //Do not translate this class
        internal static class GameDefinedFlag
        {
            internal static class Maid
            {
                internal const string YotogiCategorySwappingExecuteTimes = "夜伽_カテゴリー_実行回数_交換";
                internal const string YotogiCategoryOrgyExecuteTimes = "夜伽_カテゴリー_実行回数_乱交";
            }
            internal static class Master
            {

            }
        }

        internal static class ModUsedFlag
        {
            internal static class Maid
            {
                internal const string RequireCategorySwappingOrOrgy = "MOD_Require_Category_Swapping_Or_Orgy";
            }

            internal static class Master
            {

            }
        }

        internal static class MotionLabelType
        {
            internal const string Waiting = "Waiting";
        }

        internal static class SpecialCoordinateType
        {
            internal const string Owner = "Owner";
            internal const string UnassignedMaid = "UnassignedMaid";
        }

        internal static class IndividualCoordinateType
        {
            internal const string Maid = "F";
        }

        internal static class CallScreenName {
            internal const string Move = "Move";
            internal const string Title = "SceneToTitle";
        }

        internal static class NextButtonLabel
        {
            internal const string YotogiPlayEnd = "*play_end";

            
        }

        //Do not translate this class
        internal static class GameSceneLabel
        {
            internal const string NoonResultEnd = "*昼リザルト終了";
            internal const string NightResultEnd = "*夜リザルト終了";
        }

        internal static class ModYotogiCommandButtonID
        {
            internal const string ChangePosition = "ChangePosition";
            internal const string ChangePositionAll = "ChangePositionAll";
            internal const string ChangeFormation = "ChangeFormation";
            internal const string ChangePartner = "ChangePartner";
            internal const string FetishOrgy = "FetishOrgy";

            internal const string ChangeFormationHaremKing = "ChangeFormationHaremKing";
            internal const string ChangeMaidHaremKing = "ChangeMaidHaremKing";
            internal const string MoveLeftHaremKing = "MoveLeftHaremKing";
            internal const string MoveRightHaremKing = "MoveRightHaremKing";
        }

        internal static class CameraEaseType
        {
            internal const string EaseOutCubic = "easeOutCubic";
        }

        internal static class TextureType
        {
            internal const string Semen = "Semen";
        }

        internal static class TextureLayer
        {
            internal const int Semen = 18;
        }

        internal static class YotogiProgressFieldName
        {
            internal const string ManCount = "ManCount";
            internal const string OrgasmCount = "OrgasmCount";
        }
    }
}