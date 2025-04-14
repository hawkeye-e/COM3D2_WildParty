using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class ADVStep
    {
#pragma warning disable 0649
        public string ID;
        public string Type;
        public string NextStepID;
        public string WaitingType;
        public bool WaitFullLoad = false;
        public string Tag;                  //Used in:  ChangeBackground (Background Tag ID), 
                                            //          LoadScene (Scene Tag ID, refer to Constant.SceneType)
                                            //          ChangeBGM (BGM file name)
                                            //          Dismiss Group ("ALL" or group index)
        public string TagForNight = "";     //Same as Tag, will use this instead of tag if the current time is night


        public Fade FadeData;
        public Talk TalkData;
        public Camera CameraData;
        public Choice[] ChoiceData;
        public CharaInit CharaInitData;
        public ShowChara[] CharaData;
        public ShowGroupMotion[] GroupData;       //Separate from ShowChara to not making things over complicated
        public YotogiSetupInfo YotogiSetup;
        public Texture[] TextureData;
        public SE SEData;
        public Shuffle ShuffleData;                 //This need to be placed after CharaInit
        public ListUpdate ListUpdateData;           //For adding or removing the NPC from the list to include or exclude them from yotogi scene
        public List<WorldObject> WorldObjectData;

        public RandomPick PickData;             //This is for ordering to randomly pick some characters for later adv processing
        public List<MakeGroupFormat> GroupFormat;     //Assign group to perform group motion in ADV scene etc
        public ConvertSex ConvertSexData;

        internal class Fade
        {
            public bool IsFadeIn;
            public bool IsFadeOut;
        }

        internal class Talk
        {
            public string SpecificSpeaker;
            public string SpeakerName;
            public string Text;                 //////For the case of narrative, man or all maids
            public int Index;
            public bool UseBranchIndex = false;

            public Dictionary<string, Voice> VoiceData;

            internal class Voice
            {
                public string VoiceFile;
                public bool IsChoppingAudio = false;    //Some audio files is chopped in mod to fit the situation. Set this field to true for those cases.
                public float StartTime = 0f;            //Require IsChoppingAudio = true
                public float EndTime = 0f;              //Require IsChoppingAudio = true
                public float Volume = 1f;
            }

        }
        
        internal class Camera
        {
            public CameraType Type = CameraType.FixedPoint;
            public CameraDataInJson FixedPointData;     //Normally use this one for ADV scene, as we should have full control on the character placement
            public LookAtType LookAtData;               //Mainly used for situation where random character placement is involved and want to focus on certain character
            public CameraMoveType MoveType = CameraMoveType.Instant;
            public float AnimationTime = 2f;            //For Camera pan use only
            public LockCameraInfo LockCamera;
            public BlurCameraInfo BlurCamera;

            internal class LockCameraInfo
            {
                public bool IsLock = false;
            }

            internal class BlurCameraInfo
            {
                public bool IsBlur = false;
            }

            internal enum CameraType
            {
                FixedPoint,
                LookAt
            }

            internal enum TargetType
            {
                Maid,
                Man,
                Owner,
                GroupMaid1,
                GroupMaid2
            }

            internal enum CameraMoveType
            {
                Instant,
                Smooth
            }
            internal class LookAtType
            {
                public TargetType Target;
                public int ArrayPosition;
                public bool UseRandomPick = false;
                public bool UseDefaultCameraWorkSetting = true;
                public float OffsetX = ConfigurableValue.YotogiCameraWork.CameraVectorOffset.x;
                public float OffsetY = ConfigurableValue.YotogiCameraWork.CameraVectorOffset.y;
                public float OffsetZ = ConfigurableValue.YotogiCameraWork.CameraVectorOffset.z;

                public float TargetOffsetX = ConfigurableValue.YotogiCameraWork.TargetVectorOffset.x;
                public float TargetOffsetY = ConfigurableValue.YotogiCameraWork.TargetVectorOffset.y;
                public float TargetOffsetZ = ConfigurableValue.YotogiCameraWork.TargetVectorOffset.z;

                public float Distance = ConfigurableValue.YotogiCameraWork.CameraDistance;
                public float HorizontalRotation = ConfigurableValue.YotogiCameraWork.DefaultHorizontalRotation;
                public float VerticalRotation = ConfigurableValue.YotogiCameraWork.DefaultVerticalRotation;
            }

        }

        internal class Choice
        {
            public string Key;
            public string Value;
        }

        internal class CharaInit
        {
            public int ManRequired = -1;        //Indicate how many man character needed to be initialized. Negative to skip (eg. it is decided from user input)
            public List<string> ValidManType;   //Referencing RandomizeManSetting. Required if ManRequired is non negative.
            public List<NPCData> NPC;
            public List<ModNPCData> ModNPC;
            public bool IsClubOwnerADVMainCharacter = true;    //True: Man[0] will be the owner; False: Man[0] will be replaced with other man character and owner is accessible from StateManager.Instance.ClubOwner
            public bool RequiresMaidPairMan = false;            //For the use of converting maid to man

            internal class NPCData
            {
                public int Index;               //Share the same array with ModNPC
                public string Preset;           //The name could be in English or Japanese so this should be a better id?
                public bool EmptyLastName = false;      //Since the preset could be duplicates in npcdatas, use this as indicate we want to load the preset with a last name or not in order to locate the correct preset we want
                public string Name;             //For making it more readable in json only. The name dispalyed in the game should still use CallName
            }

            internal class ModNPCData
            {
                public int Index;               //Share the same array with NPC
                public string NPCID;
                public bool IsFemale = true;
            }
        }

        internal class ShowChara
        {
            public string Type;
            public int ArrayPosition;
            public bool UseBranchIndex = false;

            //In some scenario the master may be removed from the array, use this flag if want to do any setup with it in the adv. 
            //If this is set to true, the ArrayPosition has to be set to zero
            public bool IsMaster = false;       
            public bool Visible;
            public bool WaitLoad = false;
            public bool IsManNude = false;
            public bool OpenMouth = false;                              //True: open mouth for fella motion etc; False: default
            public SmoothMovementSetup SmoothMovement = null;

            public MotionInfo MotionInfo;
            public string FaceAnime;
            public string FaceBlend;
            public PosRot PosRot;

            public EyeSightSetting EyeSight;
            public ExtraObjectsSetting ExtraObjectsInfo;
            public string ClothesSetID;                                 //Special ID: "RESET", reset all applied ClothesSetID. Otherwise follows ClothesSet.json
            public EffectDetail Effect;

            public class SmoothMovementSetup
            {
                public float Time;
            }
        }

        internal class ShowGroupMotion
        {
            public bool UseRandomPick = false;
            public int ArrayPosition;

            public string ScriptFile;
            public string ScriptLabel;
            public int SexPosID = -1;           //in case we are trying to apply yotogi motion to the group

            public PosRot PosRot;
            public bool WaitLoad = false;
            public bool BlockInputUntilMotionChange = false;      

            public DetailSetup Maid1;
            public DetailSetup Maid2;
            public DetailSetup Man1;
            public DetailSetup Man2;
            public DetailSetup Man3;


            public class DetailSetup
            {
                public bool Visible;
                
                public bool IsManNude = false;

                public string FaceAnime;
                public string FaceBlend;
                public bool OpenMouth = false;
                public EyeSightSetting EyeSight;
                public PosRot PosRot;
            }
        }

        internal class YotogiSetupInfo
        {
            public string YotogiCode;
            public int Phase;
            public bool IsKeepCharacterVisibleAfterYotogi = true;              //The system will automatically hide all the characters when display the yotogi result. Set it to true to prevent from hiding them.
            public bool IsFinalYotogi = true;
            public bool IsClubOwnerMainCharacter = true;    //True: Man[0] will be the owner; False: Man[0] will be replaced with other man character and owner is accessible from StateManager.Instance.ClubOwner
            public MaidConvertToManSetting MaidConvertToMan = null;
            public bool UseModSemenPattern = false;

            public class MaidConvertToManSetting
            {
                public int RatioPercent;
            }
        }

        
        internal class RandomPick
        {
            public PickType Type;
            public int Num;

            internal enum PickType
            {
                Maid,
                Man,
                Group
            }
        }

        internal class MakeGroupFormat
        {
            public int GroupIndex;
            public GroupMemberInfo Maid1;
            public GroupMemberInfo Maid2;
            public GroupMemberInfo Man1;
            public GroupMemberInfo Man2;
            public GroupMemberInfo Man3;

            public class GroupMemberInfo
            {
                public string Type;
                public int ArrayPosition;
            }

            public class MemberType
            {
                public const string Owner = "Owner";
                public const string NPC = "NPC";
            }

        }



        public class Texture
        {
            public string Type;
            public string TargetType = Constant.TargetType.SingleMaid;      //Use Constant.TargetType; Valid Option: "F": maid list, "F_ALL": All maids, "NPC_F": NPC Female List
            public int IndexPosition;                                       
            public List<string> BodyTarget;                                 //Required for add only. Ignored when remove texture.
        }

        internal class SE
        {
            public string FileName;
            public bool IsLoop = false;
            public bool Stop = false;
        }

        internal class Shuffle
        {
            public string TargetList;
            public List<int> KeepPosition;
        }

        internal class ListUpdate
        {
            public List<ListUpdateDetail> Add;
            public List<ListUpdateDetail> Remove;

            internal class ListUpdateDetail
            {
                public string Type;
                public int SrcPosition;
                public int PositionToInsert;
            }
        }

        internal class ExtraObjectsSetting
        {
            public List<ExtraItemObject> AddObjects;
            public List<string> RemoveObjects;     //For remove, only need to provide the Target
        }

        internal class WorldObject
        {
            public string Src;
            public string ObjectID;                 //Used for removing the object
            public PosRot PosRot;
            public float Scale = 1;
        }

        internal class EffectDetail
        {
            public List<string> Add;
            public List<string> Remove;
        }

        internal class ConvertSex
        {
            public List<ConvertSexDetail> ToMale;           //Convert a maid to male structure 
            public List<ConvertSexDetail> ToFemale;         //Convert a man to female structure
            public List<ConvertSexDetail> BackToMale;       //Revert a converted structure character back to male
            public List<ConvertSexDetail> BackToFemale;     //Revert a converted structure character back to female

            public class ConvertSexDetail
            {
                public string Type;                         //Use Constant.TargetType
                public int ArrayPosition;
            }
        }

#pragma warning restore 0649
    }
}
