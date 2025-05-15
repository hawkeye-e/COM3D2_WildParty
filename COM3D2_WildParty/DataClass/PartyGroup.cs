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
        public Maid Man3 = null;
        public string Maid1CurrentAudioFile = "";
        public string Maid2CurrentAudioFile = "";
        public Vector3 GroupPosition = Vector3.zero;
        public Vector3 GroupOffsetVector = Vector3.zero;            //from CharaAllOfsetPosPre
        public Vector3 GroupOffsetVector2 = Vector3.zero;           //from TagAllPos
        public Vector3 GroupRotationOffset = Vector3.zero;
        public Quaternion GroupRotation = Quaternion.identity;

        public int SexPosID = -1;
        public bool IsEstrus = false;
        public bool IsIndependentExcitement = true;
        public bool IsAutomatedGroup = true;
        public bool IsVoicelessGroup = false;
        public DateTime NextActionReviewTime = DateTime.MaxValue;   //For simulating the scene of the background group. If the current time pass this value, excitement rate will increase, etc...
        public int CurrentLabelGroupID = -1;
        public string CurrentScriptFileName;    //will be updated whenever loadmotionscript called
        public string CurrentLabelName;         //For group 0 used since setting the label is not in our control so we cant set the label group id defined by the mod directly
        public string CurrentSexState;          //Referencing the SexState class. Doesnt matter for group 0
        public MotionSpecialLabel CurrentOrgasmLabelRecord = null;
        public bool IsMaid1OrgasmScreamSet = false;
        public bool IsMaid2OrgasmScreamSet = false;

        private bool isRequireLabelChange = false;

        public bool RequireSmoothPositionChange = false;

        public bool ForceCharacterVisibleOnPositionChange = true;

        public bool BlockMotionScriptChange = false;                //A flag for blocking the system to change the motion script automatically when modded orgasm command is clicked

        public string CurrentMaid1AnimationClipName;                //The current animation playing. Using for getting the animation length.

        public string Maid1VoiceType = "";
        public string Maid2VoiceType = "";

        public static string CurrentFormation = "";
        public static Maid UnassignedMaid = null;   //For handling the special situation of main group being a CharacterEX personality and no man left in other group

        public ForceSexPosInfo ForceSexPos = null;
        public List<EyeSightSetting> ForceEyeSight = new List<EyeSightSetting>();
        public List<IKAttachInfo> ForceIKAttach = new List<IKAttachInfo>();

        public Dictionary<int, Vector3> MaidOffsetList = new Dictionary<int, Vector3>();
        public Dictionary<int, Vector3> ManOffsetList = new Dictionary<int, Vector3>();

        public Dictionary<string, GameObject> ExtraObjects = new Dictionary<string, GameObject>();
        public Dictionary<Maid, List<string>> ExtraCharacterObjects = new Dictionary<Maid, List<string>>();

        //Key: index
        public Dictionary<int, Maid> ExtraManList = new Dictionary<int, Maid>();
        public List<MapCoorindates.CoordinatesInfo> ExtraManSetupInfo = new List<MapCoorindates.CoordinatesInfo>();
        public List<int> MovingExtraManIndexList = new List<int>();
        public List<Maid> MovingGroupMemberList = new List<Maid>();

        //Key: index
        public static Dictionary<int, Maid> SharedExtraManList = new Dictionary<int, Maid>();
        public static List<MapCoorindates.CoordinatesInfo> SharedExtraManSetupInfo = new List<MapCoorindates.CoordinatesInfo>();
        public static ForceSexPosInfo.Type CurrentMainGroupMotionType = ForceSexPosInfo.Type.Waiting;

        public int CurrentExtraManIndex = 0;

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
                if (Man3 != null) count++;
                return count;
            }
        }

        public string GroupType
        {
            get
            {
                if (MaidCount == 2 && ManCount == 1) return Constant.GroupType.FFM;
                else if (MaidCount == 1 && ManCount == 2) return Constant.GroupType.MMF;
                else if (MaidCount == 1 && ManCount == 3) return Constant.GroupType.MMMF;
                else if (MaidCount == 1 && ManCount == 1) return Constant.GroupType.MF;
                else if (MaidCount == 2 && ManCount == 0) return Constant.GroupType.FF;
                else
                    return "";
            }

        }

        //This function takes the extra man into consideration. If there is no extra man defined in the scenario do not use this function.
        public List<string> GetPossibleGroupType()
        {
            List<string> result = new List<string>();
            if (MaidCount == 2)
                result.Add(Constant.GroupType.FFM);
            else if (MaidCount == 1)
            {
                result.Add(Constant.GroupType.MF);
                result.Add(Constant.GroupType.MMF);
                result.Add(Constant.GroupType.MMMF);
            }

            return result;
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
            SetCharacterPosition(Maid1, 0);
            SetCharacterPosition(Maid2, 1);
            SetCharacterPosition(Man1, 0);
            SetCharacterPosition(Man2, 1);
            SetCharacterPosition(Man3, 2);

            SetSharedExtraManPosition();

            //Do not enforce the extra man position if they are moving
            if (MovingExtraManIndexList.Count == 0)
                SetExtraManPosition();
        }

        public void SetExtraManPosition()
        {
            SetExtraManPosition(ExtraManList, ExtraManSetupInfo);
        }

        public void SetExtraManPosition(int position)
        {
            var setupInfo = ExtraManSetupInfo[position];

            Maid man = ExtraManList[setupInfo.ArrayPosition];

            if (man != null)
            {
                man.Visible = setupInfo.IsManVisible;
                if (setupInfo.IsManVisible)
                    man.transform.localScale = Vector3.one;
                else
                    man.transform.localScale = Vector3.zero;
                man.transform.localPosition = Vector3.zero;
                man.transform.position = setupInfo.Pos;
                man.transform.rotation = setupInfo.Rot;
                man.body0.SetBoneHitHeightY(setupInfo.Pos.y);

            }
        }

        public static void SetSharedExtraManPosition()
        {
            SetExtraManPosition(SharedExtraManList, SharedExtraManSetupInfo);
        }

        private static void SetExtraManPosition(Dictionary<int, Maid> extraManList, List<MapCoorindates.CoordinatesInfo> infoList)
        {
            if (infoList == null || extraManList == null)
                return;
            foreach (var setupInfo in infoList)
            {
                if (extraManList.ContainsKey(setupInfo.ArrayPosition))
                {
                    Maid man = extraManList[setupInfo.ArrayPosition];

                    if (man != null)
                    {
                        man.Visible = setupInfo.IsManVisible;
                        if (setupInfo.IsManVisible)
                            man.transform.localScale = Vector3.one;
                        else
                            man.transform.localScale = Vector3.zero;
                        man.transform.localPosition = Vector3.zero;
                        man.transform.position = setupInfo.Pos;
                        man.transform.rotation = setupInfo.Rot;
                        man.body0.SetBoneHitHeightY(setupInfo.Pos.y);
                    }
                }
            }
        }

        public Maid GetMaidAtIndex(int index)
        {
            if (index == 0)
                return Maid1;
            else if (index == 1)
                return Maid2;
            return null;
        }

        public Maid GetManAtIndex(int index)
        {
            if (index == 0)
                return Man1;
            else if (index == 1)
                return Man2;
            else if (index == 2)
                return Man3;
            return null;
        }

        public void SetMaidAtIndex(int index, Maid maid)
        {
            if (index == 0)
                Maid1 = maid;
            else if (index == 1)
                Maid2 = maid;
        }
        public void SetManAtIndex(int index, Maid maid)
        {
            if (index == 0)
                Man1 = maid;
            else if (index == 1)
                Man2 = maid;
            else if (index == 2)
                Man3 = maid;
        }

        public void SetCharacterPosition(Maid maid, int indexPosition)
        {
            if (maid != null)
            {
                if (MovingGroupMemberList.Contains(maid))
                    return;

                Vector3 individualOffset = GetIndividualOffset(maid.boMAN, indexPosition);

                Quaternion finalRotation = GroupRotation * Quaternion.Euler(Vector3.up * GroupRotationOffset.y);

                if (RequireSmoothPositionChange)
                {
                    Util.SmoothMoveMaidPosition(maid, GroupPosition + GroupOffsetVector + GroupOffsetVector2 + individualOffset, finalRotation);
                }
                else
                {
                    Util.StopSmoothMove(maid);
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = GroupPosition + GroupOffsetVector + GroupOffsetVector2 + individualOffset;
                    maid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    maid.transform.rotation = finalRotation;
                    maid.body0.SetBoneHitHeightY(GroupPosition.y);
                }

                if (ForceCharacterVisibleOnPositionChange)
                    maid.Visible = true;
            }
        }

        private Vector3 GetIndividualOffset(bool isMan, int indexPosition)
        {
            Vector3 individualOffset = Vector3.zero;
            Dictionary<int, Vector3> targetDict;
            if (isMan)
                targetDict = ManOffsetList;
            else
                targetDict = MaidOffsetList;

            if (targetDict.ContainsKey(indexPosition))
                individualOffset = targetDict[indexPosition];

            return individualOffset;
        }

        public void StopAudio()
        {
            Maid1.AudioMan.Stop();
            if (Maid2 != null)
                Maid2.AudioMan.Stop();
        }

        public void GenerateNextReviewTime()
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

        public void ReloadAnimation(bool IsSmooth = true)
        {
            ReloadAnimationForMaid(Maid1, IsSmooth);
            ReloadAnimationForMaid(Maid2, IsSmooth);
            ReloadAnimationForMaid(Man1, IsSmooth);
            ReloadAnimationForMaid(Man2, IsSmooth);
            ReloadAnimationForMaid(Man3, IsSmooth);
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


        private void ReloadAnimationForMaid(Maid maid, bool IsSmooth)
        {
            if (maid != null)
            {
                if (!string.IsNullOrEmpty(maid.body0.LastAnimeFN))
                {
                    float blendTime = IsSmooth ? 0.5f : 0f;
                    maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: true, boAddQue: false, blendTime);
                }
            }
        }

        public void DetachAllIK()
        {
            DetachAllIK(Maid1);
            DetachAllIK(Maid2);
            DetachAllIK(Man1);
            DetachAllIK(Man2);
            DetachAllIK(Man3);
        }

