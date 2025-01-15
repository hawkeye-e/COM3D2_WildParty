using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //This class is for handling the yotogi skill that can be played per personality
    class PlayableSkill
    {
        internal class SkillItem
        {
            public int SexPosID;
            public string YotogiSkillID;
            public string DisplayName;
            public int Order;
            public bool IsDialogueAllowed;
        }

        private class CSVParse
        {
            public int SexPosID;
            public string YotogiSkillID;
            public string DisplayName;
            public int Order;
            public string GroupType;
            public int PersonalityType;
            public bool IsDialogueAllowed;
        }


        //Col 0: ID, Col 1: DisplayName, Col 2: FileName, Col 3: Group Type, Col 4: Active flag, Col 5: order, Col 29: Is dialogue allowed flag
        //From Col 6 onwards: the motion id for the yotogi skill
        public static Dictionary<int, Dictionary<string, List<SkillItem>>> ReadSexPosListCSVFile(string resSexPos)
        {
            //Outmost Key: Personality
            //inner key: GroupType (MF, MMF, FFM)
            Dictionary<int, Dictionary<string , List<SkillItem>>> result = new Dictionary<int, Dictionary<string, List<SkillItem>>>();

            List<CSVParse> lstLoadedData = new List<CSVParse>();

            string[] csvHAnimation = resSexPos.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvHAnimation.Length; i++)
            {
                if (csvHAnimation[i] == "") continue;      //in case empty row
                string[] rowData = csvHAnimation[i].Split(',');

                if (rowData[4].ToUpper() != "TRUE")
                {
                    //This is set to be excluded
                    continue;
                }

                CSVParse data = new CSVParse();
                data.SexPosID = int.Parse(rowData[0]);
                data.DisplayName = rowData[1];
                data.GroupType = rowData[3];
                data.Order = int.Parse(rowData[5]);
                data.IsDialogueAllowed = bool.Parse( rowData[29]);
                

                for(int p = 1; p <= Constant.PersonalityCount; p++)
                {
                    int columnIndex = p + 5;
                    int personalityValue = p * 10;

                    if (rowData[columnIndex].Trim() != string.Empty)
                    {
                        AddParseYotogiSkill(lstLoadedData, data, personalityValue, rowData[columnIndex]);
                    }
                }
            }
            var sortedData = lstLoadedData.OrderBy(x => x.Order).ToList();

            foreach (var parseItem in sortedData)
            {
                
                if (!result.ContainsKey(parseItem.PersonalityType))
                {
                    Dictionary<string, List<SkillItem>> newDict = new Dictionary<string, List<SkillItem>>();
                    result.Add(parseItem.PersonalityType, newDict);
                }

                if (!result[parseItem.PersonalityType].ContainsKey(parseItem.GroupType))
                {
                    List<SkillItem> newList = new List<SkillItem>();

                    result[parseItem.PersonalityType].Add(parseItem.GroupType, newList);
                }

                SkillItem newItem = new SkillItem();
                newItem.SexPosID = parseItem.SexPosID;
                newItem.DisplayName = parseItem.DisplayName;
                newItem.Order = parseItem.Order;
                newItem.YotogiSkillID = parseItem.YotogiSkillID;
                newItem.IsDialogueAllowed = parseItem.IsDialogueAllowed;

                result[parseItem.PersonalityType][parseItem.GroupType].Add(newItem);
            }

            return result;
        }

        private static void AddParseYotogiSkill(List<CSVParse> list, CSVParse parse, int personality, string yotogiSkillID)
        {
            CSVParse tmp = new CSVParse();
            tmp.SexPosID = parse.SexPosID;
            tmp.DisplayName = parse.DisplayName;
            tmp.GroupType = parse.GroupType;
            tmp.Order = parse.Order;
            tmp.PersonalityType = personality;
            tmp.YotogiSkillID = yotogiSkillID;
            tmp.IsDialogueAllowed = parse.IsDialogueAllowed;

            list.Add(tmp);
        }

    }
}
