using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    public class BodyWritingSetupInfo
    {
        public BodyPart Part;
        public string Slot;
        public int x;
        public int y;
        public int width;
        public int height;

        public enum BodyPart
        {
            LeftThighFront,
            LeftThighBack,
            RightThighFront,
            RightThighBack,

            BodyFront,
            BodyBack,
        }

        public static Dictionary<BodyPart, List<BodyWritingSetupInfo>> ReadCSVFile(string resFile)
        {
            Dictionary<BodyPart, List<BodyWritingSetupInfo>> result = new Dictionary<BodyPart, List<BodyWritingSetupInfo>>();

            string[] csvRow = resFile.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvRow.Length; i++)
            {
                if (csvRow[i] == "") continue;      //in case empty row
                string[] rowData = csvRow[i].Split(',');

                BodyWritingSetupInfo newItem = new BodyWritingSetupInfo();
                newItem.Part = (BodyPart)Enum.Parse(typeof(BodyPart), rowData[0]);
                newItem.Slot = rowData[1];
                newItem.x = int.Parse(rowData[2], CultureInfo.InvariantCulture);
                newItem.y = int.Parse(rowData[3], CultureInfo.InvariantCulture);
                newItem.width = int.Parse(rowData[4], CultureInfo.InvariantCulture);
                newItem.height = int.Parse(rowData[5], CultureInfo.InvariantCulture);

                if (!result.ContainsKey(newItem.Part))
                    result.Add(newItem.Part, new List<BodyWritingSetupInfo>());

                result[newItem.Part].Add(newItem);

            }

            return result;
        }
    }
}