#if COM3D2_5
#if UNITY_2022_3
        private void DetachAllIK(Maid maid)
        {
            if (maid != null)
                maid.body0.fullBodyIK.AllIKDetach();
        }
#endif
#endif

#if COM3D2
        private void DetachAllIK(Maid maid)
        {
            if (maid != null)
                maid.AllIKDetach();
        }
#endif

        public static List<int> GetExtraManEmptySpotList()
        {
            List<int> list = new List<int>();

            foreach (var kvp in SharedExtraManList)
            {
                if (kvp.Value == null)
                    list.Add(kvp.Key);
            }

            return list;
        }

        public void ProcAllProp()
        {
            ProcProp(Maid1);
            ProcProp(Maid2);
            ProcProp(Man1);
            ProcProp(Man2);
            ProcProp(Man3);
        }

        public static void ProcProp(Maid maid)
        {
            if (maid != null)
                maid.AllProcProp();
        }

        public int GetMaidOrManIndex(Maid maid)
        {
            int result = -1;
            if (maid.boMAN)
            {
                for (int i = 0; i < ManCount; i++)
                    if (GetManAtIndex(i).status.guid == maid.status.guid)
                        result = i;
            }
            else
            {
                for (int i = 0; i < MaidCount; i++)
                    if (GetMaidAtIndex(i).status.guid == maid.status.guid)
                        result = i;
            }
            return result;
        }

        public Maid GetCurrentExtraMaid(bool isIncrementIndexAfterDone = true)
        {
            Maid result;
            while (true)
            {
                if (!ExtraManList.ContainsKey(CurrentExtraManIndex))
                    CurrentExtraManIndex = 0;

                if (ExtraManList[CurrentExtraManIndex] != null)
                {
                    result = ExtraManList[CurrentExtraManIndex];
                    if (isIncrementIndexAfterDone)
                        CurrentExtraManIndex++;
                    break;
                }

                CurrentExtraManIndex++;
            }

            return result;
        }

        //For debug use
        public override string ToString()
        {
            string output = "";
            if (Maid1 != null) output += "Maid1: " + Maid1.status.fullNameJpStyle + ", ";
            if (Maid2 != null) output += "Maid2: " + Maid2.status.fullNameJpStyle + ", ";
            if (Man1 != null) output += "Man1: " + Man1.status.fullNameJpStyle + ", ";
            if (Man2 != null) output += "Man2: " + Man2.status.fullNameJpStyle + ", ";
            if (Man3 != null) output += "Man3: " + Man3.status.fullNameJpStyle + ", ";
            return output;
        }



    }
}
