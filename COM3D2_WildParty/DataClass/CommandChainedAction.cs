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

        public class StepDetail
        {
            public string StepID;
            public StepActionType ActionType;
            public TargetInfo Target;
            //public int TargetGroupPosition;
            //public int TargetMaidPosition;
            public MotionInfo MotionInfo;
            public ExtraManInfo ExtraManInfo;
            public OffsetInfo OffsetInfo;
            public PositionInfo PositionInfo;
            public TriggerSettingInfo TriggerSetting;
            public SetExtraManInfo SetExtraManInfo;
            public SetExtraManInfo RemoveExtraManInfo;
            public ManWalkSetupInfo ManWalkSetupInfo;
            public SetGroupMemberInfo SetGroupMemberInfo;
            public string NextStep;                
            public NextStepTriggerType NextStepTriggerType = NextStepTriggerType.None;
        }

        public class TargetInfo
        {
            public TargetType Type;
            public int TargetGroupPosition;
            public int TargetMaidPosition;
        }

        public enum StepActionType
        {
            ResetMotionToWaiting,
            LoadMotion,
            ResetIK,
            GetNextExtraMan,
            PositionOffset,
            SetPosition,
            Trigger,
            SetMaidToExtraMan,
            ManWalk,
            SetGroupMember,
            RemoveExtraMan
        }

        public enum TargetType
        {
            Group,              //The whole group
            GroupMaid,          //The maid in the group 
            GroupMan,           //The man in the group
            Maid,               //The maid in the WorkingMaid array
            Man,                //The man in the WorkingMan array
            TargetGroup,              //Target list stored in the processor
            TargetMaid              //Target list stored in the processor
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
            public bool IsSmoothAnimation = true;
        }

        public class PositionInfo
        {
            public TargetInfo Target;
            public string DefinedPointName;
            public bool IsSmoothAnimation = true;
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
            public TargetInfo Man1;
            public TargetInfo Man2;
            public TargetInfo Man3;
            public TargetInfo Maid1;
            public TargetInfo Maid2;
        }
    }
}
