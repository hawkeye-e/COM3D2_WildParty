using COM3D2.WildParty.Plugin.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    class StateManager
    {
        public StateManager()
        {

        }

        internal static StateManager Instance;

        
        internal bool IsInjectScheduleOptionsFinish = false;
        internal bool IsInjectPropensityFinish = false;

        internal int currentGroup = -1;
        internal List<PartyGroup> PartyGroupList = new List<PartyGroup>();
        internal List<Maid> OriginalManOrderList = new List<Maid>();
        internal List<Maid> SelectedMaidsList = new List<Maid>();
        internal List<Maid> MenList = new List<Maid>();
        internal List<Maid> NPCList = new List<Maid>();                         //For female NPC (both in-game or mod added) only
        internal List<Maid> NPCManList = new List<Maid>();                      //For male NPC only

        //Key: Maid, Value: Paired Man
        internal Dictionary<Maid, Maid> PairedManForMaidList = new Dictionary<Maid, Maid>();    //For maid to convert man use

        internal List<Maid> DummyManList = new List<Maid>();                    //This is for filling the null place in the man array of the system.
        //Key: Maid GUID
        internal Dictionary<string, ManClothingInfo> ManClothingList = new Dictionary<string, ManClothingInfo>();   //For storing the info of clothed and nude body of a man
        internal List<Maid> YotogiWorkingMaidList = new List<Maid>();           //For used in Yotogi scene, will be shuffled etc
        internal List<Maid> YotogiWorkingManList = new List<Maid>();            //For used in Yotogi scene, will be shuffled etc
        internal Dictionary<string, YotogiProgressInfo> YotogiProgressInfoList = new Dictionary<string, YotogiProgressInfo>();
        internal Maid ClubOwner;

        //Key: Maid Guid, Value.Key: ClothingTag, Value.Value: clothes file name 
        internal Dictionary<string, Dictionary<string, string>> BackupMaidClothingList = new Dictionary<string, Dictionary<string, string>>();

        //Key: Object ID defined in json
        internal Dictionary<string, GameObject> AddedGameObjectList = new Dictionary<string, GameObject>();

        internal int MaxManUsed = -1;

        internal int UndergoingModEventID = -1;                     //A number as to fit the data type of the schedule event in the game
        internal bool RequireCheckModdedSceneFlag = false;          //flag for indicating the condition of modded scene is to be checked or not. True: need to check
        
        internal bool SpoofActivateMaidObjectFlag = false;          //flag for prevent the system to uninit a maid object when shuffling

        internal bool SpoofChangeBackgroundFlag = false;          //flag for prevent the system to uninit a maid object when shuffling

        internal string ModEventProgress = Constant.EventProgress.None;

        internal bool WaitForCharactersFullLoadFlag = false;                //flag for waiting the scene to load all required characters etc.
        internal List<Maid> WaitForFullLoadList = new List<Maid>();                //flag for waiting the scene to load the required characters etc.
        internal List<Maid> IgnoreResetPropMaidList = new List<Maid>();
        internal List<Maid> MaidAsManFaceAnimeChangeList = new List<Maid>();

        internal UILabel CommandLabel = null;
        internal YotogiManager YotogiManager = null;
        internal YotogiCommandFactory YotogiCommandFactory = null;
        internal Transform CommandMaskGroupTransform = null;
        internal bool RequireInjectCommandButtons = false;          //flag for inject mod use command buttons in yotogi scene
        internal List<CustomGameObject.InjectYotogiCommand> InjectedButtons = new List<CustomGameObject.InjectYotogiCommand>();

        internal bool SpoofTagListResult = false;
        internal Dictionary<string, string> TagBackUp = null;

        

        internal string CurrentADVStepID = "";
        internal string ProcessedADVStepID = "";
        internal bool WaitForUserClick = false;
        internal bool WaitForUserInput = false;
        internal bool WaitForCameraPanFinish = false;
        internal bool WaitForSystemFadeOut = false;
        internal bool WaitForMotionChange = false;
        internal CameraView TargetCameraAfterAnimation = null;

        internal bool ForceNoCameraResetAfterFadeIn = false;

        internal List<int> RandomPickIndexList = null;
        
        //these 2 are for return the correct maid / man due to not using the list implemented by KISS
        internal string processingMaidGUID = "";                    
        internal string processingManGUID = "";

        internal CustomGameObject.YotogiExtraCommandWindow ExtraCommandWindow = null;
        internal CustomGameObject.YotogiExtraCommandWindow ExtraCommandWindowMasterCopy = null;

        internal bool SpoofAudioLoadPlay = false;

        //When the target starts the defined animation name, execute the event deletgate
        internal AnimationEndTrigger WaitingAnimationTrigger = null;
        internal AnimationEndTrigger AnimationChangeTrigger = null;

        internal VoiceLoopTrigger VoiceLoopTrigger = null;
        internal List<TimeEndTrigger> TimeEndTriggerList = new List<TimeEndTrigger>();

        internal DateTime ADVResumeTime = DateTime.MinValue;

        internal int BranchIndex = -1;                          //For ADV processing

        internal int YotogiPhase = 0;
        internal bool IsFinalYotogi = true;

        internal PartyGroup CurrentMotionKagHandlingGroup = null;
        internal bool IsMotionKagSetPosition = false;
        internal bool IsMainGroupMotionScriptFlag = false;
        
        internal bool IsYotogiUseModSemenPattern = false;
    }
}
