using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    internal class CustomAnimLoader
    {
        private static readonly string[] _customAnimFileNameList =
        {
            "wp_mod_motion_female_seductive.anm",
            "wp_mod_motion_female_long_sitting.anm",
            "wp_mod_motion_female_stand.anm",
            "wp_mod_motion_female_stand_inverse.anm"
        };

        public static byte[] GetAnimData(string fileName)
        {
            byte[] result = null;
            if (fileName.ToLower() == "wp_mod_motion_female_seductive.anm")
                result = ModResources.CustomAnimResources.wp_mod_motion_female_seductive;
            else if (fileName.ToLower() == "wp_mod_motion_female_long_sitting.anm")
                result = ModResources.CustomAnimResources.wp_mod_motion_female_long_sitting;
            else if (fileName.ToLower() == "wp_mod_motion_female_stand.anm")
                result = ModResources.CustomAnimResources.wp_mod_motion_female_stand;
            else if (fileName.ToLower() == "wp_mod_motion_female_stand_inverse.anm")
                result = ModResources.CustomAnimResources.wp_mod_motion_female_stand_inverse;

            return result;
        }

        public static bool IsAnimFileNameCustom(string fileName)
        {
            return _customAnimFileNameList.Contains(fileName.ToLower());
        }
    }
}
