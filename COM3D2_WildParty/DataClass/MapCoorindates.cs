using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class MapCoorindates
    {
        public string MapID;
        public string DisplayName;
        public bool IsRandom;           //True: The group or character is randomly assigned to any coordinates in the list. False: it is fixed and should follow the position order
        public List<CoordinateListInfo> CoordinateList;
        public List<CoordinatesInfo> SpecialCoordinates;
        public SpecialSettingInfo SpecialSetting;

        internal class CoordinateListInfo
        {
            public int MaxGroup;
            
            public List<CoordinatesInfo> GroupCoordinates;
            public List<CoordinatesInfo> IndividualCoordinates;
        }

        internal class CoordinatesInfo : PosRot
        {
            public int ArrayPosition = -1;
            public string Type ="";
            public MotionInfo Motion;
            public List<EyeSightSetting> EyeSight;
            public ForceSexPosInfo ForceSexPos;            //GroupCoordinates use only
            public List<IKAttachInfo> IKAttach;
            //public int ForceSexPosID = -1;          
            public string FaceAnime = "";
            public bool IsIndependentExcitement = true;     //True: Do not copy the excitement value from group zero; False: Link to the group zero
            //public bool IsAutomatedGroup = true;            //True: Use the BackgroundGroupMotionManager logic; False: Static motion
            public bool IsMasturbation = false;
            public bool IsManVisible = true;
        }

        internal class SpecialSettingInfo
        {
            public PosRot MainGroupMotionOffset;
            public ManualMovementSettingInfo MoveLeftSetting;
            public ManualMovementSettingInfo MoveRightSetting;
        }

        internal class ManualMovementSettingInfo
        {
            public MovementMotionSetting PreMoveMotion;
            public MovementMotionSetting PostMoveMotion;

            internal class MovementMotionSetting
            {
                public MotionAndOffset WaitingMotionBeforeMovement;
                public MotionAndOffset MainGroupMan;
                public MotionAndOffset MainGroupMaid;           //May have to expand to maid 2 in the future depends on the use
                public MotionAndOffset TargetGroupMan;          //For PreSwapMotion
                public MotionAndOffset TargetGroupMaid;         //For PreSwapMotion
                public MotionAndOffset OriginalGroupMan;        //For PostSwapMotion
                public MotionAndOffset OriginalGroupMaid;       //For PostSwapMotion
            }

            internal class MotionAndOffset
            {
                public MotionInfo Motion;
                public PosRot Offset;
                public PosRot TweenOffset;                      
            }
        }
    }
}
