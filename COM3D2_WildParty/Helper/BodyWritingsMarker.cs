using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace COM3D2.WildParty.Plugin.Helper
{
    //This class is to load and instantiate custom prefab that included in the PrefabResources.
    internal class BodyWritingsMarker
    {
        //internal static readonly Dictionary<string, BodyWritingTextureInfo> BodyWritingTextureNames = new Dictionary<string, BodyWritingTextureInfo>()
        //{
        //    { "tally_counter_kanji_1_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_1_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_1_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_1_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_1_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_1_3, TallyCountSizeWidth, TallyCountSizeHeight)},
        //    { "tally_counter_kanji_1_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_1_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_1_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_1_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "tally_counter_kanji_2_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_2_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_2_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_2_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_2_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_2_3, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_2_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_2_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_2_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_2_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "tally_counter_kanji_3_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_3_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_3_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_3_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_3_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_3_3, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_3_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_3_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_kanji_3_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_kanji_3_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "tally_counter_western_1_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_1_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_1_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_1_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_1_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_1_3, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_1_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_1_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_1_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_1_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "tally_counter_western_2_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_2_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_2_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_2_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_2_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_2_3, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_2_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_2_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_2_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_2_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "tally_counter_western_3_1" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_3_1, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_3_2" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_3_2, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_3_3" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_3_3, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_3_4" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_3_4, TallyCountSizeWidth, TallyCountSizeHeight) },
        //    { "tally_counter_western_3_5" , new BodyWritingTextureInfo(ModResources.ImageResources.tally_counter_western_3_5, TallyCountSizeWidth, TallyCountSizeHeight) },

        //    { "body_writing_front_jp_a1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_a1, 160, 30 )},
        //    { "body_writing_front_jp_b2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_b2, 100, 100 )},
        //    { "body_writing_front_jp_c2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_c2, 100, 100 )},
        //    { "body_writing_front_jp_d1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_d1, 160, 60)},
        //    { "body_writing_front_jp_e1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_e1, 160, 60)},
        //    { "body_writing_front_jp_f3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_f3, 100, 120)},
        //    { "body_writing_front_jp_g1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_g1, 100, 120)},
        //    { "body_writing_front_jp_h2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_h2, 100, 240)},
        //    { "body_writing_front_jp_i1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_i1, 100, 240)},
        //    { "body_writing_front_jp_j2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_j2, 100, 200)},
        //    { "body_writing_front_jp_k1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_k1, 100, 200)},
        //    { "body_writing_front_jp_l1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_l1,  80, 160)},
        //    { "body_writing_front_jp_m2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_m2,  80, 160)},
        //    { "body_writing_front_jp_p3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_p3,  60, 140)},
        //    { "body_writing_front_jp_q2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_front_jp_q2,  60, 140)},

        //    { "body_writing_back_jp_1_3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_1_3,  180, 50)},
        //    { "body_writing_back_jp_2_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_2_1,  80, 120)},
        //    { "body_writing_back_jp_3_2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_3_2,  80, 120)},
        //    { "body_writing_back_jp_4_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_4_1,  80, 80)},
        //    { "body_writing_back_jp_5_2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_5_2,  80, 80)},
        //    { "body_writing_back_jp_6_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_6_1,  100, 130)},
        //    { "body_writing_back_jp_7_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_7_1,  100, 130)},
        //    { "body_writing_back_jp_8_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_8_1, 100, 150)},
        //    { "body_writing_back_jp_9_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_9_1, 100, 150)},
        //    { "body_writing_back_jp_10_3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_10_3, 80, 170)},
        //    { "body_writing_back_jp_11_3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_11_3, 80, 170)},

        //    { "body_writing_back_jp_12_2", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_12_2, 50, 160)},
        //    { "body_writing_back_jp_13_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_13_1, 50, 160)},

        //    { "body_writing_back_jp_14_1", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_14_1, 70, 160)},
        //    { "body_writing_back_jp_15_3", new BodyWritingTextureInfo(ModResources.ImageResources.body_writing_back_jp_15_3, 70, 160)},
        //    //{ "body_writing_back_jp_14_1", new BodyWritingTextureInfo(System.Drawing.Color.Red, 70, 160)},

        //};

        private const string TextureFileName_KanjiFormat = "tally_counter_kanji_[X]_[Y]";
        private const string TextureFileName_WesternFormat = "tally_counter_western_[X]_[Y]";
        private const int PatternCount = 3;
        //private const int TallyCountSizeWidth = 50;
        //private const int TallyCountSizeHeight = 50;

        internal enum BodySide
        {
            Front,
            Back,
        }

        //private enum BodyPart
        //{
        //    LeftThighFront,
        //    LeftThighBack,
        //    RightThighFront,
        //    RightThighBack,
        //}

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

        //internal class BodyWritingTextureInfo
        //{
        //    internal Bitmap TextureFile;
        //    //internal int Width;
        //    //internal int Height;

        //    internal BodyWritingTextureInfo(Bitmap file, int w, int h) { 
        //        TextureFile = file;
        //        //Width = w;
        //        //Height = h;
        //    }

        //    //For testing use
        //    internal BodyWritingTextureInfo(System.Drawing.Color color, int width, int height)
        //    {
        //        TextureFile = new Bitmap(width, height);
        //        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(TextureFile))
        //        {
        //            g.Clear(color);
        //        }
        //        //Width = width;
        //        //Height = height;
        //    }
        //}

        private class BodyWritingInfo : Position
        {
            internal float r;
            internal int pattern;
        }

        private class TallyCounterMarkProgress
        {
            internal Dictionary<BodyWritingSetupInfo.BodyPart, List<BodyWritingInfo>> MarkInfo = new Dictionary<BodyWritingSetupInfo.BodyPart, List<BodyWritingInfo>>();
            internal Dictionary<BodyWritingSetupInfo.BodyPart, int> BodyPartMarkProgress = new Dictionary<BodyWritingSetupInfo.BodyPart, int>();

            //This stores the body part that NO text texture is applied
            internal List<string> BodyTextAvailableSlot = new List<string>();
        }

        //private static Dictionary<BodyWritingSetupInfo.BodyPart, List<Position>> BaseTallyCountTexturePosition;

        //private static readonly Dictionary<BodyWritingSetupInfo.BodyPart, List<Position>> BaseTexturePosition = new Dictionary<BodyWritingSetupInfo.BodyPart, List<Position>>() 
        //{
        //    { BodyWritingSetupInfo.BodyPart.LeftThighFront, 
        //        new List<Position>{ 
        //            new Position(800, 70),
        //            new Position(800, 85),
        //            new Position(800, 100),
        //            new Position(800, 115),
        //            new Position(800, 130),
        //            new Position(820, 70),
        //            new Position(820, 85),
        //            new Position(820, 100),
        //            new Position(820, 115),
        //            new Position(820, 130)
        //        }
        //    },

        //    { BodyWritingSetupInfo.BodyPart.LeftThighBack,
        //        new List<Position>{
        //            new Position(980, 70),
        //            new Position(980, 85),
        //            new Position(980, 100),
        //            new Position(980, 115),
        //            new Position(980, 130),
        //            new Position(1000, 70),
        //            new Position(1000, 85),
        //            new Position(1000, 100),
        //            new Position(1000, 115),
        //            new Position(1000, 130)
        //        }
        //    },

        //    { BodyWritingSetupInfo.BodyPart.RightThighFront,
        //        new List<Position>{
        //            new Position(190, 70),
        //            new Position(190, 85),
        //            new Position(190, 100),
        //            new Position(190, 115),
        //            new Position(190, 130),
        //            new Position(210, 70),
        //            new Position(210, 85),
        //            new Position(210, 100),
        //            new Position(210, 115),
        //            new Position(210, 130)
        //        }
        //    },

        //    { BodyWritingSetupInfo.BodyPart.RightThighBack,
        //        new List<Position>{
        //            new Position(260, 70),
        //            new Position(260, 85),
        //            new Position(260, 100),
        //            new Position(260, 115),
        //            new Position(260, 130),
        //            new Position(280, 70),
        //            new Position(280, 85),
        //            new Position(280, 100),
        //            new Position(280, 115),
        //            new Position(280, 130)
        //        }
        //    },
        //};
        private const int TallyCounterPartCount = 5;

        private const string SlotName = "body";
        private const int MatNo = 0;
        //private const int Layer = Constant.TextureLayer.BodyWritingTallyCount;
        private const GameUty.SystemMaterial BlendMode = GameUty.SystemMaterial.Alpha;
        private static readonly string[] PropNameList = { "_MainTex", "_ShadowTex" };
        private const float TallyCountScale = 0.33f;
        private const float BodyTextScale = 1f;
        private const float BaseRotation = 0;           //Degree
        private const int MaxRotationDeviation = 500;   //need to divide by 100 as rotation is float
        private const int MaxXDeviation = 3;
        private const int MaxYDeviation = 2;

        private const bool IsAdd = true;
        private const bool NoTransform = false;
        private const float Alpha = 1;
        private const int TargetBodyTexSize = 1024;

        private static Dictionary<Maid, TallyCounterMarkProgress> MarkProgressList = new Dictionary<Maid, TallyCounterMarkProgress>();

        public static bool IsFileNameExistsInBodyWritingTextureList(string fileName)
        {
            if (ModUseData.BodyWritingTextureList != null)
            {
                foreach (var kvpType in ModUseData.BodyWritingTextureList)
                {
                    foreach (var lang in kvpType.Value)
                    {
                        foreach (var part in lang.TextureData)
                        {
                            if (part.Files != null)
                                if (part.Files.Contains(fileName))
                                    return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void AddTallyCounterMark(Maid maid, TextureType textureType, BodySide bodySide)
        {
            if (textureType == TextureType.None)
                return;

            bool isLeft = RNG.Random.Next(2) == 0;
            if (bodySide == BodySide.Front)
            {
                if (isLeft)
                    AddTallyCounterMark(maid, textureType, BodyWritingSetupInfo.BodyPart.LeftThighFront, BodyWritingSetupInfo.BodyPart.RightThighFront);
                else
                    AddTallyCounterMark(maid, textureType, BodyWritingSetupInfo.BodyPart.RightThighFront, BodyWritingSetupInfo.BodyPart.LeftThighFront);
            }
            else
            {
                if (isLeft)
                    AddTallyCounterMark(maid, textureType, BodyWritingSetupInfo.BodyPart.LeftThighBack, BodyWritingSetupInfo.BodyPart.RightThighBack);
                else
                    AddTallyCounterMark(maid, textureType, BodyWritingSetupInfo.BodyPart.RightThighBack, BodyWritingSetupInfo.BodyPart.LeftThighBack);
            }
        }

        //Return false if there is no room to add
        private static bool AddTallyCounterMark(Maid maid, TextureType textureType, BodyWritingSetupInfo.BodyPart firstBodyPart, BodyWritingSetupInfo.BodyPart secondaryBodyPart)
        {
            BodyWritingSetupInfo.BodyPart bodyPart = firstBodyPart;
            
            if (!MarkProgressList.ContainsKey(maid)) {
                InitMarkProgressForMaid(maid, textureType);
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

            

            BodyWritingInfo markInfo = markProgress.MarkInfo[bodyPart][markNumber];

            string fileName = GetTextureFileName(textureType, markInfo.pattern, markSubNumber);

            //add texture
            for (int i = 0; i < PropNameList.Length; i++)
            {
                maid.body0.MulTexSet(SlotName, MatNo, PropNameList[i], Constant.TextureLayer.BodyWritingTallyCount, fileName, BlendMode, IsAdd,
                    markInfo.x, markInfo.y, markInfo.r, TallyCountScale, NoTransform, null, Alpha, TargetBodyTexSize);
            }

            //TODO: if the subnumber = 4 (a tally count word is completed), Add 2 body text randomly
            if (markSubNumber == 4)
            {
                //Randomly draw 2 slot
                for(int i=0; i<2; i++)
                    AddRandomBodyTextureForSlot(maid, textureType);
            }

            maid.body0.MulTexProc("body");

            return true;
        }

        public static void AddRandomBodyTextureForSlot(Maid maid, TextureType textureType)
        {
            if (!MarkProgressList.ContainsKey(maid))
                return;

            if (MarkProgressList[maid].BodyTextAvailableSlot.Count == 0)
                return;

            int rnd = RNG.Random.Next(MarkProgressList[maid].BodyTextAvailableSlot.Count);
            string slot = MarkProgressList[maid].BodyTextAvailableSlot[rnd];

            AddBodyTextureForSlot(maid, textureType, slot);

        }

        public static void AddBodyTextureForSlot(Maid maid, TextureType textureType, string slot)
        {
            if (!MarkProgressList.ContainsKey(maid))
                return;
            
            var textureInfo = ModUseData.BodyWritingTextureList[Constant.TextureType.BodyWritingWholeBodyText].Where(x => x.Type == textureType.ToString()).First();
            var slotInfo = textureInfo.TextureData.Where(x => x.Slot == slot).First();

            int rnd = RNG.Random.Next(slotInfo.Files.Count);
            string fileName = slotInfo.Files[rnd];

            var slotSetupInfo = ModUseData.BodyWritingBodyTextSetupList.SelectMany(x => x.Value).ToList().Where(x => x.Slot == slot).First();
            
            for (int i = 0; i < PropNameList.Length; i++)
            {
                maid.body0.MulTexSet(SlotName, MatNo, PropNameList[i], Constant.TextureLayer.BodyWritingWholeBodyText, fileName, BlendMode, IsAdd,
                    slotSetupInfo.x, slotSetupInfo.y, BaseRotation, BodyTextScale, NoTransform, null, Alpha, TargetBodyTexSize);
            }

            MarkProgressList[maid].BodyTextAvailableSlot.Remove(slot);
        }
        
        public static void RemoveTallyCounterMark(Maid maid)
        {
            for (int i = 0; i < PropNameList.Length; i++)
                maid.body0.MulTexRemove(SlotName, MatNo, PropNameList[i], Constant.TextureLayer.BodyWritingTallyCount);
            maid.body0.MulTexProc(SlotName);

            if (MarkProgressList.ContainsKey(maid))
                MarkProgressList.Remove(maid);
        }

        private static void InitMarkProgressForMaid(Maid maid, TextureType textureType) {
            TallyCounterMarkProgress tallyCounterMarkProgress = new TallyCounterMarkProgress();
            tallyCounterMarkProgress.BodyPartMarkProgress = new Dictionary<BodyWritingSetupInfo.BodyPart, int>();
            tallyCounterMarkProgress.MarkInfo = new Dictionary<BodyWritingSetupInfo.BodyPart, List<BodyWritingInfo>>();
            tallyCounterMarkProgress.BodyTextAvailableSlot = new List<string>();

            foreach (BodyWritingSetupInfo.BodyPart p in Enum.GetValues(typeof(BodyWritingSetupInfo.BodyPart)))
            {
                if (!ModUseData.BodyWritingTallyCountSetupList.ContainsKey(p))
                    continue;
                
                tallyCounterMarkProgress.BodyPartMarkProgress.Add(p, -1);
                
                List<BodyWritingInfo> markPositionList = new List<BodyWritingInfo>();
                for (int i = 0; i < ModUseData.BodyWritingTallyCountSetupList[p].Count; i++)
                    markPositionList.Add(GetRandomizeMarkTexturePosition(p, i));
                
                tallyCounterMarkProgress.MarkInfo.Add(p, markPositionList);
            }

            //Add all available slots to the slot List
            BodyWritingTextureInfo info = ModUseData.BodyWritingTextureList[Constant.TextureType.BodyWritingWholeBodyText].Where(x => x.Type == textureType.ToString()).First();
            foreach (var textureSlot in info.TextureData)
                tallyCounterMarkProgress.BodyTextAvailableSlot.Add(textureSlot.Slot);

            MarkProgressList.Add(maid, tallyCounterMarkProgress);
        }

        private static BodyWritingInfo GetRandomizeMarkTexturePosition(BodyWritingSetupInfo.BodyPart bodyPart, int markNumber)
        {
            BodyWritingInfo result = new BodyWritingInfo();

            result.x = ModUseData.BodyWritingTallyCountSetupList[bodyPart][markNumber].x;
            result.y = ModUseData.BodyWritingTallyCountSetupList[bodyPart][markNumber].y;
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

        public static int GetFullTallyMarkCount(Maid maid)
        {
            if (maid == null) 
                return 0;
            if (!MarkProgressList.ContainsKey(maid))
                return 0;

            int result = 0;

            foreach (var bodyPartMark in MarkProgressList[maid].BodyPartMarkProgress)
                result += (bodyPartMark.Value + 1) / 5;

            return result;
        }

        public static int GetTallyMarkCount(Maid maid)
        {
            if (maid == null)
                return 0;
            if (!MarkProgressList.ContainsKey(maid))
                return 0;

            int result = 0;

            foreach (var bodyPartMark in MarkProgressList[maid].BodyPartMarkProgress)
                result += bodyPartMark.Value + 1;

            return result;
        }
    }
}
