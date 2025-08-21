using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    //This class is to load and instantiate custom prefab that included in the PrefabResources.
    internal class TallyCounterMarker
    {
        internal static readonly Dictionary<string, Bitmap> TallyCounterTextureNames = new Dictionary<string, Bitmap>()
        {
            { "tally_counter_kanji_1_1" , ModResources.ImageResources.tally_counter_kanji_1_1 },
            { "tally_counter_kanji_1_2" , ModResources.ImageResources.tally_counter_kanji_1_2 },
            { "tally_counter_kanji_1_3" , ModResources.ImageResources.tally_counter_kanji_1_3 },
            { "tally_counter_kanji_1_4" , ModResources.ImageResources.tally_counter_kanji_1_4 },
            { "tally_counter_kanji_1_5" , ModResources.ImageResources.tally_counter_kanji_1_5 },

            { "tally_counter_kanji_2_1" , ModResources.ImageResources.tally_counter_kanji_2_1 },
            { "tally_counter_kanji_2_2" , ModResources.ImageResources.tally_counter_kanji_2_2 },
            { "tally_counter_kanji_2_3" , ModResources.ImageResources.tally_counter_kanji_2_3 },
            { "tally_counter_kanji_2_4" , ModResources.ImageResources.tally_counter_kanji_2_4 },
            { "tally_counter_kanji_2_5" , ModResources.ImageResources.tally_counter_kanji_2_5 },

            { "tally_counter_kanji_3_1" , ModResources.ImageResources.tally_counter_kanji_3_1 },
            { "tally_counter_kanji_3_2" , ModResources.ImageResources.tally_counter_kanji_3_2 },
            { "tally_counter_kanji_3_3" , ModResources.ImageResources.tally_counter_kanji_3_3 },
            { "tally_counter_kanji_3_4" , ModResources.ImageResources.tally_counter_kanji_3_4 },
            { "tally_counter_kanji_3_5" , ModResources.ImageResources.tally_counter_kanji_3_5 },

            { "tally_counter_western_1_1" , ModResources.ImageResources.tally_counter_western_1_1 },
            { "tally_counter_western_1_2" , ModResources.ImageResources.tally_counter_western_1_2 },
            { "tally_counter_western_1_3" , ModResources.ImageResources.tally_counter_western_1_3 },
            { "tally_counter_western_1_4" , ModResources.ImageResources.tally_counter_western_1_4 },
            { "tally_counter_western_1_5" , ModResources.ImageResources.tally_counter_western_1_5 },

            { "tally_counter_western_2_1" , ModResources.ImageResources.tally_counter_western_2_1 },
            { "tally_counter_western_2_2" , ModResources.ImageResources.tally_counter_western_2_2 },
            { "tally_counter_western_2_3" , ModResources.ImageResources.tally_counter_western_2_3 },
            { "tally_counter_western_2_4" , ModResources.ImageResources.tally_counter_western_2_4 },
            { "tally_counter_western_2_5" , ModResources.ImageResources.tally_counter_western_2_5 },

            { "tally_counter_western_3_1" , ModResources.ImageResources.tally_counter_western_3_1 },
            { "tally_counter_western_3_2" , ModResources.ImageResources.tally_counter_western_3_2 },
            { "tally_counter_western_3_3" , ModResources.ImageResources.tally_counter_western_3_3 },
            { "tally_counter_western_3_4" , ModResources.ImageResources.tally_counter_western_3_4 },
            { "tally_counter_western_3_5" , ModResources.ImageResources.tally_counter_western_3_5 },
        };

        private const string TextureFileName_KanjiFormat = "tally_counter_kanji_[X]_[Y]";
        private const string TextureFileName_WesternFormat = "tally_counter_western_[X]_[Y]";
        private const int PatternCount = 3;

        internal enum BodySide
        {
            Front,
            Back,
        }

        private enum BodyPart
        {
            LeftThighFront,
            LeftThighBack,
            RightThighFront,
            RightThighBack,
        }

        internal enum TextureType
        {
            Kanji,
            Western,
            None
        }

        private class Position
        {
            internal int x;
            internal int y;

            internal Position() { }

            internal Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private class TallyMarkInfo : Position
        {
            internal float r;
            internal int pattern;
        }

        private class TallyCounterMarkProgress
        {
            internal Dictionary<BodyPart, List<TallyMarkInfo>> MarkInfo = new Dictionary<BodyPart, List<TallyMarkInfo>>();
            internal Dictionary<BodyPart, int> BodyPartMarkProgress = new Dictionary<BodyPart, int>();
        }

        private static readonly Dictionary<BodyPart, List<Position>> BaseTexturePosition = new Dictionary<BodyPart, List<Position>>() 
        {
            { BodyPart.LeftThighFront, 
                new List<Position>{ 
                    new Position(800, 70),
                    new Position(800, 85),
                    new Position(800, 100),
                    new Position(800, 115),
                    new Position(800, 130),
                    new Position(820, 70),
                    new Position(820, 85),
                    new Position(820, 100),
                    new Position(820, 115),
                    new Position(820, 130)
                }
            },

            { BodyPart.LeftThighBack,
                new List<Position>{
                    new Position(980, 70),
                    new Position(980, 85),
                    new Position(980, 100),
                    new Position(980, 115),
                    new Position(980, 130),
                    new Position(1000, 70),
                    new Position(1000, 85),
                    new Position(1000, 100),
                    new Position(1000, 115),
                    new Position(1000, 130)
                }
            },

            { BodyPart.RightThighFront,
                new List<Position>{
                    new Position(190, 70),
                    new Position(190, 85),
                    new Position(190, 100),
                    new Position(190, 115),
                    new Position(190, 130),
                    new Position(210, 70),
                    new Position(210, 85),
                    new Position(210, 100),
                    new Position(210, 115),
                    new Position(210, 130)
                }
            },

            { BodyPart.RightThighBack,
                new List<Position>{
                    new Position(260, 70),
                    new Position(260, 85),
                    new Position(260, 100),
                    new Position(260, 115),
                    new Position(260, 130),
                    new Position(280, 70),
                    new Position(280, 85),
                    new Position(280, 100),
                    new Position(280, 115),
                    new Position(280, 130)
                }
            },
        };
        private const int TallyCounterPartCount = 5;

        private const string SlotName = "body";
        private const int MatNo = 0;
        private const int Layer = 13;            //using the layer of whip mark etc
        private const GameUty.SystemMaterial BlendMode = GameUty.SystemMaterial.Alpha;
        private static readonly string[] PropNameList = { "_MainTex", "_ShadowTex" };
        private const float BaseScale = 0.33f;
        private const float BaseRotation = 0;           //Degree
        private const int MaxRotationDeviation = 500;   //need to divide by 100 as rotation is float
        private const int MaxXDeviation = 3;
        private const int MaxYDeviation = 2;

        private const bool IsAdd = true;
        private const bool NoTransform = false;
        private const float Alpha = 1;
        private const int TargetBodyTexSize = 1024;

        private static Dictionary<Maid, TallyCounterMarkProgress> MarkProgressList = new Dictionary<Maid, TallyCounterMarkProgress>();

        public static void AddTallyCounterMark(Maid maid, TextureType textureType, BodySide bodySide)
        {
            if (textureType == TextureType.None)
                return;

            bool isLeft = RNG.Random.Next(2) == 0;
            if (bodySide == BodySide.Front)
            {
                if (isLeft)
                    AddTallyCounterMark(maid, textureType, BodyPart.LeftThighFront, BodyPart.RightThighFront);
                else
                    AddTallyCounterMark(maid, textureType, BodyPart.RightThighFront, BodyPart.LeftThighFront);
            }
            else
            {
                if (isLeft)
                    AddTallyCounterMark(maid, textureType, BodyPart.LeftThighBack, BodyPart.RightThighBack);
                else
                    AddTallyCounterMark(maid, textureType, BodyPart.RightThighBack, BodyPart.LeftThighBack);
            }
        }

        //Return false if there is no room to add
        private static bool AddTallyCounterMark(Maid maid, TextureType textureType, BodyPart firstBodyPart, BodyPart secondaryBodyPart)
        {
            BodyPart bodyPart = firstBodyPart;

            if (!MarkProgressList.ContainsKey(maid)) {
                InitMarkProgressForMaid(maid);
            }

            var markProgress = MarkProgressList[maid];

            markProgress.BodyPartMarkProgress[bodyPart]++;

            int markNumber = markProgress.BodyPartMarkProgress[bodyPart] / 5;
            int markSubNumber = markProgress.BodyPartMarkProgress[bodyPart] % 5;

            //Stop if all the place in this body part is filled
            if (markProgress.MarkInfo[bodyPart].Count <= markNumber)
            {
                //if no place in body part, will try to add in secondary body part
                bodyPart = secondaryBodyPart;
                markProgress.BodyPartMarkProgress[bodyPart]++;
                markNumber = markProgress.BodyPartMarkProgress[bodyPart] / 5;
                markSubNumber = markProgress.BodyPartMarkProgress[bodyPart] % 5;

                if (markProgress.MarkInfo[bodyPart].Count <= markNumber)
                    return false;
            }

            

            TallyMarkInfo markInfo = markProgress.MarkInfo[bodyPart][markNumber];

            string fileName = GetTextureFileName(textureType, markInfo.pattern, markSubNumber);

            //add texture
            for (int i = 0; i < PropNameList.Length; i++)
            {
                maid.body0.MulTexSet(SlotName, MatNo, PropNameList[i], Layer, fileName, BlendMode, IsAdd,
                    markInfo.x, markInfo.y, markInfo.r, BaseScale, NoTransform, null, Alpha, TargetBodyTexSize);
            }

            maid.body0.MulTexProc("body");

            return true;
        }
        
        public static void RemoveTallyCounterMark(Maid maid)
        {
            for (int i = 0; i < PropNameList.Length; i++)
                maid.body0.MulTexRemove(SlotName, MatNo, PropNameList[i], Layer);
            maid.body0.MulTexProc(SlotName);

            if (MarkProgressList.ContainsKey(maid))
                MarkProgressList.Remove(maid);
        }

        private static void InitMarkProgressForMaid(Maid maid) {
            TallyCounterMarkProgress tallyCounterMarkProgress = new TallyCounterMarkProgress();
            tallyCounterMarkProgress.BodyPartMarkProgress = new Dictionary<BodyPart, int>();
            tallyCounterMarkProgress.MarkInfo = new Dictionary<BodyPart, List<TallyMarkInfo>>();

            foreach (BodyPart p in Enum.GetValues(typeof(BodyPart)))
            {
                tallyCounterMarkProgress.BodyPartMarkProgress.Add(p, -1);

                List<TallyMarkInfo> markPositionList = new List<TallyMarkInfo>();
                for (int i = 0; i < BaseTexturePosition[p].Count; i++)
                    markPositionList.Add(GetRandomizeMarkTexturePosition(p, i));

                tallyCounterMarkProgress.MarkInfo.Add(p, markPositionList);
            }

            MarkProgressList.Add(maid, tallyCounterMarkProgress);
        }

        private static TallyMarkInfo GetRandomizeMarkTexturePosition(BodyPart bodyPart, int markNumber)
        {
            TallyMarkInfo result = new TallyMarkInfo();

            result.x = BaseTexturePosition[bodyPart][markNumber].x;
            result.y = BaseTexturePosition[bodyPart][markNumber].y;
            result.r = BaseRotation;

            result.x += RNG.Random.Next(MaxXDeviation * 2) - MaxXDeviation;
            result.y += RNG.Random.Next(MaxYDeviation * 2) - MaxYDeviation;
            result.r += (float)(RNG.Random.Next(MaxRotationDeviation * 2) - MaxRotationDeviation) / 100f;

            result.pattern = RNG.Random.Next(PatternCount);

            return result;
        }

        private static string GetTextureFileName(TextureType textureType, int patternType, int subMark)
        {
            string fileName = "";
            if (textureType == TextureType.Kanji)
                fileName = TextureFileName_KanjiFormat;
            else if (textureType == TextureType.Western)
                fileName = TextureFileName_WesternFormat;

            fileName = fileName.Replace("[X]", (patternType + 1).ToString()).Replace("[Y]", (subMark + 1).ToString());

            return fileName;
        }
    }
}
