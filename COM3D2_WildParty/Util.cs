using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using System.IO;

namespace COM3D2.WildParty.Plugin
{
    class Util
    {
        internal static GameObject InstantiateFromBundle(AssetBundle bundle, string assetName)
        {
            var asset = bundle.LoadAsset(assetName, typeof(GameObject));
            //var obj = GameObject.Instantiate(asset, parent, instantiateInWorldSpace);
            var obj = GameObject.Instantiate(asset);
            foreach (var rootGameObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (rootGameObject.GetInstanceID() == obj.GetInstanceID())
                {
                    rootGameObject.name = assetName;
                    return rootGameObject;
                }
            }
            throw new FileLoadException("Could not instantiate asset " + assetName);
        }

        public static GameObject FindInActiveObjectByName(string name)
        {
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].hideFlags == HideFlags.None)
                {
                    if (objs[i].name == name)
                    {
                        return objs[i].gameObject;
                    }
                }
            }
            return null;
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public static string GetPersonalityNameByValue(int value)
        {
            switch (value)
            {
                case Constant.PersonalityType.Muku:
                    return nameof(Constant.PersonalityType.Muku);
                case Constant.PersonalityType.Majime:
                    return nameof(Constant.PersonalityType.Majime);
                case Constant.PersonalityType.Rindere:
                    return nameof(Constant.PersonalityType.Rindere);
                case Constant.PersonalityType.Pure:
                    return nameof(Constant.PersonalityType.Pure);
                case Constant.PersonalityType.Pride:
                    return nameof(Constant.PersonalityType.Pride);
                case Constant.PersonalityType.Cool:
                    return nameof(Constant.PersonalityType.Cool);
                case Constant.PersonalityType.Yandere:
                    return nameof(Constant.PersonalityType.Yandere);
                case Constant.PersonalityType.Anesan:
                    return nameof(Constant.PersonalityType.Anesan);
                case Constant.PersonalityType.Genki:
                    return nameof(Constant.PersonalityType.Genki);
                case Constant.PersonalityType.Sadist:
                    return nameof(Constant.PersonalityType.Sadist);
                case Constant.PersonalityType.Silent:
                    return nameof(Constant.PersonalityType.Silent);
                case Constant.PersonalityType.Devilish:
                    return nameof(Constant.PersonalityType.Devilish);
                case Constant.PersonalityType.Ladylike:
                    return nameof(Constant.PersonalityType.Ladylike);
                case Constant.PersonalityType.Secretary:
                    return nameof(Constant.PersonalityType.Secretary);
                case Constant.PersonalityType.Sister:
                    return nameof(Constant.PersonalityType.Sister);
                case Constant.PersonalityType.Curtness:
                    return nameof(Constant.PersonalityType.Curtness);
                case Constant.PersonalityType.Missy:
                    return nameof(Constant.PersonalityType.Missy);
                case Constant.PersonalityType.Childhood:
                    return nameof(Constant.PersonalityType.Childhood);

                case Constant.PersonalityType.Masochist:
                    return nameof(Constant.PersonalityType.Masochist);
                case Constant.PersonalityType.Cunning:
                    return nameof(Constant.PersonalityType.Cunning);
                case Constant.PersonalityType.Friendly:
                    return nameof(Constant.PersonalityType.Friendly);
                case Constant.PersonalityType.Dame:
                    return nameof(Constant.PersonalityType.Dame);
                case Constant.PersonalityType.Gyaru:
                    return nameof(Constant.PersonalityType.Gyaru);
                default:
                    return "Unknown";
            }
        }

        //In case we need logic to handle the order of the name due to localization
        internal static string GetMaidDisplayName(Maid maid)
        {
            //TODO: The code by KISS load name like this, does it already handle the localization case to put first name first?
            return maid.status.charaName.GetFullName().Trim();
        }

        internal static void SmoothMoveMaidPosition(Maid maid, Vector3 targetPosition, Quaternion targetRotation, float time = -1)
        {
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("position", targetPosition);
            args.Add("rotation", targetRotation.eulerAngles);
            args.Add("scale", maid.transform.localScale);
            if(time > 0)
                args.Add("time", time);
            iTween.MoveTo(maid.gameObject, args);

            System.Collections.Hashtable argsRotate = new System.Collections.Hashtable();
            argsRotate.Add("position", targetPosition);
            argsRotate.Add("rotation", targetRotation.eulerAngles);
            argsRotate.Add("scale", maid.transform.localScale);
            if (time > 0)
                argsRotate.Add("time", time);
            iTween.RotateTo(maid.gameObject, argsRotate);
        }

        internal static void StopSmoothMove(Maid maid)
        {
            iTween.Stop(maid.gameObject);
        }

        public static void ResetAllGroupPosition()
        {
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var group = StateManager.Instance.PartyGroupList[i];
                group.SetGroupPosition();
            }

            if (!string.IsNullOrEmpty(PartyGroup.CurrentFormation))
            {
                MapCoorindates coordsGroup = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];

                //Pick the one that fit the most group
                MapCoorindates.CoordinateListInfo coords = coordsGroup.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();

                //Set individual position
                if (coords.IndividualCoordinates != null)
                {
                    foreach (var item in coords.IndividualCoordinates)
                    {
                        Maid targetMaid = null;
                        if (item.Type == Constant.IndividualCoordinateType.Maid)
                        {
                            if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                                targetMaid = StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition];
                        }
                        else if (item.Type == Constant.IndividualCoordinateType.Man)
                        {
                            if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                                targetMaid = StateManager.Instance.YotogiWorkingManList[item.ArrayPosition];
                        }
                        else if (item.Type == Constant.IndividualCoordinateType.NPCMale)
                        {
                            if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                                targetMaid = StateManager.Instance.NPCManList[item.ArrayPosition];
                        }
                        else if (item.Type == Constant.IndividualCoordinateType.NPCFemale)
                        {
                            if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                                targetMaid = StateManager.Instance.NPCList[item.ArrayPosition];
                        }
                        else if (item.Type == Constant.IndividualCoordinateType.Owner)
                            targetMaid = StateManager.Instance.ClubOwner;


                        if (targetMaid != null)
                        {
                            targetMaid.transform.localPosition = Vector3.zero;
                            targetMaid.transform.position = item.Pos;
                            targetMaid.transform.rotation = item.Rot;
                            targetMaid.body0.SetBoneHitHeightY(item.Pos.y);
                        }
                    }
                }

                //Set special position
                if (coordsGroup.SpecialCoordinates != null)
                {
                    foreach (var item in coordsGroup.SpecialCoordinates)
                    {
                        if (item.Type == Constant.SpecialCoordinateType.Owner)
                        {
                            StateManager.Instance.ClubOwner.transform.localPosition = Vector3.zero;
                            StateManager.Instance.ClubOwner.transform.position = item.Pos;
                            StateManager.Instance.ClubOwner.transform.rotation = item.Rot;
                            StateManager.Instance.ClubOwner.body0.SetBoneHitHeightY(item.Pos.y);
                        }
                        else if (item.Type == Constant.SpecialCoordinateType.UnassignedMaid)
                        {
                            if (PartyGroup.UnassignedMaid != null)
                            {
                                PartyGroup.UnassignedMaid.transform.localPosition = Vector3.zero;
                                PartyGroup.UnassignedMaid.transform.position = item.Pos;
                                PartyGroup.UnassignedMaid.transform.rotation = item.Rot;
                                PartyGroup.UnassignedMaid.body0.SetBoneHitHeightY(item.Pos.y);
                            }
                        }
                    }
                }
            }
        }

        public static void SetAllMaidVisiblility(bool isVisible)
        {
            foreach(Maid maid in StateManager.Instance.SelectedMaidsList)
                maid.Visible = isVisible;
        }

        public static void SetAllManVisiblility(bool isVisible)
        {
            foreach (Maid man in StateManager.Instance.MenList)
                man.Visible = isVisible;
        }

        public static void SetAllPartyMemberVisibility(bool isVisible)
        {
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                if (StateManager.Instance.PartyGroupList[i].Maid1 != null)
                    StateManager.Instance.PartyGroupList[i].Maid1.Visible = isVisible;
                if (StateManager.Instance.PartyGroupList[i].Maid2 != null)
                    StateManager.Instance.PartyGroupList[i].Maid2.Visible = isVisible;
                if (StateManager.Instance.PartyGroupList[i].Man1 != null)
                    StateManager.Instance.PartyGroupList[i].Man1.Visible = isVisible;
                if (StateManager.Instance.PartyGroupList[i].Man2 != null)
                    StateManager.Instance.PartyGroupList[i].Man2.Visible = isVisible;
                if (StateManager.Instance.PartyGroupList[i].Man3 != null)
                    StateManager.Instance.PartyGroupList[i].Man3.Visible = isVisible;
            }
        }

        public static PartyGroup GetPartyGroupByCharacter(Maid chara)
        {
            if (chara == null)
                return null;

            foreach (var group in StateManager.Instance.PartyGroupList)
            {
                if (group.Maid1.status.guid == chara.status.guid || group.Man1?.status.guid == chara.status.guid
                    || group.Maid2?.status.guid == chara.status.guid || group.Man2?.status.guid == chara.status.guid
                    || group.Man3?.status.guid == chara.status.guid
                    )
                    return group;
            }
            return null;
        }

        internal static PartyGroup GetPartyGroupByGUID(string guid)
        {
            if (StateManager.Instance.PartyGroupList == null)
                return null;
            if (StateManager.Instance.PartyGroupList.Count == 0)
                return null;

            //first group case
            if (guid == "")
                return StateManager.Instance.PartyGroupList[0];

            foreach (var group in StateManager.Instance.PartyGroupList)
            {
                if (group.Maid1.status.guid == guid || group.Man1?.status.guid == guid
                    || group.Maid2?.status.guid == guid || group.Man2?.status.guid == guid
                     || group.Man3?.status.guid == guid
                    )
                    return group;
            }
            return null;
        }

        internal static MapCoorindates.CoordinatesInfo GetGroupCoordinateInfo(PartyGroup group)
        {
            //locate the group index
            int groupIndex = -1;
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
                if (StateManager.Instance.PartyGroupList[i] == group)
                    groupIndex = i;

            if (groupIndex < 0)
                return null;

            MapCoorindates.CoordinateListInfo coordinateListInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            MapCoorindates.CoordinatesInfo coordinatesInfo = coordinateListInfo.GroupCoordinates.Where(x => x.ArrayPosition == groupIndex).First();
            return coordinatesInfo;
        }

        internal static int GetIndexPositionInWorkingYotogiArrayForMaid(Maid maid)
        {
            if (maid == null)
                return -1;

            for (int i = 0; i < StateManager.Instance.YotogiWorkingMaidList.Count; i++)
                if (StateManager.Instance.YotogiWorkingMaidList[i].status.guid == maid.status.guid)
                {
                    return i;
                }

            return -1;
        }

        internal static PlayableSkill.SkillItem GetGroupCurrentSkill(PartyGroup group)
        {
            if (StateManager.Instance.PartyGroupList.Count > 0)
            {
                return ModUseData.ValidSkillList[group.Maid1.status.personal.id][group.GroupType].Where(x => x.SexPosID == group.SexPosID).First();
            }
            return null;
        }

        internal static PlayableSkill.SkillItem GetGroupSkillIDBySexPosID(PartyGroup group, int sexPosID)
        {
            int personality = group.Maid1.status.personal.id;
            string groupType = group.GroupType;
            return ModUseData.ValidSkillList[personality][groupType].Where(x => x.SexPosID == sexPosID).First();
        }

        internal static string GetSkillGroupType(PartyGroup group, string skillID)
        {
            int personality = group.Maid1.status.personal.id;
            foreach(var groupDictionary in ModUseData.ValidSkillList[personality])
            {
                foreach (var item in groupDictionary.Value)
                    if (item.YotogiSkillID == skillID)
                        return groupDictionary.Key;
            }
            return "";
        }

        internal static int GetGroupTypeMemberCount(string groupType, bool isMan)
        {
            char toCheck = isMan? 'M':'F';
            return groupType.Count(x => x == toCheck);
        }

        internal static Maid SearchManCharacterByGUID(string GUID)
        {
            //check also the club owner
            if (StateManager.Instance.ClubOwner != null)
                if (StateManager.Instance.ClubOwner.status.guid == GUID)
                {
                    return StateManager.Instance.ClubOwner;
                }
            //man list
            if (StateManager.Instance.MenList != null)
                foreach (Maid man in StateManager.Instance.MenList)
                    if (man.status.guid == GUID)
                    {
                        return man;
                    }
            //npc list
            if (StateManager.Instance.NPCManList != null)
                foreach (Maid npc in StateManager.Instance.NPCManList)
                    if (npc.status.guid == StateManager.Instance.processingManGUID)
                    {
                        return npc;
                    }

            return null;
        }

        //Use Constant.TargetType
        internal static List<Maid> GetTargetList(string targetType)
        {
            if (targetType == Constant.TargetType.SingleMaid)
                return StateManager.Instance.SelectedMaidsList;
            else if (targetType == Constant.TargetType.SingleMan)
                return StateManager.Instance.MenList;
            else if (targetType == Constant.TargetType.NPCFemale)
                return StateManager.Instance.NPCList;
            else if (targetType == Constant.TargetType.NPCMale)
                return StateManager.Instance.NPCManList;

            return null;
        }

        internal static bool IsManAConvertedMaid(Maid man)
        {
            return Helper.BoneNameConverter.IsMaidConvertedMan(man);
        }

        internal static bool IsExPackPersonality(Maid maid)
        {
            return Constant.EXPackPersonality.Contains(maid.status.personal.id);
        }

        internal static int GetCurrentDefaultSexPosID()
        {
            int result = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].DefaultSexPosID;
            if(ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].SpecialCaseDefaultSexPosIDList != null)
            {
                var specialCase = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].SpecialCaseDefaultSexPosIDList.Where(x => x.Personality.Contains(StateManager.Instance.SelectedMaidsList[0].status.personal.id)).FirstOrDefault();

                if (specialCase != null)
                    result = specialCase.DefaultSexPosID;
            }

            return result;
        }
        
        internal static BackupParamAccessor.Params GetBackupParam(Maid maid, bool isDaytime)
        {
            int slotIndex = 0;
            for (int i = 0; i < BackupParamAccessor.BackupParamAccessor.slots.Length; i++)
            {
                if (BackupParamAccessor.BackupParamAccessor.slots[i].maid_guid == maid.status.guid)
                {
                    slotIndex = i;
                    break;
                }
            }

            BackupParamAccessor.SCENE_ID timeBeforeHappen;
            if (isDaytime)
                timeBeforeHappen = BackupParamAccessor.SCENE_ID.Morning;
            else
                timeBeforeHappen = BackupParamAccessor.SCENE_ID.NoonBonus;

            return BackupParamAccessor.BackupParamAccessor.GetBackupParams(slotIndex, timeBeforeHappen);
        }

        internal static void ForceGroupPosition()
        {
            foreach (var group in StateManager.Instance.PartyGroupList)
                group.SetGroupPosition();
        }

        internal static void ClearGenericCollection<T>(List<T> list)
        {
            if (list != null)
                list.Clear();
            list = null;
        }

        internal static void ClearGenericCollection<TKey, TValue>(Dictionary<TKey,TValue> list)
        {
            if (list != null)
                list.Clear();
            list = null;
        }

        /*
         * Determine formular: BaseRate + ((CurrentExcite - MinExciteRequirement) ^ 2 / 100) , cap at value set in config file
         * Justification: want to have a exponential curve so that the group could stay a bit longer in average in the level of 200-300 
         */
        internal static bool IsBackgroundMaidOrgasm(Maid maid)
        {
            //if (maid.status.currentExcite < ConfigurableValue.YotogiSimulation.MinOrgasmExcitementRate)
            if (maid.status.currentExcite < Config.MinOrgasmExcitementRate)
                return false;

            //int rate = ConfigurableValue.YotogiSimulation.BaseOrgasmChanceInPercentage + ((int)Math.Pow(maid.status.currentExcite - ConfigurableValue.YotogiSimulation.MinOrgasmExcitementRate, 2) / 100);
            //rate = Math.Min(rate, ConfigurableValue.YotogiSimulation.OrgasmChanceCapInPercentage);
            int rate = Config.BaseOrgasmChanceInPercentage + ((int)Math.Pow(maid.status.currentExcite - Config.MinOrgasmExcitementRate, 2) / 100);
            rate = Math.Min(rate, Config.OrgasmChanceCapInPercentage);

            return RNG.Random.Next(100) < rate;
        }

        internal static int GetRandomExcitementRate()
        {
            //return RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinInitialRandomExciteValue, ConfigurableValue.YotogiSimulation.MaxInitialRandomExciteValue);
            return RNG.Random.Next(Config.MinInitialRandomExciteValue, Config.MaxInitialRandomExciteValue);
        }

        internal static bool IsExcitementLevelChanged(int oldValue, int newValue)
        {
            return (oldValue / 100) != (newValue / 100);
        }


        internal static BackgroundGroupMotion.MotionItem GetMotionItemOfGroup(PartyGroup group)
        {
            if (group == null)
                return null;
            return ModUseData.BackgroundMotionList[group.GroupType].Where(x => x.ID == group.SexPosID).First();
        }

        internal static BackgroundGroupMotion.MotionItem GetMotionItemBySexPosID(int sexPosID)
        {
            foreach(var kvp in ModUseData.BackgroundMotionList)
            {
                var list = kvp.Value.Where(x => x.ID == sexPosID).ToList();
                if(list.Count > 0)
                    return list[0];
            }
            return null;
        }

        internal static Maid GetSemenTarget(PartyGroup group, MotionSpecialLabel.SemenTarget target)
        {
            switch (target)
            {
                case MotionSpecialLabel.SemenTarget.Maid1:
                    return group.Maid1;
                case MotionSpecialLabel.SemenTarget.Maid2:
                    return group.Maid2;
                default:
                    return null;
            }
        }

        internal static MotionSpecialLabel GetCurrentMotionSpecialLabel(PartyGroup group, string labelName)
        {
            var motionItem = GetMotionItemOfGroup(group);
            //check if it is special label first
            var specialLabelMatch = motionItem.SpecialLabels.Where(x => x.Label == labelName).ToList();
            if(specialLabelMatch.Count > 0)
            {
                return specialLabelMatch.First();
            }
            else
            {
                //if no match, it is normal play type
                return null;
            }

            
        }

        internal static List<BackgroundGroupMotion.MotionLabel> GetCurrentMotionLabel(PartyGroup group, string labelName)
        {
            var motionItem = GetMotionItemOfGroup(group);
            var motionLabels = motionItem.ValidLabels.Where(x => x.ID == group.SexPosID && x.LabelName == labelName && x.ExcitementLevel == group.ExcitementLevel).ToList();
            if(motionLabels.Count > 0)
            {
                return motionLabels;
            }
            else
            {
                return null;
            }
        }

        internal static int GetFetishIDByButtonID(string buttonID)
        {
            if (!ModUseData.ExtraYotogiCommandDataList.ContainsKey(buttonID))
                return -1;
            else
                return ModUseData.ExtraYotogiCommandDataList[buttonID].FetishID;
        }

        internal static Scenario GetUndergoingScenario()
        {
            return ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
        }

        //From WfCameraMoveSupport class
        internal static float RoundDegree(float degres)
        {
            if (!(0f <= degres) || !(degres <= 360f))
            {
                degres -= Mathf.Floor(degres / 360f) * 360f;
                return (!Mathf.Approximately(360f, degres)) ? degres : 0f;
            }
            return Mathf.Abs(degres);
        }

        public static bool NearlyEquals(Vector3 v1, Vector3 v2, float unimportantDifference = 0.0001f)
        {
            if(v1 != v2)
            {
                return NearlyEquals(v1.x, v2.x) && NearlyEquals(v1.y, v2.y) && NearlyEquals(v1.z, v2.z);
            }

            return true;
        }

        public static bool NearlyEquals(Vector2 v1, Vector2 v2, float unimportantDifference = 0.0001f)
        {
            if (v1 != v2)
            {
                return NearlyEquals(v1.x, v2.x) && NearlyEquals(v1.y, v2.y);
            }

            return true;
        }

        public static bool NearlyEquals(float value1, float value2, float unimportantDifference = 0.0001f)
        {
            if (value1 != value2)
            {
                return Math.Abs(value1 - value2) < unimportantDifference;
            }

            return true;
        }

        public static Vector2 ParseVector2RawString(string vectorInString)
        {
            var split = vectorInString.Split(',');
            return new Vector2(float.Parse(split[0].Trim()), float.Parse(split[1].Trim()));
        }

        public static Vector3 ParseVector3RawString(string vectorInString)
        {
            var split = vectorInString.Split(',');
            return new Vector3(float.Parse(split[0].Trim()), float.Parse(split[1].Trim()), float.Parse(split[2].Trim()));
        }

        public static Quaternion ParseQuaternionRawString(string quaternionInString)
        {
            var split = quaternionInString.Split(',');
            return new Quaternion(float.Parse(split[0].Trim()), float.Parse(split[1].Trim()), float.Parse(split[2].Trim()), float.Parse(split[3].Trim()));
        }
    }
}
