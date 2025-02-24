using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;


namespace COM3D2.WildParty.Plugin.HooksAndPatches.DebugUse
{
    internal class Patches
    {

        internal static void CaptureSpokenDialoguesInfo(string name, string text, string voice_file)
        {
            if (Config.DebugCaptureDialogues)
            {
                if (!DebugHelper.CaptureDialogueAndText.DialoguesDictionary.ContainsKey(voice_file) && !string.IsNullOrEmpty(voice_file))
                {
                    DebugHelper.CaptureDialogueAndText.DebugCaptureDialogueAndText newitem = new DebugHelper.CaptureDialogueAndText.DebugCaptureDialogueAndText();
                    newitem.Speaker = name;
                    newitem.text = text;
                    DebugHelper.CaptureDialogueAndText.DialoguesDictionary.Add(voice_file, newitem);
                }
            }
        }

        internal static void CapturePrivateTouchEventDialogues(PrivateMaidTouchManager.VoiceData voiceData)
        {
            if (Config.DebugCaptureDialogues)
            {
                if (!DebugHelper.CaptureDialogueAndText.DialoguesDictionary.ContainsKey(voiceData.voiceFileName) && !string.IsNullOrEmpty(voiceData.voiceFileName))
                {
                    DebugHelper.CaptureDialogueAndText.DebugCaptureDialogueAndText newitem = new DebugHelper.CaptureDialogueAndText.DebugCaptureDialogueAndText();
                    newitem.Speaker = "privateTouch";
                    newitem.text = voiceData.text;
                    DebugHelper.CaptureDialogueAndText.DialoguesDictionary.Add(voiceData.voiceFileName, newitem);
                }
            }
        }



        internal static void ProcessAudioFileChopping()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                DebugHelper.DebugState.Instance.IsCtrlPressed = !DebugHelper.DebugState.Instance.IsCtrlPressed;
            }

