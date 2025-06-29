using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    class PersonalityVoice
    {
        public List<VoiceEntry> NormalPlayVoice;
        public List<OrgasmScreamEntry> OrgasmScream;
        public List<VoiceEntry> OrgasmWait;
        public List<VoiceEntry> InsertVoice;

        internal class VoiceEntry
        {
            public string Personality;
            public int ExcitementLevel;
            public string MotionType;
            public string NormalFile;
            public string EstrusFile;
            
        }

        internal class OrgasmScreamEntry
        {
            public string Personality;
            public string Type;
            public string FileName;
            public float StartTime;
            public float EndTime;
        }

        internal enum VoiceEntryType
        {
            NormalPlay,
            OrgasmWait,
            Insert,

            Waiting
        }


        //Col 0: Personality type, Col 1: MotionType, Col 2: ExcitementLevel, Col 3: Normal status ile, Col 4: Estrus staus file
        public static List<VoiceEntry> ReadVoiceEntryCSVFile(string voiceRes)
        {
            List<VoiceEntry> result = new List<VoiceEntry>();

            string[] csvVoice = voiceRes.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvVoice.Length; i++)
            {
                if (csvVoice[i] == "") continue;      //in case empty row
                string[] rowData = csvVoice[i].Split(',');

                VoiceEntry data = new VoiceEntry();
                data.Personality = rowData[0];
                data.MotionType = rowData[1];
                data.ExcitementLevel = int.Parse(rowData[2]);
                data.NormalFile = rowData[3];
                data.EstrusFile = rowData[4];

                result.Add(data);
            }

            return result;
        }

        public static List<OrgasmScreamEntry> ReadOrgasmScreamCSVFile(string voiceRes)
        {
            List<OrgasmScreamEntry> result = new List<OrgasmScreamEntry>();

            string[] csvVoice = voiceRes.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvVoice.Length; i++)
            {
                if (csvVoice[i] == "") continue;      //in case empty row
                string[] rowData = csvVoice[i].Split(',');

                OrgasmScreamEntry data = new OrgasmScreamEntry();
                data.Personality = rowData[0];
                data.Type = rowData[1];
                data.FileName = rowData[2];
                data.StartTime = float.Parse(rowData[3], CultureInfo.InvariantCulture);
                data.EndTime = float.Parse(rowData[4], CultureInfo.InvariantCulture);

                result.Add(data);
            }

            return result;
        }
    }
}
