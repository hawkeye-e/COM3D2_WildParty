using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    internal class CustomAnimLoader
    {
        public static byte[] GetAnimData(string fileName)
        {
            byte[] result = null;
            if (fileName.ToLower() == "wp_mod_motion_female_seductive.anm")
                result = ModResources.CustomAnimResources.wp_mod_motion_female_seductive;

            return result;
        }
    }
}
