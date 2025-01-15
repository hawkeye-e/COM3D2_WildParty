using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //I dont see this part would be changed so leave all those system used magic strings or numbers as they are
    class TagSupportData
    {
        public static ScriptManagerFast.KagTagSupportFast ConvertDictionaryToTagSupportType(Dictionary<string, string> dict){
            ScriptManagerFast.KagTagSupportFast tag = new ScriptManagerFast.KagTagSupportFast();
            foreach (var kvp in dict)
                tag.AddTagProperty(kvp.Key, kvp.Value);

            return tag;
        }

        public static Dictionary<string, string> GetTagForCharacterSelectScreen()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("cancel_label", "*End");
            result.Add("name", "SceneCharacterSelect");
            result.Add("label", "*夜伽キャラクター選択終了");
            result.Add("type", "NewYotogi");
            result.Add("tagname", "scenecall");

            return result;
        }

        public static Dictionary<string, string> GetTagForNoonTimeFinalResultScreen()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("name", "SceneDaily");
            result.Add("label", "*昼リザルト終了");
            result.Add("type", "ResultWorkDaytime");
            result.Add("tagname", "scenecall");

            return result;
        }

        public static Dictionary<string, string> GetTagForNightTimeFinalResultScreen()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("name", "SceneDaily");
            result.Add("label", "*夜リザルト終了");
            result.Add("type", "ResultWorkNight");
            result.Add("tagname", "scenecall");

            return result;
        }

        public static Dictionary<string, string> GetTagForYotogiSkillPlay(string skillID)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("name", "Play");
            result.Add("skillid", skillID);
            result.Add("label", Constant.NextButtonLabel.YotogiPlayEnd);
            result.Add("tagname", "yotogicall");

            return result;
        }


        public static Dictionary<string, string> GetTagPlayBGMRoom()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("fade", "0.5f");

            return result;
        }

        //public static Dictionary<string, string> GetTagForSemenTextureHandling(SemenPattern pattern)
        //{
        //    Dictionary<string, string> result = new Dictionary<string, string>();

        //    result.Add("x", pattern.XRange[0] + ":" + pattern.XRange[1]);
        //    result.Add("y", pattern.YRange[0] + ":" + pattern.YRange[1]);
        //    result.Add("r", pattern.RotRange[0] + ":" + pattern.RotRange[1]);
        //    result.Add("layer", pattern.LayerNo.ToString());
        //    result.Add("matno", pattern.MatNo.ToString());
        //    result.Add("part", pattern.Part);
        //    result.Add("delay", "0");
        //    //result.Add("maid", "0");
        //    result.Add("s", pattern.Scale + ":" + pattern.Scale);
        //    result.Add("res", String.Join(":", pattern.FileName.ToArray()));
        //    result.Add("slot", pattern.Slotname);
        //    result.Add("tagname", "texmuladd");
        //    return result;
        //}

    }
}
