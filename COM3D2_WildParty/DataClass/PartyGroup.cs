using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    internal class PartyGroup
    {
        public Maid Maid1;
        public Maid Maid2;
        public Maid Man1 = null;
        public Maid Man2 = null;
        public string Maid1CurrentAudioFile = "";
        public string Maid2CurrentAudioFile = "";
        public Vector3 GroupPosition = Vector3.zero;
        public Vector3 GroupOffsetVector = Vector3.zero;
        public Quaternion GroupRotation = Quaternion.identity;

        public int SexPosID;
        public bool IsEstrus = false;
        public bool IsIndependentExcitement = true;
        public bool IsAutomatedGroup = true;
        public DateTime NextActionReviewTime = DateTime.MaxValue;   //For simulating the scene of the background group. If the current time pass this value, excitement rate will increase, etc...
        public int CurrentLabelGroupID = -1;
        public string CurrentLabelName;         //For group 0 used since setting the label is not in our control so we cant set the label group id defined by the mod directly
        public string CurrentSexState;          //Referencing the SexState class. Doesnt matter for group 0
        public MotionSpecialLabel CurrentOrgasmLabelRecord = null;
        public bool IsMaid1OrgasmScreamSet = false;
        public bool IsMaid2OrgasmScreamSet = false;

        private bool isRequireLabelChange = false;

        public bool RequireSmoothPositionChange = false;

        public string CurrentMaid1AnimationClipName;                //The current animation playing. Using for getting the animation length.

        public string Maid1VoiceType = "";
        public string Maid2VoiceType = "";

        public static string CurrentFormation = "";
        public static Maid UnassignedMaid = null;   //For handling the special situation of main group being a CharacterEX personality and no man left in other group

        public ForceSexPosInfo ForceSexPos = null;
        public List<EyeSightSetting> ForceEyeSight = new List<EyeSightSetting>();
        public List<IKAttachInfo> ForceIKAttach = new List<IKAttachInfo>();

        public static ForceSexPosInfo.Type CurrentMainGroupMotionType = ForceSexPosInfo.Type.Waiting;
        public PartyGroup() { }


        public int MaidCount
        {
            get
            {
                int count = 1;
                if (Maid2 != null) count++;
                return count;
            }
        }

        public int ManCount
        {
            get
            {
                int count = 0;
                if (Man1 != null) count++;
                if (Man2 != null) count++;
                return count;
            }
        }

        public string GroupType
        {
            get
            {
                if (MaidCount == 2 && ManCount == 1) return Constant.GroupType.FFM;
                else if (MaidCount == 1 && ManCount == 2) return Constant.GroupType.MMF;
                else if (MaidCount == 1 && ManCount == 1) return Constant.GroupType.MF;
                else
                    return "";
            }
            
        }

        public int ExcitementRate
        {
            get
            {
                return Maid1.status.currentExcite;
            }
        }

        public int ExcitementLevel
        {
            get
            {
                if (ExcitementRate < 0)
                    return 0;
                else if (ExcitementRate < 100)
                    return 1;
                else if (ExcitementRate < 200)
                    return 2;
                else
                    return 3;
            }
        }

        public int SensualRate
        {
            get { return Maid1.status.currentSensual; }
            set { Maid1.status.currentSensual = value; }
        }

        public void SetGroupPosition(Vector3 pos, Quaternion rot)
        {
            GroupPosition = pos;
            GroupRotation = rot;
            SetGroupPosition();
        }

        public void SetGroupPosition()
        {
            SetCharacterPosition(Maid1);
            SetCharacterPosition(Maid2);
            SetCharacterPosition(Man1);
            SetCharacterPosition(Man2);
        }

        private void SetCharacterPosition(Maid maid)
        {
            if (maid != null)
            {
                if (RequireSmoothPositionChange)
                {
                    Util.SmoothMoveMaidPosition(maid, GroupPosition + GroupOffsetVector, GroupRotation);
                }
                else
                {
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = GroupPosition + GroupOffsetVector;
                    maid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    maid.transform.rotation = GroupRotation;
                }
                maid.Visible = true;
            }
        }

        public void StopAudio()
        {
            Maid1.AudioMan.Stop();
            if (Maid2 != null)
                Maid2.AudioMan.Stop();
        }

        public  void GenerateNextReviewTime()
        {
            NextActionReviewTime = DateTime.Now.AddSeconds(
                        //RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinBackgroundGroupReviewTimeInSeconds, ConfigurableValue.YotogiSimulation.MaxBackgroundGroupReviewTimeInSeconds)
                        RNG.Random.Next(Config.MinBackgroundGroupReviewTimeInSeconds, Config.MaxBackgroundGroupReviewTimeInSeconds)
                        );
        }

        public void GenerateNextReviewTime(int second)
        {
            NextActionReviewTime = DateTime.Now.AddSeconds(RNG.Random.Next(second));
        }

        public void StopNextReviewTime()
        {
            NextActionReviewTime = DateTime.MaxValue;
        }

        public void RequestNextReviewTimeAfter(float second)
        {
            NextActionReviewTime = DateTime.Now.AddMilliseconds(second * 1000);
        }

        public void ReloadAnimation()
        {
            ReloadAnimationForMaid(Maid1);
            ReloadAnimationForMaid(Maid2);
            ReloadAnimationForMaid(Man1);
            ReloadAnimationForMaid(Man2);
        }

        public bool IsChangeMotionLabel()
        {
            if (isRequireLabelChange)
            {
                isRequireLabelChange = false;
                return true;
            }

            return (RNG.Random.Next(100) < Config.ChangeMotionRateInPercentage);
        }

        public void RequestMotionLabelChange()
        {
            isRequireLabelChange = true;
        }
        

        private void ReloadAnimationForMaid(Maid maid)
        {
            if (maid != null)
            {
                if (!string.IsNullOrEmpty(maid.body0.LastAnimeFN))
                {
                    maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: true, boAddQue: false, 0f);
                }
            }
        }

        public void DetachAllIK()
        {
            DetachAllIK(Maid1);
            DetachAllIK(Maid2);
            DetachAllIK(Man1);
            DetachAllIK(Man2);
        }

        private void DetachAllIK(Maid maid)
        {
            if (maid != null)
                maid.body0.fullBodyIK.AllIKDetach();
        }

        //For debug use
        public override string ToString()
        {
            string output = "";
            if (Maid1 != null) output += "Maid1: " + Maid1.status.fullNameJpStyle + ", ";
            if (Maid2 != null) output += "Maid2: " + Maid2.status.fullNameJpStyle + ", ";
            if (Man1 != null) output += "Man1: " + Man1.status.fullNameJpStyle + ", ";
            if (Man2 != null) output += "Man2: " + Man2.status.fullNameJpStyle + ", ";
            return output;
        }



    }
}
