using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class YotogiExtraCommandCallbacks
    {
        internal static void ChangeMainGroupSkill_Callback(string skillID)
        {
            GameMain.Instance.MainCamera.FadeOut(5f, f_dg: delegate
            {
                YotogiHandling.ChangeMainGroupSkill(skillID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(5f);
        }

        internal static void MassChangeGroupSkill_Callback(string skillID)
        {
            GameMain.Instance.MainCamera.FadeOut(5f, f_dg: delegate
            {
                //this will only apply to group that have the same man-woman combination as the main group
                var groupZero = StateManager.Instance.PartyGroupList[0];

                var skillItem = ModUseData.ValidOrgySkillList[groupZero.Maid1.status.personal.id][groupZero.GroupType].Where(x => x.YotogiSkillID == skillID).First();
                //change all other groups
                for (int i = 1; i < StateManager.Instance.PartyGroupList.Count; i++)
                {
                    if (StateManager.Instance.PartyGroupList[i].GroupType == groupZero.GroupType)
                    {
                        YotogiHandling.ChangeBackgroundGroupSexPosition(StateManager.Instance.PartyGroupList[i], skillItem.SexPosID, true);
                    }
                }
                YotogiHandling.ChangeMainGroupSkill(skillID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(5f);
        }

        internal static void ChangeFormation_Callback(string formationID)
        {
            PartyGroup.CurrentFormation = formationID;
            GameMain.Instance.MainCamera.FadeOut(5f, f_dg: delegate
            {
                YotogiHandling.SetGroupFormation(formationID);
            });
            GameMain.Instance.MainCamera.FadeIn(5f);
        }

        internal static void ChangeTargetMaid_Callback(string maid_guid)
        {
            GameMain.Instance.MainCamera.FadeOut(5f, f_dg: delegate
            {
                //Change the maid for maid[0], keep man[0] and shuffle all the remaining
                Maid selectedMaid = StateManager.Instance.SelectedMaidsList.Where(x => x.status.guid == maid_guid).First();
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                GameMain.Instance.CharacterMgr.SetActiveMaid(selectedMaid, 0);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                CharacterHandling.StopCurrentAnimation();


                CharacterHandling.AssignPartyGrouping(true);
                BackgroundGroupMotionManager.InitNextReviewTimer();
                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                //need to update the main group
                var initialSkill = YotogiHandling.GetRandomSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();



                Util.ResetAllGroupPosition();

                CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            });
            GameMain.Instance.MainCamera.FadeIn(5f);
        }
    }
}
