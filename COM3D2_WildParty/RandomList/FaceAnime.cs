using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.RandomList
{
    internal class FaceAnime
    {
        internal static readonly string[] SmileList = { Constant.FaceAnimeType.GentleSmile, Constant.FaceAnimeType.Smile, Constant.FaceAnimeType.SmileWithEyeClosed };
        internal static readonly string[] OhList = { Constant.FaceAnimeType.Thinking, Constant.FaceAnimeType.Thinking, Constant.FaceAnimeType.Puzzled };
        internal static readonly string[] HornyList =
            { Constant.FaceAnimeType.EroticExcited0, Constant.FaceAnimeType.EroticExcited1, Constant.FaceAnimeType.EroticExcited2, Constant.FaceAnimeType.EroticExcited3,
              Constant.FaceAnimeType.EroticAnticipate, Constant.FaceAnimeType.Aftertaste
            };
        internal static readonly string[] RestList =
            { Constant.FaceAnimeType.Sigh, Constant.FaceAnimeType.CloseEye, Constant.FaceAnimeType.AfterOrgasm1, Constant.FaceAnimeType.AfterOrgasm2,
              Constant.FaceAnimeType.AfterOrgasmExcite1, Constant.FaceAnimeType.AfterOrgasmExcite2
            };
        internal static readonly string[] MaidAsManHornyList =
        {
            Constant.FaceAnimeType.Estrus, Constant.FaceAnimeType.Seduce, Constant.FaceAnimeType.EroticExcited0, Constant.FaceAnimeType.EroticExcited1,
            Constant.FaceAnimeType.EroticExcited2, Constant.FaceAnimeType.EroticExcited3, Constant.FaceAnimeType.EroticLikeable3
        };
        internal static readonly string[] AngryList =
        {
            Constant.FaceAnimeType.Scornful, Constant.FaceAnimeType.Angry_Mu, Constant.FaceAnimeType.Angry, Constant.FaceAnimeType.LittleAngry,
        };


        internal static class FaceAnimeCode
        {
            internal const string RandomSmile = "RandomSmile";
            internal const string RandomOh = "RandomOh";
            internal const string RandomHorny = "RandomHorny";
            internal const string RandomRest = "RandomRest";
            internal const string RandomMaidAsManHorny = "RandomMaidAsManHorny";
            internal const string RandomAngry = "RandomAngry";
        }

        public static string GetFaceBlendString(int exciteValue)
        {
            if (exciteValue > 200)
                return Constant.FaceBlendCode.ExciteLevel4;
            if (exciteValue > 100)
                return Constant.FaceBlendCode.ExciteLevel3;
            if (exciteValue > 0)
                return Constant.FaceBlendCode.ExciteLevel2;
            else
                return Constant.FaceBlendCode.ExciteLevel1;
        }
    }
}
