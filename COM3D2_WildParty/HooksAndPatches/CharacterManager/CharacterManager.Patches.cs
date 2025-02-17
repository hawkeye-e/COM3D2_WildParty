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
                foreach (var group in StateManager.Instance.PartyGroupList)
                {
                    if (group.Man1.status.guid == StateManager.Instance.processingManGUID)
                    {
                        //If it is a hit, determine which man in the group is returned
                        if (manNo == 0)
                            result = group.Man1;
                        else if (manNo == 1)
                            result = group.Man2;
                    }
                }
                //check also the club owner
                if (StateManager.Instance.ClubOwner != null)
                    if (StateManager.Instance.ClubOwner.status.guid == StateManager.Instance.processingManGUID)
                        result = StateManager.Instance.ClubOwner;

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
                        if (group.Maid1.status.guid == StateManager.Instance.processingMaidGUID)
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

        
        internal static void SpoofGetManCharacters(KagTagSupport tag_data, ref Maid __result)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (tag_data.IsValid("man"))
                {
                    //Use the recorded group index to locate the correct man characters and return as result
                    int manIndex = tag_data.GetTagProperty("man").AsInteger();

                    if (StateManager.Instance.currentGroup >= 0 && __result != null && StateManager.Instance.PartyGroupList.Count > 1)
                    {
                        //TODO: need to use the manIndex
                        if (manIndex == 0)
                        {
                            if (StateManager.Instance.PartyGroupList[StateManager.Instance.currentGroup].Man1 != null)
                                __result = StateManager.Instance.PartyGroupList[StateManager.Instance.currentGroup].Man1;
                        }
                        else if (manIndex == 1)
                        {

                            if (StateManager.Instance.PartyGroupList[StateManager.Instance.currentGroup].Man2 != null)
                                __result = StateManager.Instance.PartyGroupList[StateManager.Instance.currentGroup].Man2;
                        }

                    }
                }
                else
                {
                    //Maid flow, we record the group index
                    for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
                    {
                        if (__result != null && StateManager.Instance.PartyGroupList[i].Maid1.status.guid == __result.status.guid)
                        {
                            StateManager.Instance.currentGroup = i;
                            break;
                        }
                    }
                }
            }
        }
    }
}
