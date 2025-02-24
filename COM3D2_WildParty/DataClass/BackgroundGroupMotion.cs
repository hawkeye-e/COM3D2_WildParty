using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //In this class we only focus on all possible motion for the background motion. we dont care about motion per personality here
    class BackgroundGroupMotion
    {
        internal class MotionItem
        {
            public int ID;                          //SexPosID, not the same as the yotogi skill ID the game used
            public string FileName;
            public List<MotionLabel> ValidLabels;
            public List<MotionSpecialLabel> SpecialLabels;
            public int Phase;
            public bool IsBGGroupUse;               //True: will be used by automated group; False: Not used by automated group. The labels information would be used by main group so have to leave a record here
            public List<int> MaidIndex;
            public List<int> ManIndex;
        }

        internal class MotionLabel
        {
            public int ID;
            public string LabelName;
            public int ExcitementLevel;
            public string VoiceType1;
            public string VoiceType2;
            public int LabelGroupID;
        }
        
        
        //Col 0: ID, Col 1: DisplayName, Col 2: FileName, Col 3: Group Type, Col 4: Active flag
        public static Dictionary<string, List<MotionItem>> ReadSexPosListCSVFile(string resSexPos, string resLabel)
        {
            List<MotionLabel> lstMotionLabels = ReadSexPosLabelsCSVFile(resLabel);

            Dictionary<string, List<MotionItem>> result = new Dictionary<string, List<MotionItem>>();

            List<MotionItem> lstMF = new List<MotionItem>();
            List<MotionItem> lstMMF = new List<MotionItem>();
            List<MotionItem> lstMMMF = new List<MotionItem>();
            List<MotionItem> lstFFM = new List<MotionItem>();

            result.Add(Constant.GroupType.MF, lstMF);
            result.Add(Constant.GroupType.MMF, lstMMF);
            result.Add(Constant.GroupType.MMMF, lstMMMF);
            result.Add(Constant.GroupType.FFM, lstFFM);

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

                MotionItem data = new MotionItem();
                data.ID = int.Parse(rowData[0]);
                data.FileName = rowData[2];
                data.Phase = int.Parse(rowData[32]);
                data.IsBGGroupUse = bool.Parse(rowData[31]);

                string maidIndexRaw = rowData[33];
                string manIndexRaw = rowData[34];

                string[] maidIndexSplit = maidIndexRaw.Split(';');
                data.MaidIndex = new List<int>();
                for (int j = 0; j < maidIndexSplit.Length; j++)
                    data.MaidIndex.Add(int.Parse(maidIndexSplit[j]));

                string[] manIndexSplit = manIndexRaw.Split(';');
                data.ManIndex = new List<int>();
                for (int j = 0; j < manIndexSplit.Length; j++)
                    data.ManIndex.Add(int.Parse(manIndexSplit[j]));

                data.ValidLabels = lstMotionLabels.Where(x => x.ID == data.ID).ToList();

                switch (rowData[3].ToUpper())
                {
                    case Constant.GroupType.MF:
                        lstMF.Add(data);
                        break;
                    case Constant.GroupType.MMF:
                        lstMMF.Add(data);
                        break;
                    case Constant.GroupType.MMMF:
                        lstMMMF.Add(data);
                        break;
                    case Constant.GroupType.FFM:
                        lstFFM.Add(data);
                        break;
                }
                
            }

            return result;
        }


        //Col 0: ID, Col 1: ExcitementLevel, Col 2: Label name, Col 3: Voice Type 1, Col 4: Voice Type 2, Col 5: Label Group ID
        private static List<MotionLabel> ReadSexPosLabelsCSVFile(string resourcesString)
        {
            List<MotionLabel> result = new List<MotionLabel>();

            string[] csvLabel = resourcesString.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvLabel.Length; i++)
            {
                if (csvLabel[i] == "") continue;      //in case empty row
                string[] rowData = csvLabel[i].Split(',');

                MotionLabel data = new MotionLabel();
                data.ID = int.Parse(rowData[0]);
                data.ExcitementLevel = int.Parse(rowData[1]);
                data.LabelName= rowData[2];
                data.VoiceType1 = rowData[3];
                data.VoiceType2 = rowData[4];
                data.LabelGroupID = int.Parse(rowData[5]);

                result.Add(data);
            }

            return result;
        }

    }
}
