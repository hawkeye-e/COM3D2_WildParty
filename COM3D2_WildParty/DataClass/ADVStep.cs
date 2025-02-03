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
        public PostYotogiSetup PostYotogi;
        public Texture[] TextureData;

        public RandomPick PickData;             //This is for ordering to randomly pick some characters for later adv processing
        public List<MakeGroupFormat> GroupFormat;     //Assign group to perform group motion in ADV scene etc

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

            public Dictionary<string, Voice> VoiceData;

            internal class Voice
            {
                public string VoiceFile;
                public bool IsChoppingAudio = false;    //Some audio files is chopped in mod to fit the situation. Set this field to true for those cases.
                public float StartTime = 0f;            //Require IsChoppingAudio = true
                public float EndTime = 0f;              //Require IsChoppingAudio = true
            }

        }
        
        internal class Camera
        {
            public CameraType Type = CameraType.FixedPoint;
            public FixedPointType FixedPointData;       //Normally use this one for ADV scene, as we should have full control on the character placement
            public LookAtType LookAtData;               //Mainly used for situation where random character placement is involved and want to focus on certain character
            public CameraMoveType MoveType = CameraMoveType.Instant;

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

            internal class FixedPointType
            {
                public float PosX;
                public float PosY;
                public float PosZ;
                public float TargetPosX;
                public float TargetPosY;
                public float TargetPosZ;
                public float Distance;
                public float AngleFrom;
                public float AngleTo;
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
            public bool IsClubOwnerMainCharacter = true;    //True: Man[0] will be the owner; False: Man[0] will be replaced with other man character and owner is accessible from StateManager.Instance.ClubOwner
        }

        internal class ShowChara
        {
            public string Type;
            public int ArrayPosition;

            //In some scenario the master may be removed from the array, use this flag if want to do any setup with it in the adv. 
            //If this is set to true, the ArrayPosition has to be set to zero
            public bool IsMaster = false;       
            public bool Visible;
            public bool WaitLoad = false;
            public bool ShowPenis = false;
            public bool OpenMouth = false;                              //True: open mouth for fella motion etc; False: default

            public MotionInfoData MotionInfo;
            public string FaceAnime;
            public string FaceBlend;
            public PosRot PosRot;

            public EyeSightSetting EyeSight;

            public class MotionInfoData : MotionInfo
            {              
                public string RandomMotion;
            }
        }

        internal class ShowGroupMotion
        {
            public bool UseRandomPick = false;
            public int ArrayPosition;

            public string ScriptFile;
            public string ScriptLabel;

            public PosRot PosRot;
            public bool WaitLoad = false;

            public DetailSetup Maid1;
            public DetailSetup Maid2;
            public DetailSetup Man1;
            public DetailSetup Man2;


            public class DetailSetup
            {
                public bool Visible;
                
                public bool ShowPenis = false;

                public string FaceAnime;
                public string FaceBlend;
                public bool OpenMouth = false;
                public EyeSightSetting EyeSight;
                public PosRot PosRot;
            }
        }

        internal class PostYotogiSetup
        {
            public bool IsKeepCharacterVisible = true;              //The system will automatically hide all the characters when display the yotogi result. Set it to true to prevent from hiding them.
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

            public class GroupMemberInfo
            {
                public string Type;
                public int ArrayPosition;
            }

            public class MemberType
            {
                public const string Owner = "Owner";
            }

        }



        public class Texture
        {
            public string Type;
            public int Target = -1;         //default -1 indicate all. Otherwise index position
        }
        
    }
}
