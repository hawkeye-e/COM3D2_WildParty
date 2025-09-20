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
        public const int FullBodyTextAreaCount = 30;
        private const string TextureFileName_KanjiFormat = "tally_counter_kanji_[X]_[Y]";
        private const string TextureFileName_WesternFormat = "tally_counter_western_[X]_[Y]";
        private const int PatternCount = 3;
        private static Config _config = new Config();

        private class Config
        {
            public bool IsEnableBodyWritings;
            public TextureType TallyCountType;
            public TextureType BodyTextType;
            public bool IsApplyFullBodyText;
        }

        internal enum BodySide
        {
            Front,
            Back,
        }

        internal enum TextureType
        {
            Kanji,
            Western,
            //None
        }

        internal enum Scope
        {
            Default,
            AllNtrScenario,
            None,
        }

        internal enum Level
        {
            TallyCountOnly,
            FullBodyText
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

        private const int TallyCounterPartCount = 5;

        private const string SlotName = "body";
        private const int MatNo = 0;
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

        //public static void AddTallyCounterMark(Maid maid, TextureType textureType, BodySide bodySide)
        public static void AddTallyCounterMark(Maid maid, BodySide bodySide)
        {
            if (!_config.IsEnableBodyWritings)
                return;

            bool isLeft = RNG.Random.Next(2) == 0;
            if (bodySide == BodySide.Front)
            {
                if (isLeft)
                    AddTallyCounterMark(maid, BodyWritingSetupInfo.BodyPart.LeftThighFront, BodyWritingSetupInfo.BodyPart.RightThighFront);
                else
                    AddTallyCounterMark(maid, BodyWritingSetupInfo.BodyPart.RightThighFront, BodyWritingSetupInfo.BodyPart.LeftThighFront);
            }
            else
            {
                if (isLeft)
                    AddTallyCounterMark(maid, BodyWritingSetupInfo.BodyPart.LeftThighBack, BodyWritingSetupInfo.BodyPart.RightThighBack);
                else
                    AddTallyCounterMark(maid, BodyWritingSetupInfo.BodyPart.RightThighBack, BodyWritingSetupInfo.BodyPart.LeftThighBack);
            }
        }

        //Return false if there is no room to add
        private static bool AddTallyCounterMark(Maid maid, BodyWritingSetupInfo.BodyPart firstBodyPart, BodyWritingSetupInfo.BodyPart secondaryBodyPart)
        {
            BodyWritingSetupInfo.BodyPart bodyPart = firstBodyPart;
            
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

            

            BodyWritingInfo markInfo = markProgress.MarkInfo[bodyPart][markNumber];

            string fileName = GetTextureFileName(_config.TallyCountType, markInfo.pattern, markSubNumber);

            //add texture
            for (int i = 0; i < PropNameList.Length; i++)
            {
                maid.body0.MulTexSet(SlotName, MatNo, PropNameList[i], Constant.TextureLayer.BodyWritingTallyCount, fileName, BlendMode, IsAdd,
                    markInfo.x, markInfo.y, markInfo.r, TallyCountScale, NoTransform, null, Alpha, TargetBodyTexSize);
            }

            //if the subnumber = 4 (a tally count word is completed), Add 2 body text randomly
            if (_config.IsApplyFullBodyText)
            {
                if (markSubNumber == 4)
                {
                    //Randomly draw 2 slot
                    for (int i = 0; i < 2; i++)
                        AddRandomBodyTexture(maid, _config.BodyTextType);
                }
            }

            maid.body0.MulTexProc("body");

            return true;
        }

        public static void AddRandomBodyTexture(Maid maid, TextureType textureType)
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

        internal static void InitMarkProgressForMaid(Maid maid) {
            if (MarkProgressList.ContainsKey(maid))
                return;

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
            BodyWritingTextureInfo info = ModUseData.BodyWritingTextureList[Constant.TextureType.BodyWritingWholeBodyText].Where(x => x.Type == _config.BodyTextType.ToString()).First();
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

        public static int GetBodyTextCount(Maid maid)
        {
            if (maid == null)
                return 0;
            if (!MarkProgressList.ContainsKey(maid))
                return 0;

            return FullBodyTextAreaCount - MarkProgressList[maid].BodyTextAvailableSlot.Count;
        }

        public static int GetBodyTextAvailableSlotCount(Maid maid)
        {
            if (maid == null)
                return 0;
            if (!MarkProgressList.ContainsKey(maid))
                return 0;

            return MarkProgressList[maid].BodyTextAvailableSlot.Count;
        }

        public static void SetUpBodyWritingSystem(bool isEnable, TextureType tallyCountType, TextureType bodyTextType, bool isApplyFullBodyText)
        {
            _config.IsEnableBodyWritings = isEnable;
            _config.TallyCountType = tallyCountType;
            _config.BodyTextType = bodyTextType;
            _config.IsApplyFullBodyText = isApplyFullBodyText;
        }

        public static bool IsEnableBodyWritings
        {
            get
            {
                return _config.IsEnableBodyWritings;
            }
        }
    }
}
