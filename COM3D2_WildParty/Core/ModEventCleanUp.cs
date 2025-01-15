﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class ModEventCleanUp
    {
        internal static void ResetModEvent()
        {
            RemoveAddedStockMan(StateManager.Instance.MenList);

            UnloadCharacters(StateManager.Instance.SelectedMaidsList);
            if (StateManager.Instance.ClubOwner != null && StateManager.Instance.MenList != null)
                StateManager.Instance.MenList.Add(StateManager.Instance.ClubOwner);
            UnloadCharacters(StateManager.Instance.MenList, false);

            RestoreBackupData();

            //Reset all the states
            ResetAllState();
        }


        //Due to the way of handling the mod event is a bit different from what the original game do, we need to fix some of the data in order to reflect the correct status
        internal static void UpdateMaidStatusForModEvent(List<Maid> maidList)
        {
            foreach (var maid in maidList)
            {
                //get the status prior the yotogi event happened
                BackupParamAccessor.Params oldParams = Util.GetBackupParam(maid, GameMain.Instance.CharacterMgr.status.isDaytime);
                maid.status.playCountYotogi = oldParams.yotogi_play_count + 1;

                //TODO: Update the maid.status.sexPlayNumberOfPeople too???

            }
        }

        private static void RemoveAddedStockMan(List<Maid> list)
        {
            if (list == null)
                return;
            foreach (var chara in list)
            {
                //For the man list, since we have add stock man on purpose and it have expanded the list. Need to remove those stock man list properly to prevent logic error when the game try to init all things.
                var m_listStockMan = Traverse.Create(GameMain.Instance.CharacterMgr).Field("m_listStockMan").GetValue<List<Maid>>();
                if (m_listStockMan != null)
                    m_listStockMan.Remove(chara);
            }
        }

        private static void UnloadCharacters(List<Maid> list, bool isMan = false)
        {
            if (list == null)
                return;
            //The position here doesnt matter much, we just want to use a non-zero index here
            int dummy_position = 5;
            foreach (var chara in list)
            {
                //may have scale it to zero to hide the model
                chara.transform.localScale = Vector3.one;

                //In order to unload the characters properly we use the original logic by KISS. Put the character in the maid list and deactivate it using the CharacterMgr
                //Spoof flag is set as we dont want the game to load all the bones again during set active
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                if (isMan)
                    GameMain.Instance.CharacterMgr.SetActiveMan(chara, dummy_position);
                else
                    GameMain.Instance.CharacterMgr.SetActiveMaid(chara, dummy_position);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                GameMain.Instance.CharacterMgr.Deactivate(dummy_position, isMan);
            }
        }


        private static void RestoreBackupData()
        {
            if (StateManager.Instance.OriginalManOrderList != null)
            {

                //Restore the backup man list
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
                {
                    if (i >= StateManager.Instance.OriginalManOrderList.Count)
                        break;

                    Maid man = StateManager.Instance.OriginalManOrderList[i];
                    GameMain.Instance.CharacterMgr.SetActiveMan(man, i);
                    
                    GameMain.Instance.CharacterMgr.CharaVisible(i, false, true);
                }
            }
        }

        private static void ResetAllState()
        {
            StateManager.Instance.UndergoingModEventID = -1;
            StateManager.Instance.MaxManUsed = -1;

            Util.ClearGenericCollection(StateManager.Instance.PartyGroupList);
            Util.ClearGenericCollection(StateManager.Instance.OriginalManOrderList);
            Util.ClearGenericCollection(StateManager.Instance.SelectedMaidsList);
            Util.ClearGenericCollection(StateManager.Instance.MenList);
            Util.ClearGenericCollection(StateManager.Instance.InjectedButtons);
            Util.ClearGenericCollection(StateManager.Instance.YotogiProgressInfoList);
            Util.ClearGenericCollection(StateManager.Instance.RandomPickIndexList);


            StateManager.Instance.RequireCheckModdedSceneFlag = false;
            StateManager.Instance.WaitForCharactersFullLoadFlag = false;
            StateManager.Instance.RequireInjectCommandButtons = false;

            StateManager.Instance.YotogiManager = null;
            StateManager.Instance.YotogiCommandFactory = null;
            StateManager.Instance.CommandMaskGroupTransform = null;

            StateManager.Instance.CurrentADVStepID = "";
            StateManager.Instance.ProcessedADVStepID = "";
            StateManager.Instance.processingMaidGUID = "";
            StateManager.Instance.processingManGUID = "";
        }


        internal static void RemoveMaidsFromSelectionList(List<Maid> maidList)
        {
            foreach (var maid in maidList)
            {
                SceneCharacterSelect.chara_guid_stock_list.Remove(maid.status.guid);
            }
        }
    }
}