            else if (Input.GetKeyDown(KeyCode.U))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 1f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime -= offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime < 0f)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime = 0f;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                float offset = 0.01f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.5f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime -= offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime < 0f)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime = 0f;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                float offset = 0.01f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.5f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime += offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime > DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime = DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 1f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime += offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime > DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime = DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 1f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime -= offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime < DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime = DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                float offset = 0.01f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.5f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime -= offset;
                if (DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime < DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime)
                    DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime = DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }

            else if (Input.GetKeyDown(KeyCode.K))
            {
                float offset = 0.01f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.5f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime += offset;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 1f;
                DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime += offset;

                WildParty.Log.LogInfo("StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {

                DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex += 1;
                if (DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex >= DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion.Length)
                    DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex = 0;
                WildParty.Log.LogInfo("Current File: " + DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion[DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex]);
            }
            else if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex -= 1;
                if (DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex < 0)
                    DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex = DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion.Length - 1;
                WildParty.Log.LogInfo("Current File: " + DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion[DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex]);
            }
            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                //for output the chopped voice file

                var maid = GameMain.Instance.CharacterMgr.GetMaid(0);

                Helper.AudioChoppingManager.PlaySubClip(maid, "", DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion[DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex],
                    DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime, DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
                WildParty.Log.LogInfo("Current File: " + DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion[DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex]
                    + " StartTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioStartTime + ", EndTime: " + DebugHelper.DebugState.Instance.DebugSubclipAudioEndTime);
            }
            else if (Input.GetKeyDown(KeyCode.Period))
            {
                //for output the whole voice file

                var maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                maid.Visible = true;
                maid.AudioMan.LoadPlay(DebugHelper.DebugState.Instance.DebugVoiceFileInQuestion[DebugHelper.DebugState.Instance.DebugUseVoiceFileArrayIndex], 0f, false);

            }
        }

        internal static void PrintCameraInfo()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                //for printing the current camera setup information

                WildParty.Log.LogInfo("Camera Pos: " + GameMain.Instance.MainCamera.GetPos());
                WildParty.Log.LogInfo("Camera target Pos: " + GameMain.Instance.MainCamera.GetTargetPos());
                WildParty.Log.LogInfo("Camera distance: " + GameMain.Instance.MainCamera.GetDistance());
                WildParty.Log.LogInfo("Camera angle: " + GameMain.Instance.MainCamera.GetAroundAngle());

            }
        }

        internal static void ApplyAnimationInStudio()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                //for setting animation in studio so that making the adv scene easier

                //for (int i = 0; i < GameMain.Instance.CharacterMgr.GetMaidCount(); i++)
                //{
                //    Maid maid = GameMain.Instance.CharacterMgr.GetMaid(i);
                //    if (maid != null)
                //        WildParty.Log.LogInfo(maid.status.fullNameJpStyle);

                //    if (i == 0)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "pillow_talk_taiki_f.anm", "pillow_talk_taiki_f.anm");
                //    }
                //    if (i == 2)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "OM_yorisoi_aibu_taiki_f.anm", "om_yorisoi_aibu_taiki_f.anm");

                //    }
                //    if (i == 3)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "harem_seijyouiC_3_f.anm", "harem_seijyouic_3_f.anm");

                //    }
                //    if (i == 4)
                //    {
                //        Core.CharacterHandling.LoadMotionScript(0, false, "ero_scene_001.ks", "*ピロトーク_一人", maid.status.guid, "");

                //    }
                //    if (i == 5)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "harem_seijyoui_3_f2.anm", "harem_seijyoui_3_f2.anm");
                //    }
                //    if (i == 6)
                //    {
                //        Core.CharacterHandling.LoadMotionScript(0, false, "ero_scene_001.ks", "*気絶", maid.status.guid, "");
                //    }
                //    if (i == 7)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "pillow_talk_taiki_f.anm", "pillow_talk_taiki_f.anm");
                //    }
                //    if (i == 8)
                //    {
                //        Core.CharacterHandling.LoadMotionScript(0, false, "ero_scene_001.ks", "*気絶", maid.status.guid, "");
                //    }
                //    if (i == 9)
                //    {
                //        Core.CharacterHandling.PlayAnimation(maid, "pillow_talk_taiki_f.anm", "pillow_talk_taiki_f.anm");
                //    }
                //}
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMan(i);
                    if (maid != null)
                        WildParty.Log.LogInfo(maid.status.fullNameJpStyle);

                    if (i == 0)
                    {
                        Core.CharacterHandling.LoadMotionScript(0, false, "h_man_001.ks", "*男ソファーワイン待機L", "", maid.status.guid);
                        //Core.CharacterHandling.PlayAnimation(maid, "OM_kyousitu_aibu_taiki_m.anm", "OM_kyousitu_aibu_taiki_m.anm");
                    }
                    if (i >= 1)
                    {
                        //Core.CharacterHandling.LoadMotionScript(0, false, "h_man_001.ks", "*男ソファーワイン待機", "", maid.status.guid);
                        Core.CharacterHandling.PlayAnimation(maid, "harituke_sex_taiki_m.anm", "harituke_sex_taiki_m.anm");

                    }
                }
            }
        }

        internal static void PrintAllCapturedDialoguesAndTexts()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                //For printing all the captured dialogue file names and corresponding texts
                if (Config.DebugCaptureDialogues)
                {
                    WildParty.Log.LogInfo("Speaker,VoiceFile,Text");

                    foreach (var kvp in DebugHelper.CaptureDialogueAndText.DialoguesDictionary)
                    {
                        WildParty.Log.LogInfo(kvp.Value.Speaker + "," + kvp.Key + "," + kvp.Value.text);
                    }
                }

                foreach (var kvp in DebugHelper.DebugState.Instance.ScriptInfoCapture)
                {
                    foreach(var kvp2 in kvp.Value)
                        WildParty.Log.LogInfo("script file: " + kvp.Key + ", label: " + kvp2);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                DebugHelper.Debug.MapAvailableSkillForMaid(GameMain.Instance.CharacterMgr.GetMaid(0));
            }
        }

        internal static void PrintCharacterSetup()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                //For printing the json string for the character placement in adv scene (mainly for position and rotation info)

                //int counter = 0;
                //for (int i = 0; i < StateManager.Instance.SelectedMaidsList.Count; i++)
                //{
                //    Maid maid = StateManager.Instance.SelectedMaidsList[i];
                //    if (maid != null)
                //    {
                //        if (maid.Visible)
                //        {
                //            WildParty.Log.LogInfo(maid.status.fullNameJpStyle);
                //            WildParty.Log.LogInfo("{");
                //            WildParty.Log.LogInfo("\"Type\": \"F\",");
                //            WildParty.Log.LogInfo("\"ArrayPosition\": " + counter + ",");
                //            WildParty.Log.LogInfo("\"Visible\": true,");
                //            WildParty.Log.LogInfo("\"WaitLoad\": true,");
                //            WildParty.Log.LogInfo("\"MotionType\": \"RandomRest\"");
                //            WildParty.Log.LogInfo("\"FaceAnime\": \"RandomRest\",");
                //            WildParty.Log.LogInfo("\"PosRot\": {");
                //            WildParty.Log.LogInfo("\"PosString\": " + maid.transform.position + ",");
                //            WildParty.Log.LogInfo("\"RotString\": " + maid.transform.rotation + "");
                //            WildParty.Log.LogInfo("\"local PosString\": " + maid.transform.localPosition + ",");
                //            WildParty.Log.LogInfo("\"local RotString\": " + maid.transform.localRotation + "");
                //            //WildParty.Log.LogInfo("\"PosX\": " + maid.transform.localPosition.x.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosY\": " + maid.transform.localPosition.y.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosZ\": " + maid.transform.localPosition.z.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"Rotate\": " + maid.GetRot().y.ToString("0.0") + "");
                //            WildParty.Log.LogInfo("}");
                //            WildParty.Log.LogInfo("},");

                //            counter++;
                //        }
                //    }
                //}

                //counter = 0;
                //for (int i = 0; i < StateManager.Instance.MenList.Count; i++)
                //{
                //    Maid maid = StateManager.Instance.MenList[i];
                //    if (maid != null)
                //    {
                //        if (maid.Visible)
                //        {
                //            WildParty.Log.LogInfo(maid.status.fullNameJpStyle);
                //            WildParty.Log.LogInfo("{");
                //            WildParty.Log.LogInfo("\"Type\": \"M\",");
                //            WildParty.Log.LogInfo("\"ArrayPosition\": " + counter + ",");
                //            WildParty.Log.LogInfo("\"Visible\": true,");
                //            WildParty.Log.LogInfo("\"ShowPenis\": true,");
                //            WildParty.Log.LogInfo("\"WaitLoad\": true,");
                //            WildParty.Log.LogInfo("\"MotionType\": \"RandomRest\"");
                //            WildParty.Log.LogInfo("\"PosRot\": {");
                //            WildParty.Log.LogInfo("\"PosString\": " + maid.transform.position + ",");
                //            WildParty.Log.LogInfo("\"RotString\": " + maid.transform.rotation + "");
                //            WildParty.Log.LogInfo("\"local PosString\": " + maid.transform.localPosition + ",");
                //            WildParty.Log.LogInfo("\"local RotString\": " + maid.transform.localRotation + "");
                //            //WildParty.Log.LogInfo("\"PosX\": " + maid.transform.localPosition.x.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosY\": " + maid.transform.localPosition.y.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosZ\": " + maid.transform.localPosition.z.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"Rotate\": " + maid.GetRot().y.ToString("0.0") + "");
                //            WildParty.Log.LogInfo("}");
                //            WildParty.Log.LogInfo("},");

                //            counter++;
                //        }
                //    }
                //}

                //if (StateManager.Instance.ClubOwner != null)
                //{
                //    Maid maid = StateManager.Instance.ClubOwner;
                //    if (maid != null)
                //    {
                //        if (maid.Visible)
                //        {
                //            WildParty.Log.LogInfo("Club Owner: ");
                //            WildParty.Log.LogInfo(maid.status.fullNameJpStyle);
                //            WildParty.Log.LogInfo("{");
                //            WildParty.Log.LogInfo("\"Type\": \"M\",");
                //            WildParty.Log.LogInfo("\"ArrayPosition\": " + counter + ",");
                //            WildParty.Log.LogInfo("\"Visible\": true,");
                //            WildParty.Log.LogInfo("\"ShowPenis\": true,");
                //            WildParty.Log.LogInfo("\"WaitLoad\": true,");
                //            WildParty.Log.LogInfo("\"MotionType\": \"RandomRest\"");
                //            WildParty.Log.LogInfo("\"PosRot\": {");
                //            WildParty.Log.LogInfo("\"PosString\": " + maid.transform.position + ",");
                //            WildParty.Log.LogInfo("\"RotString\": " + maid.transform.rotation + "");
                //            WildParty.Log.LogInfo("\"local PosString\": " + maid.transform.localPosition + ",");
                //            WildParty.Log.LogInfo("\"local RotString\": " + maid.transform.localRotation + "");
                //            //WildParty.Log.LogInfo("\"PosX\": " + maid.transform.localPosition.x.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosY\": " + maid.transform.localPosition.y.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"PosZ\": " + maid.transform.localPosition.z.ToString("0.0") + ",");
                //            //WildParty.Log.LogInfo("\"Rotate\": " + maid.GetRot().y.ToString("0.0") + "");
                //            WildParty.Log.LogInfo("}");
                //            WildParty.Log.LogInfo("},");

                //            counter++;
                //        }
                //    }
                //}

                ////This part is for studio mode

                int counter = 0;
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetMaidCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMaid(i);
                    if (maid != null)
                    {
                        if (maid.Visible)
                        {
                            WildParty.Log.LogInfo("{");
                            WildParty.Log.LogInfo("\"Type\": \"F\",");
                            WildParty.Log.LogInfo("\"ArrayPosition\": " + counter + ",");
                            WildParty.Log.LogInfo("\"Visible\": true,");
                            WildParty.Log.LogInfo("\"WaitLoad\": true,");
                            WildParty.Log.LogInfo("\"MotionType\": \"RandomRest\"");
                            WildParty.Log.LogInfo("\"FaceAnime\": \"RandomRest\",");
                            WildParty.Log.LogInfo("\"PosRot\": {");
                            WildParty.Log.LogInfo("\"PosString\": \"" + maid.transform.position + "\",");
                            WildParty.Log.LogInfo("\"RotString\": \"" + maid.transform.rotation + "\"");
                            //WildParty.Log.LogInfo("\"PosX\": " + maid.transform.localPosition.x.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"PosY\": " + maid.transform.localPosition.y.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"PosZ\": " + maid.transform.localPosition.z.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"Rotate\": " + maid.GetRot().y.ToString("0.0") + "");
                            WildParty.Log.LogInfo("}");
                            WildParty.Log.LogInfo("},");

                            counter++;
                        }
                    }
                }

                counter = 0;
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetMaidCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMan(i);
                    if (maid != null)
                    {
                        if (maid.Visible)
                        {
                            WildParty.Log.LogInfo("{");
                            WildParty.Log.LogInfo("\"Type\": \"M\",");
                            WildParty.Log.LogInfo("\"ArrayPosition\": " + counter + ",");
                            WildParty.Log.LogInfo("\"Visible\": true,");
                            WildParty.Log.LogInfo("\"ShowPenis\": true,");
                            WildParty.Log.LogInfo("\"WaitLoad\": true,");
                            WildParty.Log.LogInfo("\"MotionType\": \"RandomRest\"");
                            WildParty.Log.LogInfo("\"PosRot\": {");
                            WildParty.Log.LogInfo("\"PosString\": \"" + maid.transform.position + "\",");
                            WildParty.Log.LogInfo("\"RotString\": \"" + maid.transform.rotation + "\"");
                            //WildParty.Log.LogInfo("\"PosX\": " + maid.transform.localPosition.x.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"PosY\": " + maid.transform.localPosition.y.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"PosZ\": " + maid.transform.localPosition.z.ToString("0.0") + ",");
                            //WildParty.Log.LogInfo("\"Rotate\": " + maid.GetRot().y.ToString("0.0") + "");
                            WildParty.Log.LogInfo("}");
                            WildParty.Log.LogInfo("},");

                            counter++;
                        }
                    }
                }
            }
        }

        internal static void CaptureMotionFileNames(string file_name, string label_name)
        {
            if (Config.DebugLogMotionData)
            {
                DebugHelper.CaptureMotionData.currentMotionFileName = file_name;
                DebugHelper.CaptureMotionData.currentMotionLabelName = label_name;
            }
        }

        internal static void CaptureMotionFaceInfo(string tag)
        {
            if (Config.DebugLogMotionData)
                DebugHelper.CaptureMotionData.currentFaceAnim = tag;
        }

        internal static void CaptureMotionVoiceFile(AudioSourceMgr asMgr, string f_strFileName, bool f_bLoop)
        {
            if (Config.DebugLogMotionData)
            {
                if (DebugHelper.CaptureMotionData.MotionSoundDictionary.ContainsKey(DebugHelper.CaptureMotionData.currentSkillID))
                {
                    //we add a item here
                    DebugHelper.CaptureMotionData.DebugSkillData.MotionSound ms = new DebugHelper.CaptureMotionData.DebugSkillData.MotionSound();
                    ms.FileName = DebugHelper.CaptureMotionData.currentMotionFileName;
                    ms.MotionLabel = DebugHelper.CaptureMotionData.currentMotionLabelName;
                    ms.FaceAnim = DebugHelper.CaptureMotionData.currentFaceAnim;
                    ms.SoundFile = f_strFileName;
                    ms.SoundLoop = f_bLoop;
                    ms.Excitement = DebugHelper.CaptureMotionData.DebugSkillData.MotionSound.GetExcitement(GameMain.Instance.CharacterMgr.GetMaid(0).status.currentExcite);

                    if (GameMain.Instance.CharacterMgr.GetMaid(0).AudioMan == asMgr)
                    {
                        ms.Personality = Util.GetPersonalityNameByValue(GameMain.Instance.CharacterMgr.GetMaid(0).status.personal.id);
                    }
                    else if (GameMain.Instance.CharacterMgr.GetMaid(1) != null)
                    {
                        if (GameMain.Instance.CharacterMgr.GetMaid(1).AudioMan == asMgr)
                        {
                            ms.Personality = Util.GetPersonalityNameByValue(GameMain.Instance.CharacterMgr.GetMaid(1).status.personal.id);
                        }
                    }

                    if (!DebugHelper.CaptureMotionData.MotionSoundDictionary[DebugHelper.CaptureMotionData.currentSkillID].ValidMotionList.Any(
                        x => x.FileName == ms.FileName && x.MotionLabel == ms.MotionLabel && x.FaceAnim == ms.FaceAnim && x.SoundFile == ms.SoundFile && x.SoundLoop == ms.SoundLoop && x.Excitement == ms.Excitement
                        && x.Personality == ms.Personality)
                        )
                    {
                        DebugHelper.CaptureMotionData.MotionSoundDictionary[DebugHelper.CaptureMotionData.currentSkillID].ValidMotionList.Add(ms);
                    }
                }

            }
        }

        internal static void CaptureSkillName(YotogiPlayManager playMgr)
        {
            if (Config.DebugLogMotionData)
            {
                if (!DebugHelper.CaptureMotionData.MotionSoundDictionary.ContainsKey(playMgr.playingSkill.skill_pair.base_data.id))
                {
                    DebugHelper.CaptureMotionData.DebugSkillData data = new DebugHelper.CaptureMotionData.DebugSkillData();
                    data.SkillName = playMgr.playingSkill.skill_pair.base_data.name;
                    DebugHelper.CaptureMotionData.MotionSoundDictionary.Add(playMgr.playingSkill.skill_pair.base_data.id, data);
                }
                DebugHelper.CaptureMotionData.currentSkillID = playMgr.playingSkill.skill_pair.base_data.id;
            }
        }

        internal static void CaptureMotionSoundInfo()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //reset param
                Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                maid.status.currentMind = maid.status.maxMind;
                maid.status.currentExcite = -100;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //reset dict
                DebugHelper.CaptureMotionData.MotionSoundDictionary.Clear();
                DebugHelper.CaptureMotionData.MotionSoundDictionary = new Dictionary<int, DebugHelper.CaptureMotionData.DebugSkillData>();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //print face info
                WildParty.Log.LogInfo("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");

                foreach (var kvp in DebugHelper.CaptureMotionData.MotionSoundDictionary)
                {
                    List<string> face0 = new List<string>();
                    List<string> face1 = new List<string>();
                    List<string> face2 = new List<string>();
                    List<string> face3 = new List<string>();


                    foreach (var item in kvp.Value.ValidMotionList)
                    {
                        if (item.SoundLoop)
                        {

                            if (item.Excitement == "-100~-51" || item.Excitement == "-50~0")
                            {
                                if (!face0.Contains(item.FaceAnim))
                                    face0.Add(item.FaceAnim);
                            }
                            else if (item.Excitement == "0~49" || item.Excitement == "50~99")
                            {
                                if (!face1.Contains(item.FaceAnim))
                                    face1.Add(item.FaceAnim);
                            }
                            else if (item.Excitement == "100~149" || item.Excitement == "150~199")
                            {
                                if (!face2.Contains(item.FaceAnim))
                                    face2.Add(item.FaceAnim);
                            }
                            else if (item.Excitement == "200~249" || item.Excitement == "250~299" || item.Excitement == "300")
                            {
                                if (!face3.Contains(item.FaceAnim))
                                    face3.Add(item.FaceAnim);
                            }
                        }

                    }

                    WildParty.Log.LogInfo("Excitement level 0: " + string.Join(",", face0.ToArray()));
                    WildParty.Log.LogInfo("Excitement level 1: " + string.Join(",", face1.ToArray()));
                    WildParty.Log.LogInfo("Excitement level 2: " + string.Join(",", face2.ToArray()));
                    WildParty.Log.LogInfo("Excitement level 3: " + string.Join(",", face3.ToArray()));

                    WildParty.Log.LogInfo("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad0))
            {

                WildParty.Log.LogInfo("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                //format: Skill ID, Skill Name
                foreach (var kvp in DebugHelper.CaptureMotionData.MotionSoundDictionary)
                {
                    WildParty.Log.LogInfo(kvp.Key + "," + kvp.Value.SkillName);
                }
                //format: Skill ID, /*Skill Name*/, Motion file name, label, excitement, sound file, sound loop, face anime, Personality
                WildParty.Log.LogInfo("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                foreach (var kvp in DebugHelper.CaptureMotionData.MotionSoundDictionary)
                {
                    foreach (var item in kvp.Value.ValidMotionList)
                    {
                        WildParty.Log.LogInfo(kvp.Key + ","
                            //+ kvp.Value.SkillName + ","
                            + item.FileName + ","
                            + item.MotionLabel + ","
                            + item.Excitement + ","
                            + item.SoundFile + ","
                            + item.SoundLoop + ","
                            + item.FaceAnim + ","
                            + item.Personality

                            );
                    }

                }

                WildParty.Log.LogInfo("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            }

            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                foreach(var g in StateManager.Instance.PartyGroupList)
                {
                    PrintCharacterPosRot(g.Maid1);
                    PrintCharacterPosRot(g.Maid2);
                    PrintCharacterPosRot(g.Man1);
                    PrintCharacterPosRot(g.Man2);
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugX -= offset;
                
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugX += offset;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugY -= offset;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugY += offset;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugZ -= offset;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                float offset = 0.1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.01f;
                DebugHelper.DebugState.Instance.DebugZ += offset;
            }
        }

        private static void PrintCharacterPosRot(Maid maid)
        {
            if (maid == null)
                return;
            WildParty.Log.LogInfo(maid.status.fullNameJpStyle);
            WildParty.Log.LogInfo("Pos: " + maid.transform.position);
            WildParty.Log.LogInfo("Rot: " + maid.transform.rotation);
        }

        internal static void YotogiPlayGroupArrangement()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                DebugHelper.DebugState.Instance.IsCtrlPressed = !DebugHelper.DebugState.Instance.IsCtrlPressed;
            }

            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {

                Vector3 offset = new Vector3(0.1f, 0f, 0f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(0.01f, 0f, 0f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 offset = new Vector3(-0.1f, 0f, 0f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(-0.01f, 0f, 0f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 offset = new Vector3(0f, 0f, -0.1f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(0f, 0f, -0.01f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 offset = new Vector3(0f, 0f, 0.1f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(0f, 0f, 0.01f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);

            }
            else if (Input.GetKeyDown(KeyCode.Insert))
            {
                Vector3 offset = new Vector3(0f, 0.1f, 0f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(0f, 0.01f, 0f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Vector3 offset = new Vector3(0f, -0.1f, 0f);
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = new Vector3(0f, -0.01f, 0f);

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseMoveGroup(group, offset);
            }
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                float offset = 1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = 0.1f;

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseRotateGroup(group, offset);
            }
            else if (Input.GetKeyDown(KeyCode.End))
            {
                float offset = -1f;
                if (DebugHelper.DebugState.Instance.IsCtrlPressed)
                    offset = -0.1f;
                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseRotateGroup(group, offset);
            }

            else if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                float offset = 90f;

                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseRotateGroup(group, offset);
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                float offset = -90f;
                var group = StateManager.Instance.PartyGroupList[DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex];
                TestingUseRotateGroup(group, offset);
            }

            else if (Input.GetKeyDown(KeyCode.PageUp))
            {
                DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex--;
                if (DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex < 0)
                    DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex = StateManager.Instance.PartyGroupList.Count - 1;
                WildParty.Log.LogInfo("Current Group Index: " + DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex);
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex++;
                if (DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex >= StateManager.Instance.PartyGroupList.Count)
                    DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex = 0;
                WildParty.Log.LogInfo("Current Group Index: " + DebugHelper.DebugState.Instance.DebugUseCurrentGroupIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                //reset the position for all group for arrangment purpose
                foreach (var group in StateManager.Instance.PartyGroupList)
                {
                    group.GroupPosition = Vector3.zero;
                    group.GroupRotation = new Quaternion(0f, 0f, 0f, 1f);
                    group.SetGroupPosition();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                int i = 0;
                foreach (var group in StateManager.Instance.PartyGroupList)
                {
                    var maid = group.Maid1;

                    WildParty.Log.LogInfo(maid.status.fullNameJpStyle);
                    WildParty.Log.LogInfo("{");
                    WildParty.Log.LogInfo("\"ArrayPosition\": " + i);
                    WildParty.Log.LogInfo("\"PosString\": \"" + group.GroupPosition.x.ToString("0.0") + ", " + group.GroupPosition.y.ToString("0.0") + ", " + group.GroupPosition.z.ToString("0.0") + "\",");
                    WildParty.Log.LogInfo("\"RotString\": \"" + maid.transform.localRotation.x.ToString("0.0") + ", " + maid.transform.localRotation.y.ToString("0.0") + ", "
                        + maid.transform.localRotation.z.ToString("0.0") + ", " + maid.transform.localRotation.w.ToString("0.0") + "\",");
                    WildParty.Log.LogInfo("},");

                    i++;

                }

            }
        }



        private static void TestingUseMoveGroup(PartyGroup group, Vector3 offset)
        {
            group.GroupPosition += offset;
            group.SetGroupPosition();

            if (PartyGroup.UnassignedMaid != null)
                PartyGroup.UnassignedMaid.transform.position += offset;
        }

        private static void TestingUseRotateGroup(PartyGroup group, float offset)
        {
            TestingUseRotateMaid(group.Maid1, offset);
            TestingUseRotateMaid(group.Maid2, offset);
            TestingUseRotateMaid(group.Man1, offset);
            TestingUseRotateMaid(group.Man2, offset);

            group.GroupRotation = group.Maid1.transform.rotation;

            if (PartyGroup.UnassignedMaid != null)
                PartyGroup.UnassignedMaid.transform.Rotate(Vector3.up, offset);
        }

        private static void TestingUseRotateMaid(Maid maid, float offset)
        {
            if (maid != null)
            {
                if (maid.Visible)
                {
                    maid.gameObject.transform.Rotate(Vector3.up, offset);
                }
            }
        }
    }
}
