using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    class MasturbationMotion
    {
        public string Tag;
        public string FileName;
        public string MotionFileName;
        public string Label;
        public string VoiceType;

        public static class Type
        {
            public const string ManKneeDown = "ManKneeDown";
            public const string MaidOnFloor = "MaidOnFloor";
        }

        public static Dictionary<string, MasturbationMotion> ReadCSVFile(string resFile)
        {
            Dictionary<string, MasturbationMotion> result = new Dictionary<string, MasturbationMotion>();

            string[] csvRow = resFile.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvRow.Length; i++)
            {
                if (csvRow[i] == "") continue;      //in case empty row
                string[] rowData = csvRow[i].Split(',');

                MasturbationMotion newItem = new MasturbationMotion();
                newItem.Tag = rowData[1];
                newItem.FileName = rowData[2];
                newItem.MotionFileName = rowData[3];
                newItem.Label = rowData[4];
                newItem.VoiceType = rowData[5];

                result.Add(rowData[0], newItem);

            }

            return result;
        }
    }

}
