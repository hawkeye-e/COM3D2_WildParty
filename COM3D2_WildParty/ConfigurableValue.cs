using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    internal class ConfigurableValue
    {
        internal const int MaxOrgyPartyManCount = 20;        //While it is possible to add more so that you could have like 40 maids + 80 men, it would be slow as hell to load...
        internal const float CameraFadeTime = 0.5f;
        internal const int OrgasmFinishFollowUpBaseExtraWaitingTimeInSecond = 2;
        internal const int OrgasmFinishFollowUpVariableExtraWaitingTimeInSecond = 3;

        internal const int GangBangReQueueMinLengthDiffLimit = 2;       //When a man is trying to requeue after orgasm, if the minimum length of all queues is X, the man is allowed to join a queue if its length <= X + (this value)
        internal const int ReplacedOrgasmWaitBufferTime = 2;
        internal const float AnimationBlendTime = 0.5f;

        internal static class YotogiSimulation
        {
            //Values of this class will be included in the config to let the player edit. This class will act as default suggested value.

            //Valid Excite value: -100 to 300
            internal const int MaxInitialRandomExciteValue = 200;
            internal const int MinInitialRandomExciteValue = -100;

            //We dont want all the group updating the excitement rate or change position etc at the same time which will make it looks extermely weird. Will pick a random value between these 2 values for each group.
            internal const int MaxBackgroundGroupReviewTimeInSeconds = 10;
            internal const int MinBackgroundGroupReviewTimeInSeconds = 5;

            //Serve the purpose of simulating clicking a command during yotogi. Will pick a random value between these 2 values and add to the maid's current excitement rate.
            internal const int MaxExcitementRateIncrease = 15;
            internal const int MinExcitementRateIncrease = 5;

            //Serve the purpose of simulating clicking a command during yotogi. Will pick a random value between these 2 values and add to the maid's current sensual rate if not in estrus mode.
            internal const int MaxSensualRateIncrease = 15;
            internal const int MinSensualRateIncrease = 5;

            //Serve the purpose of simulating the sensual decay when in estrus mode. Will pick a random value between these 2 values and decrease the maid's current sensual rate if in estrus mode.
            internal const int MaxSensualRateDecay = 20;
            internal const int MinSensualRateDecay = 10;

            //If the maid's current excitement rate pass this number, orgasm could occur
            internal const int MinOrgasmExcitementRate = 200;

            //For the calculation of the chance of orgasm in the current excitement level
            internal const int BaseOrgasmChanceInPercentage = 10;
            internal const int OrgasmChanceCapInPercentage = 75;

            //For some sex positions there are several kind of motion (eg: piston, kiss + piston, grab the boobs + piston etc ),
            //this number used to determine if the group should switch into another motion label under the same position to make it looks more realistic
            internal const int ChangeMotionRateInPercentage = 50;

            //How long can a group wait (to have a little rest) after orgasm
            internal const int MaxTimeToResumeSexAfterOrgasm = 5;
        }
        

        internal static class YotogiCameraWork
        {
            internal static readonly Vector3 CameraVectorOffset = new Vector3(3f, 2f, 2f);
            internal static readonly Vector3 TargetVectorOffset = Vector3.zero;
            internal const float CameraDistance = 4f;
            //internal static readonly Vector2 CameraAngle = new Vector2(230f, 32f);

            internal const float DefaultHorizontalRotation = 225f;
            internal const float DefaultVerticalRotation = 25f;
            //internal const float DefaultSidewardRotation = 0f;
        }


    }
}