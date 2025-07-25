using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.CharacterManager
{
    internal class Patches
    {
        internal static void GetSpoofMan(int manNo, ref Maid result)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                PartyGroup targetGroup = null;
                if (StateManager.Instance.IsMainGroupMotionScriptFlag)
                {
                    targetGroup = StateManager.Instance.PartyGroupList[0];
                }
                else
                {
                    foreach (var group in StateManager.Instance.PartyGroupList)
                    {
                        if (group.Man1?.status.guid == StateManager.Instance.processingManGUID)
                        {
                            targetGroup = group;
                        }
                    }
                }

                if (targetGroup != null)
                {
                    if (targetGroup.SexPosID >= 0)
                    {
                        //If it is a hit, determine which man in the group is returned
                        BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(targetGroup.SexPosID);

                        for (int i = 0; i < motionItem.ManIndex.Count; i++)
                            if (motionItem.ManIndex[i] == manNo)
                            {
                                result = targetGroup.GetManAtIndex(i);
                                return;
                            }
                    }
                    else
                    {
                        //For the case of it is not a yotogi motion
                        result = targetGroup.GetManAtIndex(manNo);
                        return;
                    }
                }

                Maid searchResult = Util.SearchManCharacterByGUID(StateManager.Instance.processingManGUID);
                if (searchResult != null)
                    result = searchResult;

            }
        }

        internal static void GetSpoofMaid(int maidNo, ref Maid result)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.PartyGroupList.Count > 0)
                {
                    if (PartyGroup.UnassignedMaid != null)
                        //Special case handling for temporary unassigned maid (no ffm case for Ex class maid so put the extra one to unassigned)
                        if (StateManager.Instance.processingMaidGUID == PartyGroup.UnassignedMaid.status.guid)
                        {
                            result = PartyGroup.UnassignedMaid;
                            return;
                        }

                    foreach (var group in StateManager.Instance.PartyGroupList)
                    {
                        if (group.Maid1?.status.guid == StateManager.Instance.processingMaidGUID)
                        {
                            //If it is a hit, determine which maid in the group is returned
                            if (maidNo == 0)
                                result = group.Maid1;
                            else if (maidNo == 1)
                                result = group.Maid2;
                            return;
                        }
                    }

                }


                //case of group not yet assigned (eg. still in adv scene)
                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {
                    if (maid.status.guid == StateManager.Instance.processingMaidGUID)
                    {
                        result = maid;
                        return;
                    }
                }
                foreach (var maid in StateManager.Instance.NPCList)
                {
                    if (maid.status.guid == StateManager.Instance.processingMaidGUID)
                    {
                        result = maid;
                        return;
                    }
                }
            }
        }


        internal static void GetSpoofMaidByString(string guid, ref Maid result)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {
                    if (maid.status.guid == guid)
                    {
                        result = maid;
                    }
                }

            }
        }

        internal static bool CheckSpoofActivateMaidObject()
        {
            //We dont want this function to execute if the spoof flag is set
            if (StateManager.Instance.SpoofActivateMaidObjectFlag)
            {
                return false;
            }
            return true;
        }


        internal static void SpoofGetManCharacters(BaseKagManager instance, KagTagSupport tag_data, ref Maid __result)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (!(instance is MotionKagManager))
                    return;
                if (__result == null)
                    return;

                MotionKagManager motionKagManager = (MotionKagManager)instance;

                if (tag_data.IsValid("man"))
                {
                    Maid man = motionKagManager.main_man;
                    Maid maid = motionKagManager.main_maid;

                    if (maid != null)
                    {
                        //this is a group motion
                        int manIndex = tag_data.GetTagProperty("man").AsInteger();

                        //use maid to search for the group.
                        PartyGroup group = Util.GetPartyGroupByCharacter(maid);

                        if (group != null)
                        {
                            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

                            if (motionItem == null)
                                __result = group.GetManAtIndex(0);
                            else
                            {
                                for (int i = 0; i < motionItem.ManIndex.Count; i++)
                                    if (motionItem.ManIndex[i] == manIndex)
                                        __result = group.GetManAtIndex(i);
                            }
                        }
                    }
                    else
                    {
                        __result = man;
                    }

                }
                else
                {
                    //Maid flow
                    Maid maid = motionKagManager.main_maid;

                    int maidIndex = tag_data.GetTagProperty("maid").AsInteger();

                    if (maidIndex > 0)
                    {
                        PartyGroup group = Util.GetPartyGroupByCharacter(maid);
                        if (group != null)
                        {
                            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

                            if (motionItem == null)
                                __result = group.GetMaidAtIndex(0);
                            else
                            {
                                for (int i = 0; i < motionItem.MaidIndex.Count; i++)
                                    if (motionItem.MaidIndex[i] == maidIndex)
                                        __result = group.GetMaidAtIndex(i);
                            }
                        }
                    }
                    else
                    {
                        if(maid != null)
                            __result = maid;
                    }
                }
            }
        }

        internal static bool IsSwapNewBodyNeeded(CharacterMgr characterMgr, int slot)
        {
            Maid man = characterMgr.GetMan(slot);
            if (StateManager.Instance.SelectedMaidsList.Contains(man))
                return false;
            return true;
        }
    }
}
