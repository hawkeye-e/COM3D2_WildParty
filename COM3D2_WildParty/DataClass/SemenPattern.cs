using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class SemenPattern
    {
        public string Slotname;
        public string Part;                         //The "Part" variable is supposed to be for calling TagTexMulAdd used. 
        public int MatNo;
        public List<string> PropName;
        public int LayerNo;
        public List<List<string>> FileName;
        public GameUty.SystemMaterial BlendMode;
        public bool Add;
        public List<RangeInt> XRange;
        public List<RangeInt> YRange;
        public List<RangeFloat> RotRange;
        public List<RangeFloat> Scale;
        public bool NoTransform;
        public float Alpha;
        public int TargetBodyTexSize;
        public int SplashCount;

        [JsonProperty]
        private List<string> XRangeRaw;
        [JsonProperty]
        private List<string> YRangeRaw;
        [JsonProperty]
        private List<string> RotRangeRaw;
        [JsonProperty]
        private List<string> FileNameRaw;
        [JsonProperty]
        private List<string> ScaleRaw;

        public SubProp SubProp;



        public void PostInitDataProcess()
        {
            XRange = new List<RangeInt>();
            foreach(var rawData in XRangeRaw)
            {
                string[] raw = rawData.Split(':');
                RangeInt newRange = new RangeInt(int.Parse(raw[0]), int.Parse(raw[1]));
                XRange.Add(newRange);
            }

            YRange = new List<RangeInt>();
            foreach (var rawData in YRangeRaw)
            {
                string[] raw = rawData.Split(':');
                RangeInt newRange = new RangeInt(int.Parse(raw[0]), int.Parse(raw[1]));
                YRange.Add(newRange);
            }

            RotRange = new List<RangeFloat>();
            foreach (var rawData in RotRangeRaw)
            {
                string[] raw = rawData.Split(':');
                RangeFloat newRange = new RangeFloat(float.Parse(raw[0]), float.Parse(raw[1]));
                RotRange.Add(newRange);
            }

            Scale = new List<RangeFloat>();
            foreach (var rawData in ScaleRaw)
            {
                string[] raw = rawData.Split(':');
                RangeFloat newRange = new RangeFloat(float.Parse(raw[0]), float.Parse(raw[1]));
                Scale.Add(newRange);
            }

            FileName = new List<List<string>>();
            foreach (var rawData in FileNameRaw)
            {
                List<string> newItem = new List<string>();
                string[] raw = rawData.Split(':');
                for(int i=0; i<raw.Length; i++)
                    newItem.Add("res:" + raw[i]);
                FileName.Add(newItem);
            }
        }

        public class RangeInt
        {
            public int MaxValue;
            public int MinValue;
            public RangeInt(int min, int max)
            {
                MinValue = min;
                MaxValue = max;
            }
        }
        public class RangeFloat
        {
            public float MaxValue;
            public float MinValue;
            public RangeFloat(float min, float max)
            {
                MinValue = min;
                MaxValue = max;
            }
        }


        public class SlotType
        {
            public const string Head = "head";
            public const string Body = "body";
        }
        public class MaterialType
        {
            public const int Head = 5;
            public const int Body = 0;
        }
        public class PropType
        {
            public const string MainTexture = "_MainTex";
            public const string ShadowTexture = "_ShadowTex";
        }
        public const int SemenLayer = 18;
    }
}
