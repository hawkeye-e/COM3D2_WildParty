using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.Init
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".Init";

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MaidStatus.Propensity.Data), MethodType.Constructor, new Type[] { typeof(int), typeof(CsvParser), typeof(CsvParser) })]
        private static bool PropensityDataConstructorPre(int id)
        {
            return Patches.IsCreatePropensityData(id);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MaidStatus.Propensity), nameof(MaidStatus.Propensity.CreateData))]
        private static void CreateData()
        {
            Patches.InjectCustomFetish();
        }
    }
}
