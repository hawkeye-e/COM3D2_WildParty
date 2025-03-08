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

        internal static readonly string[] FellatioLabel = { "Fellatio", "DeepThroat", "PainFellatio", "FellatioExcite", "FellatioDrunk", "FellatioBitch" };

        internal const string MainEventScriptFile = "AllMain.ks";

        internal static readonly string[] ModIconNames = { 
            ModResources.ImageResources.OrgyPartyIconFileName, 
            ModResources.ImageResources.HaremKingIconFileName,
            ModResources.ImageResources.HappyGBClubIconFileName
        };

        internal static readonly string[] DressingClothingTagArray = {
            ClothingTag.acchat,
            ClothingTag.headset,
            ClothingTag.wear,
            ClothingTag.skirt,
            ClothingTag.onepiece,
            ClothingTag.mizugi,
            ClothingTag.bra,
            ClothingTag.panz,
            ClothingTag.acckubi,
            ClothingTag.acckubiwa,
            ClothingTag.shoes,
            ClothingTag.stkg,
            ClothingTag.accude,
            ClothingTag.accsenaka,
            ClothingTag.glove,
            ClothingTag.accashi,
            ClothingTag.accshippo,
        };


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
            internal const string LoadYotogi = "LoadYotogi";  //Separated from Special. 
            
            internal const string Pick = "Pick";
            internal const string MakeGroup = "MakeGroup";  //Assign characters into a group in order to set group motion etc
            internal const string DismissGroup = "DismissGroup";  //Assign characters into a group in order to set group motion etc

            internal const string AddTexture = "AddTexture";
            internal const string RemoveTexture = "RemoveTexture";

            internal const string Shuffle = "Shuffle";
            internal const string ListUpdate = "ListUpdate";
            internal const string TimeWait = "TimeWait";

            internal const string ADVEnd = "ADVEnd";    //End the scenario and return to normal flow
        }

        internal static class ADVTalkSpearkerType
        {
            internal const string Narrative = "Narrative";
            internal const string Owner = "Owner";
            internal const string SelectedMaid = "SelectedMaid";        //Maid 0
            internal const string Maid = "Maid";                        //Require Index position
            internal const string RandomMaid = "RandomMaid";
            internal const string NPCFemale = "NPC_F";
            internal const string NPCMale = "NPC_M";
            internal const string All = "All";
        }

        internal static class WaitingType
        {
            internal const string Auto = "Auto";    //player no need to do anything and the adv scene will proceed to next step
            internal const string Click = "Click";
            internal const string InputChoice = "InputChoice";
            internal const string FadeOut = "FadeOut";
            internal const string CameraPan = "CameraPan";
            internal const string SystemFadeOut = "SystemFadeOut";      //This one is for waiting for the fade out caused by the original system not by this mod

            internal const string Special = "Special";  //Need special handling to proceed to next step (eg. doing some branching). 
        }

        internal static class JsonReplaceTextLabels
        {
            internal const string ClubName = "[=ClubName]";
            internal const string ClubOwnerName = "[=ClubOwnerName]";

            internal const string PersonalityType = "[=PType]";
            internal const string MapType = "[=MType]";

            internal const string ManCount = "[=ManCount]";
            internal const string OrgasmCount = "[=OrgasmCount]";
            internal const string CurrentManCount = "[=CurrentManCount]";
            internal const string CurrentOrgasmCount = "[=CurrentOrgasmCount]";

            /*Format: [=Name,{Source},{Index},{CallMethod}]
             * {Source} :   "Maid" : From SelectedMaidsList; "NPC_F" : From NPCList;  "NPC_M" : From NPCManList
             * {Index}  :   Index position in the source list
             * {CallMethod} :   Options: "CallName", "FullName", "LastName", "FirstName"
             */
            internal const string CharacterNameRegex = @"\[\=Name,(Maid|NPC_F|NPC_M),(\d{1,2}),(CallName|FullName|LastName|FirstName|NickName)\]";
            //internal const string CharacterNameRegex = @"\[\=Name,(\d{1,2}),([a-zA-Z0-9]+)\]";

            //Format: [=RandomGroup,{GroupIndex},Maid{MaidIndex}Name]
            internal const string RandomGroupRegex = @"\[\=RandomGroup,(\d{1,2}),([a-zA-Z0-9]+)\]";

            internal static class CharacterNameSourceType
            {
                internal const string Maid = "Maid";
                internal const string NPCFemale = "NPC_F";
                internal const string NPCMale = "NPC_M";
            }
            internal static class CharacterNameDisplayType
            {
                internal const string CallName = "CallName";
                internal const string FullName = "FullName";
                internal const string LastName = "LastName";
                internal const string FirstName = "FirstName";
                internal const string NickName = "NickName";
            }
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
            internal const string MMMF = "MMMF";
        }

        internal static class TargetType
        {
            internal const string SingleMan = "M";
            internal const string SingleMaid = "F";
            internal const string AllMen = "M_ALL";
            internal const string AllMaids = "F_ALL";
            internal const string NPCFemale = "NPC_F";
            internal const string NPCMale = "NPC_M";
        }

        internal static class EventProgress
        {
            internal const string None = "NONE";
            internal const string Init = "INIT";
            internal const string IntroADV = "INTRO_ADV";
            internal const string YotogiInit = "YOTOGI_INIT";
            internal const string YotogiPlay = "YOTOGI_PLAY";
            internal const string YotogiEnd = "YOTOGI_END";

            internal const string ADV = "ADV";
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
            internal const string YotogiPlayManagerPlayerState = "player_state_";
            internal const string YotogiPlayManagerParamBasicBar = "param_basic_bar_";

            internal const string YotogiPlayManagerPositionChanger = "positionChanger";

            internal const string YotogiPlayManagerPositionChangerButton = "m_AppearButton";

            internal const string WfScreenChildrenFadeStatus = "fade_status_";

            internal const string CharacterSelectMainSelectedMaid = "select_maid_";

            internal const string UndressingManagerAllMaidArray = "all_maid_data_";

            internal const string MaidStatusFirstName = "firstName_";
            internal const string MaidStatusLastName = "lastName_";
            internal const string MaidStatusNickName = "nickName_";

            internal const string TBodyAnimCache = "m_AnimCache";
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
            internal const string Man = "M";
            internal const string NPCMale = "NPC_M";
            internal const string NPCFemale = "NPC_F";
            internal const string Owner = "Owner";
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

            internal const string OrgasmExternal = "OrgasmExternal";
            internal const string OrgasmInternal = "OrgasmInternal";
            internal const string OrgasmMouth = "OrgasmMouth";
            internal const string OrgasmFace = "OrgasmFace";
            internal const string OrgasmBukkake = "OrgasmBukkake";
            internal const string OrgasmBukkake2 = "OrgasmBukkake2";

            internal const string ChangeFormationHappyGBClub = "ChangeFormationHappyGBClub";
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

        internal static class OrgasmType
        {
            internal const string Face = "Face";
            internal const string Mouth = "Mouth";
            internal const string Internal = "Internal";
            internal const string External = "External";
            internal const string Bukkake = "Bukkake";
            internal const string Bukkake2 = "Bukkake2";
        }

        
        internal static class ClothingTag
        {
            internal const string acchat = "acchat";            //帽子
            internal const string headset = "headset";          //ヘッドドレス
            internal const string acckami = "acckami";          //前髪
            internal const string acckamisub = "acckamisub";    //リボン
            internal const string wear = "wear";                //トップス
            internal const string skirt = "skirt";              //ボトムス
            internal const string megane = "megane";            //メガネ
            internal const string acchead = "acchead";          //アイマスク
            internal const string onepiece = "onepiece";        //ワンピース
            internal const string mizugi = "mizugi";            //水着
            internal const string accmimi = "accmimi";          //耳
            internal const string acchana = "acchana";          //鼻
            internal const string bra = "bra";                  //ブラジャー
            internal const string panz = "panz";                //パンツ
            internal const string acckubi = "acckubi";          //ネックレス
            internal const string acckubiwa = "acckubiwa";      //チョーカー
            internal const string shoes = "shoes";              //靴
            internal const string stkg = "stkg";                //靴下
            internal const string accude = "accude";            //腕
            internal const string accsenaka = "accsenaka";      //背中
            internal const string glove = "glove";              //手袋
            internal const string accashi = "accashi";          //足首
            internal const string accshippo = "accshippo";      //しっぽ
            internal const string accheso = "accheso";          //へそ
            internal const string accnip = "accnip";            //乳首
            internal const string accxxx = "accxxx";            //前穴
        }

        internal static class SexStateRuleDefinition
        {
            internal const string Common = "Common";
            internal const string GangBangNormal = "GangBangNormal";
            internal const string GangBangQueued = "GangBangQueued";
        }

        internal static class YotogiSceneCode
        {
            internal const string OrgyParty = "OrgyParty";
            internal const string HaremKing = "HaremKing";
            internal const string HappyGBClub = "HappyGBClub";
        }

        internal static class TexturePattern
        {
            internal class Semen
            {
                internal const string Vagina = "Vagina";
                internal const string Belly = "Belly";
                internal const string Ass = "Ass";
                internal const string Back = "Back";
                internal const string Breast = "Breast";
                internal const string Mouth = "Mouth";
                internal const string Face = "Face";
            }
        }
    }
}