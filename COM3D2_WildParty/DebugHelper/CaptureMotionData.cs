using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    public class CaptureMotionData
    {
        //Key: Skill ID, 
        public static Dictionary<int, DebugSkillData> MotionSoundDictionary = new Dictionary<int, DebugSkillData>();

        public static string currentMotionFileName;
        public static string currentMotionLabelName;
        public static string currentFaceAnim;
        public static int currentSkillID = -1;

        public class DebugSkillData
        {
            public string SkillName;

            public List<MotionSound> ValidMotionList = new List<MotionSound>();

            public class MotionSound
            {
                public string FileName;
                public string MotionLabel;
                public string Excitement;
                public string SoundFile;
                public bool SoundLoop;
                public string FaceAnim;
                public string Personality;

                public static string GetExcitement(int e)
                {
                    if (e >= 300)
                        return "300";
                    else if (e >= 250)
                        return "250~299";
                    else if (e >= 200)
                        return "200~249";
                    else if (e >= 150)
                        return "150~199";
                    else if (e >= 100)
                        return "100~149";
                    else if (e >= 50)
                        return "50~99";
                    else if (e >= 0)
                        return "0~49";
                    else if (e >= -50)
                        return "-50~0";
                    else
                        return "-100~-51";
                }
            }
        }
    }
}
