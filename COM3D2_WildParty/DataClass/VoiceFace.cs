using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    class VoiceFace
    {
        internal class VoiceFaceEntry
        {
            public List<string> NormalFaceFiles;
            public List<string> EstrusFaceFiles;
            public string NormalFaceBlend;
            public string EstrusFaceBlend;

            public VoiceFaceEntry()
            {
                NormalFaceFiles = new List<string>();
                EstrusFaceFiles = new List<string>();
                
            }
        }


        //Col 0: Personality type, Col 1: MotionType, Col 2: ExcitementLevel, Col 3: Normal status ile, Col 4: Estrus staus file
        //From Col 6 onwards: the motion id for the yotogi skill
        public static Dictionary<string, Dictionary<int, VoiceFace.VoiceFaceEntry>> ReadCSVFile(string resFile)
        {
            Dictionary<string, Dictionary<int, VoiceFace.VoiceFaceEntry>> result = new Dictionary<string, Dictionary<int, VoiceFace.VoiceFaceEntry>>();

            string[] csvRow = resFile.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvRow.Length; i++)
            {
                if (csvRow[i] == "") continue;      //in case empty row
                string[] rowData = csvRow[i].Split(',');

                int eLevel = int.Parse(rowData[1]);

                if (!result.ContainsKey(rowData[0]))
                    result.Add(rowData[0], new Dictionary<int, VoiceFaceEntry>());
                if (!result[rowData[0]].ContainsKey(eLevel)) {
                    result[rowData[0]].Add(eLevel, new VoiceFaceEntry());
                }

                string[] nFiles = rowData[2].Split(';');
                foreach(var s in nFiles)
                {
                    result[rowData[0]][eLevel].NormalFaceFiles.Add(s.Trim());
                }

                string[] eFiles = rowData[3].Split(';');
                foreach (var s in eFiles)
                {
                    result[rowData[0]][eLevel].EstrusFaceFiles.Add(s.Trim());
                }

                result[rowData[0]][eLevel].NormalFaceBlend = rowData[4].Trim();
                result[rowData[0]][eLevel].EstrusFaceBlend = rowData[5].Trim();

            }

            return result;
        }


    }
}
