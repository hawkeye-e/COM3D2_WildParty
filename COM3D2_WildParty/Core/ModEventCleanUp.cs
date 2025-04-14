using System;
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
            //It could happen that some of the maids are converted to man when the mod event end (eg. terminate in the middle of mod event). Do a check and recover all back to maid structure.
            RecoverMaidStructure();

            RemoveAddedStockMan(StateManager.Instance.MenList);
            RemoveAddedStockMan(StateManager.Instance.NPCManList);
            RemoveMaidPairMan(StateManager.Instance.PairedManForMaidList);

            ResetAllMaid();

            UnloadCharacters(StateManager.Instance.SelectedMaidsList, Constant.CharacterType.Maid);
            if (StateManager.Instance.ClubOwner != null && StateManager.Instance.MenList != null)
                StateManager.Instance.MenList.Add(StateManager.Instance.ClubOwner);
            UnloadCharacters(StateManager.Instance.MenList, Constant.CharacterType.Man);
            UnloadNPC(StateManager.Instance.NPCList);
            UnloadCharacters(StateManager.Instance.NPCManList, Constant.CharacterType.Man);

            //Just want to destory the the following object so doesnt matter if it is calling BanishmentMaid
            StateManager.Instance.MenList.Remove(StateManager.Instance.ClubOwner);
            UnloadNPC(StateManager.Instance.MenList);
            UnloadNPC(StateManager.Instance.NPCManList);

            RemoveAddedObjects();

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
            //Remove the owner from this list in case it is added here (club owner is not temporarily added stock man)
            if (list.Contains(StateManager.Instance.ClubOwner))
                list.Remove(StateManager.Instance.ClubOwner);
            
            foreach (var chara in list)
            {
                //For the man list, since we have add stock man on purpose and it have expanded the list. Need to remove those stock man list properly to prevent logic error when the game try to init all things.
                var m_listStockMan = Traverse.Create(GameMain.Instance.CharacterMgr).Field("m_listStockMan").GetValue<List<Maid>>();
                if (m_listStockMan != null)
                    m_listStockMan.Remove(chara);
            }
        }

        private static void RecoverMaidStructure()
        {
            foreach(Maid maid in StateManager.Instance.SelectedMaidsList)
                CharacterHandling.RecoverMaidFromManStructure(maid);
        }
        
        private static void RemoveMaidPairMan(Dictionary<Maid, Maid> list)
        {
            if(list != null)
            {
                foreach(var kvp in list)
                {
                    //These objects do not belong to any in game array. Can just destroy the object
                    UnityEngine.Object.DestroyImmediate(kvp.Value.gameObject);
                }
            }
        }

        private static void UnloadCharacters(List<Maid> list, Constant.CharacterType type)
        {
            if (list == null)
                return;
            //The position here doesnt matter much, we just want to use a non-zero index here
            int dummy_position = 2;
            foreach (var chara in list)
            {
                //may have scale it to zero to hide the model
                chara.transform.localScale = Vector3.one;

                //In order to unload the characters properly we use the original logic by KISS. Put the character in the maid list and deactivate it using the CharacterMgr
                //Spoof flag is set as we dont want the game to load all the bones again during set active
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                if (type == Constant.CharacterType.Man)
                    GameMain.Instance.CharacterMgr.SetActiveMan(chara, dummy_position);
                else if (type == Constant.CharacterType.Maid)
                    GameMain.Instance.CharacterMgr.SetActiveMaid(chara, dummy_position);
                else if (type == Constant.CharacterType.NPC)
                    GameMain.Instance.CharacterMgr.SetActiveMaid(chara, dummy_position);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                bool isMan = !(type == Constant.CharacterType.Maid || type == Constant.CharacterType.NPC);                
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
            StateManager.Instance.BranchIndex = -1;

            Util.ClearGenericCollection(StateManager.Instance.PartyGroupList);
            Util.ClearGenericCollection(StateManager.Instance.OriginalManOrderList);
            Util.ClearGenericCollection(StateManager.Instance.SelectedMaidsList);
            Util.ClearGenericCollection(StateManager.Instance.MenList);
            Util.ClearGenericCollection(StateManager.Instance.NPCList);
            Util.ClearGenericCollection(StateManager.Instance.NPCManList);
            Util.ClearGenericCollection(StateManager.Instance.ManClothingList);
            Util.ClearGenericCollection(StateManager.Instance.InjectedButtons);
            Util.ClearGenericCollection(StateManager.Instance.YotogiProgressInfoList);
            Util.ClearGenericCollection(StateManager.Instance.RandomPickIndexList);

            Util.ClearGenericCollection(StateManager.Instance.YotogiWorkingMaidList);
            Util.ClearGenericCollection(StateManager.Instance.YotogiWorkingManList);

            Util.ClearGenericCollection(StateManager.Instance.DummyManList);

            Util.ClearGenericCollection(PartyGroup.SharedExtraManList);
            Util.ClearGenericCollection(PartyGroup.SharedExtraManSetupInfo);

            Util.ClearGenericCollection(StateManager.Instance.TimeEndTriggerList);
            Util.ClearGenericCollection(StateManager.Instance.AddedGameObjectList);

            Util.ClearGenericCollection(StateManager.Instance.BackupMaidClothingList);
            Util.ClearGenericCollection(StateManager.Instance.IgnoreResetPropMaidList);
            Util.ClearGenericCollection(StateManager.Instance.PairedManForMaidList);
            
            StateManager.Instance.RequireCheckModdedSceneFlag = false;
            StateManager.Instance.WaitForCharactersFullLoadFlag = false;
            StateManager.Instance.RequireInjectCommandButtons = false;
            StateManager.Instance.SpoofChangeBackgroundFlag = false;

            StateManager.Instance.IsMotionKagSetPosition = false;
            StateManager.Instance.IsMainGroupMotionScriptFlag = false;
            StateManager.Instance.IsYotogiUseModSemenPattern = false;

            StateManager.Instance.YotogiManager = null;
            StateManager.Instance.YotogiCommandFactory = null;
            StateManager.Instance.CommandMaskGroupTransform = null;
            StateManager.Instance.ExtraCommandWindow = null;
            StateManager.Instance.WaitingAnimationTrigger = null;
            StateManager.Instance.AnimationChangeTrigger = null;
            StateManager.Instance.VoiceLoopTrigger = null;

            StateManager.Instance.CurrentMotionKagHandlingGroup = null;

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

        private static void RemoveAddedObjects()
        {
            foreach(var kvp in StateManager.Instance.AddedGameObjectList)
                GameMain.Instance.BgMgr.DelPrefabFromBg(kvp.Key);
        }

        private static void ResetAllMaid()
        {
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
            {
                CharacterHandling.RestoreMaidClothesInfo(maid);
                maid.ResetAll();
            }
        }

        private static void UnloadNPC(List<Maid> maidList)
        {
            foreach (Maid maid in maidList)
                GameMain.Instance.CharacterMgr.BanishmentMaid(maid);
        }

        //For the maids that are created by the player, there is a thumb icon. Those injected by this mod does not.
        //Use this difference to remove any injected maids that the mod fail to remove properly in previous version.
        internal static void RemoveInjectedModNPC()
        {
            var stockmaids = GameMain.Instance.CharacterMgr.GetStockMaidList();
            for (int i = stockmaids.Count - 1; i >= 0; i--)
            {
                Maid maid = stockmaids[i];               
                if (maid.GetThumIcon() == null)
                {
                    GameMain.Instance.CharacterMgr.BanishmentMaid(maid);
                }
            }
        }
    }
}
