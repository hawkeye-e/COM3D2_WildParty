﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class MotionSpecialLabel
    {
        public int SexPosID;
        public string Label;
        public string Type;                 //For the sex flow use
        public string SemenType1;           //For determine where the semen texture goes?
        public string SemenType2;
        public string SemenType3;
        public SemenTarget SemenTarget1;
        public SemenTarget SemenTarget2;
        public SemenTarget SemenTarget3;

        public string VoiceType1;           //Maid1
        public string VoiceType2;           //Maid2

        public string WaitLabel1;           //Maid1
        public string WaitLabel2;           //Maid2

        public string OrgasmType;

        public static class LabelType
        {
            public const string Waiting = "Waiting";
            public const string Insert = "Insert";
            public const string Orgasm = "Orgasm";
        }

        public enum SemenTarget
        {
            None,
            Maid1,
            Maid2
        }

        public static List<MotionSpecialLabel> ReadCSVFile(string resFile)
        {
            List<MotionSpecialLabel> result = new List<MotionSpecialLabel>();

            string[] csvRow = resFile.Split('\n');
            //first line is header so we start with i = 1
            for (int i = 1; i < csvRow.Length; i++)
            {
                if (csvRow[i] == "") continue;      //in case empty row
                string[] rowData = csvRow[i].Split(',');

                MotionSpecialLabel newItem = new MotionSpecialLabel();
                newItem.SexPosID = int.Parse(rowData[0]);
                newItem.Label = rowData[1];
                newItem.Type = rowData[2];
                newItem.SemenType1 = rowData[3];
                newItem.SemenType2 = rowData[4];
                newItem.SemenType3 = rowData[5];

                if (string.IsNullOrEmpty(rowData[6]))
                    newItem.SemenTarget1 = SemenTarget.None;
                else
                    newItem.SemenTarget1 = (SemenTarget)Enum.Parse(typeof(SemenTarget), rowData[6], true);

                if (string.IsNullOrEmpty(rowData[7]))
                    newItem.SemenTarget2 = SemenTarget.None;
                else
                    newItem.SemenTarget2 = (SemenTarget)Enum.Parse(typeof(SemenTarget), rowData[7], true);

                if (string.IsNullOrEmpty(rowData[8]))
                    newItem.SemenTarget3 = SemenTarget.None;
                else
                    newItem.SemenTarget3 = (SemenTarget)Enum.Parse(typeof(SemenTarget), rowData[8], true);

                newItem.VoiceType1 = rowData[9];
                newItem.VoiceType2 = rowData[10];

                newItem.WaitLabel1 = rowData[11];
                newItem.WaitLabel2 = rowData[12];

                newItem.OrgasmType = rowData[13];

                result.Add(newItem);

            }

            return result;
        }
    }


}
