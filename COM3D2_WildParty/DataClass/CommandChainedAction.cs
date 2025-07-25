using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    //All the Remarks properties in the json will not be read into memory. It is only used for note down what the step does for readability.
    internal class CommandChainedAction
    {
        public string FirstStep;            //Defines the entry point of the chained action
        public List<StepDetail> Steps;
        //public List<string> RequiredParams;

        public class StepDetail
        {
            public string StepID;
            public StepActionType ActionType;
            public TargetInfo Target;
            //public int TargetGroupPosition;
            //public int TargetMaidPosition;
            public StepMotionInfo MotionInfo;
            public ExtraManInfo ExtraManInfo;
            public OffsetInfo OffsetInfo;
            public PositionInfo PositionInfo;
            public TriggerSettingInfo TriggerSetting;
            public SetExtraManInfo SetExtraManInfo;
            public SetExtraManInfo RemoveExtraManInfo;
            public ManWalkSetupInfo ManWalkSetupInfo;
            public SetGroupMemberInfo SetGroupMemberInfo;
            public UpdateSexPosInfo UpdateSexPosInfo;
            public LoadMotionLabelGroupInfo LoadMotionLabelGroupInfo;
            public SwapGroupMemberInfo SwapGroupMemberInfo;

            public ValueData Value;

            public string NextStep;                
            public NextStepTriggerType NextStepTriggerType = NextStepTriggerType.None;
        }

        public class TargetInfo
        {
            public TargetType Type;
            public int TargetGroupPosition;
            public int TargetMaidPosition;

            public string ParamName;
            public SubParamType ParamGroupMember = SubParamType.None;               //Only in use if it is group param
            public enum SubParamType
            {
                None,
                Man1,
                Man2,
                Man3,
                Maid1,
                Maid2
            }
        }

        public class StepMotionInfo : MotionInfo
        {
            public string ParamMotion;
        }

        public enum StepActionType
        {
            ResetMotionToWaiting,
            LoadMotion,                 //Use this to load motion if the motion file, script file or custom motion file can be provided
            ResetIK,
            GetNextExtraMan,
            PositionOffset,
            SetPosition,
            Trigger,
            SetMaidToExtraMan,
            ManWalk,
            SetGroupMember,
            RemoveExtraMan,

            UpdateSexPos,
            LoadMotionLabelGroup,        //Use this for Load motion with the label group id defined in the mod
            SwapGroupMember
        }

        public enum TargetType
        {
            Group,              //The whole group
            GroupMaid,          //The maid in the group 
            GroupMan,           //The man in the group
            Maid,               //The maid in the WorkingMaid array
            Man,                //The man in the WorkingMan array
            TargetGroup,              //Target list stored in the processor
            TargetMaid,              //Target list stored in the processor
            Param,               //Use the Param string to get the target
            Null                //Request to return null
        }

        public enum NextStepTriggerType
        {
            None,
            AnimationEnd,
            TimeEnd
        }

        public enum ExtraManType
        {
            Group,
            Global
        }

        public class ExtraManInfo
        {
            public ExtraManType Type;
            public NextMethodType GetNextMethod;

            public enum NextMethodType
            {
                Increment,
                Random
            }
        }

        public class OffsetInfo
        {
            public PosRotVectorFormat Offset;
            public string Param;
            public bool IsSmoothAnimation = true;
            public string IsSmoothAnimationParamName;
        }

        public class PositionInfo
        {
            public TargetInfo Target;
            public string DefinedPointParamName;        //If this value is set, will use the point name defined in the param
            public string DefinedPointName;
            public bool IsSmoothAnimation = true;
            public string IsSmoothAnimationParamName;
        }

        public class TriggerSettingInfo
        {
            public NextStepTriggerType TriggerType;
            public TimeEndTriggerWaitType TimeToWaitType;
            public float CustomTimeToWait;

            public enum TimeEndTriggerWaitType
            {
                DefaultAnimationBlendTime,
                DefaultTweenMoveTime,
                Custom
            }
        }

        public class SetExtraManInfo
        {
            public string GroupParamName;                //if this value is set, will get the group from parameters

            public ExtraManType Type;
            public int TargetGroupPosition;     //For ExtraManType = Group only. Indicate which group's extra man this man will be set
            public bool IsSmoothPositionChange = true;
            public bool IsSmoothAnimationChange = true;
            public WayToLocateEmpty LocateEmptyMethod = WayToLocateEmpty.FirstEmpty;

            public enum WayToLocateEmpty
            {
                FirstEmpty,
                LastEmpty,
                Random
            }
        }

        internal class ManWalkSetupInfo
        {
            public TargetInfo Destination;
            public string SetupParam;                           //If this value is set, will get the setting info from param

            public string StandingAnimationFile;
            [JsonProperty]
            private string StandingMotionOffsetString;
            [JsonProperty]
            private string WalkingMotionOffsetString;
            [JsonProperty]
            private string RotationOffsetString;
            [JsonProperty]
            private string MaidMotionOffsetString;                //Used for fixing the issue of the man "walk over" the maid 


            public Vector3 StandingMotionOffset
            {
                get { return Util.ParseVector3RawString(StandingMotionOffsetString); }
            }
            public Vector3 WalkingMotionOffset
            {
                get { return Util.ParseVector3RawString(WalkingMotionOffsetString); }
            }

            public Vector3 RotationOffset
            {
                get { return Util.ParseVector3RawString(RotationOffsetString); }
            }

            public Vector3 MaidMotionOffset
            {
                get { return Util.ParseVector3RawString(MaidMotionOffsetString); }
            }

        }

        public class SetGroupMemberInfo
        {
            public int TargetGroupPosition;
            public string TargetParam;          //If this value is set, will use this to get the index position instead of TargetGroupPosition
            public TargetInfo Man1;
            public TargetInfo Man2;
            public TargetInfo Man3;
            public TargetInfo Maid1;
            public TargetInfo Maid2;
            public ValueData NewSexPos;
            public YotogiPlay.PlayerState PlayerState = YotogiPlay.PlayerState.Normal;
            public bool IsFinalizedGroupUpdateStep = true;          //Set it to true if the step is not a finalized group update
        }

        public class UpdateSexPosInfo
        {
            public ValueData Value;
            public YotogiPlay.PlayerState PlayerState = YotogiPlay.PlayerState.Normal;
            public bool IsSmoothAnimation = true;
            public string IsSmoothAnimationParamName;           //If this is supplied, it will override the IsSmoothAnimation value
        }

        public class LoadMotionLabelGroupInfo
        {
            public ValueData Value;
            public bool IsSmoothAnimation = true;
            public string IsSmoothAnimationParamName;           //If this is supplied, it will override the IsSmoothAnimation value
        }

        public class SwapGroupMemberInfo
        {
            public TargetInfo Target1;
            public TargetInfo Target2;
        }

        public class ValueData
        {
            public string ParamValue;       //If this is set, the value will be get from parameter list
            public object Value;            //plain value supplied in the json file
        }
    }
}
