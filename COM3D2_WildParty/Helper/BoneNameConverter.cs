using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    internal class BoneNameConverter
    {
        internal static void ConvertManStructureToFemale(Maid man)
        {
            RecurConvertManBoneNameToFemale(man.transform);
        }

        internal static void RecoverConvertedManStructure(Maid man)
        {
            RecurRecoverManBoneName(man.transform);
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
