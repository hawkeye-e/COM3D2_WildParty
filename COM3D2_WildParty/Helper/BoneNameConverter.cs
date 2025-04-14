using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    internal class BoneNameConverter
    {
        //Key: Maid GUID, Value.Key: Original Parent Bone; Value.Value: Bones that have been rearranged
        private static Dictionary<string, Dictionary<Transform, List<Transform>>> _convertedFemaleDisplacedBones = new Dictionary<string, Dictionary<Transform, List<Transform>>>();

        //Key: Maid GUID, Value, the dummy man object that "lend" the penis to maid
        private static Dictionary<string, PairedMaidAndMan> _pairedMan = new Dictionary<string, PairedMaidAndMan>();

        private class PairedMaidAndMan
        {
            internal Maid Maid;
            internal Maid Man;

            internal PairedMaidAndMan(Maid m1, Maid m2)
            {
                Maid = m1;
                Man = m2;
            }
        }

        internal static void ConvertManStructureToFemale(Maid man)
        {
            if (man == null)
                return;
            RecurConvertManBoneNameToFemale(man.transform);
        }

        internal static void RecoverConvertedManStructure(Maid man)
        {
            if (man == null)
                return;
            RecurRecoverManBoneName(man.transform);
        }

        internal static void ConvertFemaleStructureToMale(Maid maid, Maid dummyMan)
        {
            if (maid == null || dummyMan == null)
                return;
            _pairedMan.Add(maid.status.guid, new PairedMaidAndMan(maid, dummyMan));
            RecurConvertFemaleBoneNameToMale(maid, maid.transform);
            AddPenis(maid, dummyMan);
            //SetManTransparent(dummyMan);
        }

        internal static void RecoverConvertedMaidStructure(Maid maid)
        {
            if (maid == null || !_pairedMan.ContainsKey(maid.status.guid))
                return;

            ReturnPenis(maid, _pairedMan[maid.status.guid].Man);
            RecurRecoverFemaleBoneName(maid, maid.transform);
            _convertedFemaleDisplacedBones.Remove(maid.status.guid);
            _pairedMan.Remove(maid.status.guid);
        }

        internal static void RecoverAllMaid()
        {
            List<string> convertedMaids = _pairedMan.Keys.ToList();
            foreach(string guid in convertedMaids)
            {
                RecoverConvertedMaidStructure(_pairedMan[guid].Maid);
            }
        }

        internal static bool IsMaidConvertedMan(Maid maid)
        {
            return _pairedMan.ContainsKey(maid.status.guid);
        }

        private static void AddPenis(Maid maid, Maid dummyMan)
        {
            if (dummyMan != null)
            {
                string smBoneName = dummyMan.body0.m_Bones.name.Replace("_BO_", "_SM_");

                TransplantPenisWithoutBalls(dummyMan.body0.m_Bones.transform, maid.transform, "ManBip/ManBip Pelvis/chinkoCenter/chinko1", "Offset/_BO_mbody/ManBip/ManBip Pelvis");
                TransplantPenisWithoutBalls(dummyMan.body0.m_Bones.transform, maid.transform, smBoneName + "/ManBip/ManBip Pelvis/chinkoCenter/chinko1", "Offset/_BO_mbody/_SM_mbody/ManBip/ManBip Pelvis");
                TransplantPenisWithoutBalls(dummyMan.body0.m_Bones.transform, maid.transform, "_SM_mbody_moza/ManBip/ManBip Pelvis/chinkoCenter/chinko1", "Offset/_BO_mbody/_SM_mbody_moza/ManBip/ManBip Pelvis");
                TransplantPenisWithoutBalls(dummyMan.body0.m_Bones2.transform, maid.transform, "ManBip/ManBip Pelvis/chinkoCenter/chinko1", "Offset/_BO_mbody MR/ManBip/ManBip Pelvis");
            }
        }

        //Remove the penis object from the source character and attach it to the maid
        private static Transform TransplantPenisWithoutBalls(Transform srcCharacter, Transform targetCharacter, string sourceBoneLocation, string targetBoneLocation)
        {
            Transform srcBoneTransform = srcCharacter.transform.Find(sourceBoneLocation);
            Transform maidPelvis = targetCharacter.transform.Find(targetBoneLocation);

            GameObject chinkoCenter = GameObject.Instantiate(srcBoneTransform.parent.gameObject);
            chinkoCenter.name = "chinkoCenter";
            chinkoCenter.SetActive(true);

            //We only need the cloned chinkoCenter object, the cloned children is not needed as we will use the one we are going to transplant
            for(int i= chinkoCenter.transform.childCount - 1; i>=0; i--)
            {
                var t = chinkoCenter.transform.GetChild(i);
                t.SetParent(null);
                GameObject.DestroyImmediate(t.gameObject);
            }

            chinkoCenter.transform.SetParent(maidPelvis, false);
            chinkoCenter.transform.position = chinkoCenter.transform.parent.position;

            srcBoneTransform.SetParent(chinkoCenter.transform);
            srcBoneTransform.localPosition = new Vector3(-0.05f, -0.03f, 0f);
            srcBoneTransform.localScale = Vector3.one;


            return srcBoneTransform.transform;
        }

        private static void ReturnPenis(Maid maid, Maid man)
        {
            string smBoneName = man.body0.m_Bones.name.Replace("_BO_", "_SM_");
            ReturnPenisWithoutBalls(maid.transform, man.body0.m_Bones.transform, "Offset/_BO_mbody/ManBip/ManBip Pelvis/chinkoCenter/chinko1", "ManBip/ManBip Pelvis/chinkoCenter");
            ReturnPenisWithoutBalls(maid.transform, man.body0.m_Bones.transform, "Offset/_BO_mbody/_SM_mbody/ManBip/ManBip Pelvis/chinkoCenter/chinko1", smBoneName + "/ManBip/ManBip Pelvis/chinkoCenter");
            ReturnPenisWithoutBalls(maid.transform, man.body0.m_Bones.transform, "Offset/_BO_mbody/_SM_mbody_moza/ManBip/ManBip Pelvis/chinkoCenter/chinko1", "_SM_mbody_moza/ManBip/ManBip Pelvis/chinkoCenter");
            ReturnPenisWithoutBalls(maid.transform, man.body0.m_Bones2.transform, "Offset/_BO_mbody MR/ManBip/ManBip Pelvis/chinkoCenter/chinko1", "ManBip/ManBip Pelvis/chinkoCenter");
        }

        //Detach the penis object from the maid and attach it back to the dummy character
        private static Transform ReturnPenisWithoutBalls(Transform srcCharacter, Transform targetCharacter, string sourceBoneLocation, string targetBoneLocation)
        {
            Transform srcBoneTransform = srcCharacter.transform.Find(sourceBoneLocation);
            Transform targetChinkoCenter = targetCharacter.transform.Find(targetBoneLocation);

            GameObject goMaidChinkoCenter = srcBoneTransform.parent.gameObject;

            srcBoneTransform.transform.SetParent(targetChinkoCenter, false);

            goMaidChinkoCenter.transform.SetParent(null);
            GameObject.DestroyImmediate(goMaidChinkoCenter);

            return srcBoneTransform.transform;
        }


        private static void RecurConvertFemaleBoneNameToMale(Maid maid, Transform transform)
        {
            if (transform != null)
            {
                if (transform.name == "Bip01 Spine0a")
                {
                    List<Transform> lstChild = new List<Transform>();
                    for (int i = transform.childCount - 1; i >=0 ; i--)
                    {
                        Transform t = transform.GetChild(i);
                        lstChild.Add(t);

                        t.parent = transform.parent;
                    }

                    if (!_convertedFemaleDisplacedBones.ContainsKey(maid.status.guid))
                    {
                        _convertedFemaleDisplacedBones.Add(maid.status.guid, new Dictionary<Transform, List<Transform>>());
                    }

                    Dictionary<Transform, List<Transform>> maidRearrangeBoneList = _convertedFemaleDisplacedBones[maid.status.guid];
                    maidRearrangeBoneList.Add(transform, lstChild);

                    foreach (var t in lstChild)
                        RecurConvertFemaleBoneNameToMale(maid, t);
                }
                else
                {

                    transform.name = transform.name.Replace("Bip01", "ManBip");
                    transform.name = transform.name.Replace("body001", "mbody");
                    transform.name = transform.name.Replace("Spine1a", "Spine2");

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform t = transform.GetChild(i);
                        RecurConvertFemaleBoneNameToMale(maid, t);
                    }
                }
            }
        }

        private static void RecurRecoverFemaleBoneName(Maid maid, Transform transform)
        {
            if (transform != null)
            {
                if (transform.name == "Bip01 Spine0a")
                {
                    if (_convertedFemaleDisplacedBones.ContainsKey(maid.status.guid))
                    {
                        if (_convertedFemaleDisplacedBones[maid.status.guid].ContainsKey(transform))
                        {
                            List<Transform> lstChild = _convertedFemaleDisplacedBones[maid.status.guid][transform];
                            foreach (Transform t in lstChild)
                            {
                                t.parent = transform;
                            }

                            _convertedFemaleDisplacedBones[maid.status.guid].Remove(transform);
                        }
                    }

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform t = transform.GetChild(i);
                        RecurRecoverFemaleBoneName(maid, t);
                    }
                }
                else
                {

                    transform.name = transform.name.Replace("ManBip", "Bip01");
                    transform.name = transform.name.Replace("mbody", "body001");
                    transform.name = transform.name.Replace("Spine2", "Spine1a");

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform t = transform.GetChild(i);
                        RecurRecoverFemaleBoneName(maid, t);
                    }
                }
            }
        }

        //Convert the name of the bones to female version, in order to apply female used animation
        //Currently work for lower part of the body only.
        private static void RecurConvertManBoneNameToFemale(Transform transform)
        {
            if (transform != null)
            {
                //This part is needed for upper part of the body, but will cause the displacement of the head
                /*
                if (transform.name == "ManBip Spine1")
                {
                    GameObject gp = new GameObject("Bip01 Spine0a");
                    gp.transform.position = transform.position;
                    gp.transform.rotation = transform.rotation;
                    gp.transform.parent = transform.parent;
                    transform.parent = gp.transform;
                }
                */

                transform.name = transform.name.Replace("ManBip", "Bip01");
                transform.name = transform.name.Replace("mbody", "body001");
                transform.name = transform.name.Replace("Spine2", "Spine1a");



                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform t = transform.GetChild(i);
                    RecurConvertManBoneNameToFemale(t);
                }
            }
        }

        private static void RecurRecoverManBoneName(Transform transform)
        {
            if (transform != null)
            {
                transform.name = transform.name.Replace("Bip01", "ManBip");
                transform.name = transform.name.Replace("body001", "mbody");
                transform.name = transform.name.Replace("Spine1a", "Spine2");

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform t = transform.GetChild(i);
                    RecurRecoverManBoneName(t);
                }
            }
        }

    }
}
