using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.HardCodeMotion
{
    /*
     * This class tries to apply a walking motion to a man character
     **/


    internal class ManWalkController
    {
        //Key: GUID
        internal static Dictionary<string, DistanceTrigger> ManWalkDistanceTriggerList = new Dictionary<string, DistanceTrigger>();

        private const string WALKING_MOTION_FILE = "aruki_1_idou_f_once_.anm";

        private const float StartMovingTime = 0.735f;

        internal class StandingMotionType
        {
            internal string AnimationFileName;
            internal Vector3 StandingMotionOffset;
            internal Vector3 WalkingMotionOffset;
            internal Vector3 RotationOffset;

            public StandingMotionType()
            {
            }

            public StandingMotionType(string fileName, Vector3 standingOffset, Vector3 walkingOffset, Vector3 rotationOffset)
            {
                AnimationFileName = fileName;
                StandingMotionOffset = standingOffset;
                WalkingMotionOffset = walkingOffset;
                RotationOffset = rotationOffset;
            }
        }

        internal class DistanceTrigger
        {
            internal EventDelegate ToBeExecuted;
            internal float Distance;
            internal DateTime StartTime;
            internal Vector3 StartingVector;
        }


        internal static void MoveForward(Maid man, float distance, StandingMotionType motionType, EventDelegate postMoveAction = null)
        {
            Core.CharacterHandling.PlayAnimation(man, motionType.AnimationFileName, motionType.AnimationFileName);

            DistanceTrigger trigger = new DistanceTrigger();
            trigger.Distance = distance;
            trigger.ToBeExecuted = new EventDelegate(() => MoveForwardEnd(man, distance, motionType, postMoveAction));
            trigger.StartingVector = man.body0.Spine.transform.position;
            trigger.StartTime = DateTime.Now;

            //need to change the bone names to female before we apply the walking motion as the motion is female only
            Helper.BoneNameConverter.ConvertManStructureToFemale(man);

            //the orientation is different from standing motion and walking motion so we have to rotate it first
            man.transform.rotation = Quaternion.Euler(motionType.RotationOffset + man.transform.rotation.eulerAngles);

            //apply offset
            float angle = man.transform.rotation.eulerAngles.y;
            Vector3 offsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * (-motionType.StandingMotionOffset + motionType.WalkingMotionOffset);
            man.transform.position = man.transform.position + offsetRespectToRotation;

            Core.CharacterHandling.PlayAnimation(man, WALKING_MOTION_FILE, WALKING_MOTION_FILE);

            ManWalkDistanceTriggerList.Add(man.status.guid, trigger);
        }

        private static void MoveForwardEnd(Maid man, float distance, StandingMotionType motionType, EventDelegate postMoveAction)
        {
            float angle = man.transform.rotation.eulerAngles.y;
            Vector3 offsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * (new Vector3(0f, 0f, distance) + motionType.StandingMotionOffset - motionType.WalkingMotionOffset);

            man.transform.position = man.transform.position + offsetRespectToRotation;
            man.transform.rotation = Quaternion.Euler(-motionType.RotationOffset + man.transform.rotation.eulerAngles);

            Helper.BoneNameConverter.RecoverConvertedManStructure(man);
            Core.CharacterHandling.PlayAnimation(man, motionType.AnimationFileName, motionType.AnimationFileName);

            if (postMoveAction != null)
                postMoveAction.Execute();
        }

        internal static void CheckManWalkingDistanceTrigger(Maid man)
        {
            List<string> toBeKilled = new List<string>();
            foreach (var kvp in ManWalkDistanceTriggerList)
            {
                if (kvp.Key == man.status.guid)
                {
                    DistanceTrigger trigger = kvp.Value;

                    float currentDistance = Math.Abs(Vector3.Distance(man.body0.Spine.transform.position, trigger.StartingVector));

                    if (currentDistance >= trigger.Distance && DateTime.Now.Subtract(trigger.StartTime).TotalMilliseconds >= (StartMovingTime * 1000))
                    {
                        man.body0.StopAnime();
                        //have pass through destination
                        var delegateToExecute = trigger.ToBeExecuted;
                        toBeKilled.Add(man.status.guid);
                        delegateToExecute.Execute();
                    }
                }
            }
            foreach (var guid in toBeKilled)
                ManWalkDistanceTriggerList.Remove(guid);
            toBeKilled.Clear();
        }

    }
}

