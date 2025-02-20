using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM3D2.WildParty.Plugin.RandomList
{
    internal class Motion
    {
        internal static readonly MotionInfo[] FemaleRestList =
{
            new MotionInfo("work_001.ks", "*しゃがみ＿待機", "syagami_pose_f.anm", "syagami_pose_f.anm"),
            new MotionInfo("ero_scene_001.ks", "*気絶", "ero_scene_kizetu_f.anm", "ero_scene_kizetu_f.anm"),
            new MotionInfo("ero_scene_001.ks", "*ピロトーク_一人", "pillow_talk_f.anm", "pillow_talk_f.anm")
        };

        internal static readonly MotionInfo[] MaleRestList =
        {
            new MotionInfo("", "", "mp_arai_taiki_m.anm", "mp_arai_taiki_m.anm"),
            new MotionInfo("", "", "taimenzai3_taiki_m.anm", "taimenzai3_taiki_m.anm"),
            new MotionInfo("", "", "om_kousoku_aibu_taiki_m.anm", "om_kousoku_aibu_taiki_m.anm")
        };

        internal static readonly MotionInfo[] MaleStandingList =
{
            new MotionInfo("", "", "man_porse01.anm", "man_porse01.anm"),
            new MotionInfo("", "", "man_tati_seigan_m.anm", "man_tati_seigan_m.anm"),
            new MotionInfo("", "", "man_tati_tyarao_m.anm", "man_tati_tyarao_m.anm"),
            new MotionInfo("", "", "man_tati_harawosasuru_m.anm", "man_tati_harawosasuru_m.anm"),
            //new MotionInfo("", "", "poseizi_taiki_m.anm", "poseizi_taiki_m.anm")
        };

        internal static class RandomMotionCode
        {
            internal const string RandomRest = "RandomRest";
            internal const string RandomStanding = "RandomStanding";
        }

        public static MotionInfo GetRandomMotionByCode(string code, bool isMan)
        {
            MotionInfo result = null;

            MotionInfo[] targetList= null;

            if(code == RandomMotionCode.RandomRest)
            {
                if (isMan)
                    targetList = MaleRestList;
                else
                    targetList = FemaleRestList;
            }
            else if(code == RandomMotionCode.RandomStanding)
            {
                targetList = MaleStandingList;
            }

            if(targetList != null)
            {
                int rnd = RNG.Random.Next(targetList.Length);
                result = targetList[rnd];
            }

            return result;
        }
    }
}
