using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    internal class ForceSexPosInfo
    {
        public int Waiting;
        public int NormalPlay;
        public int Orgasm;

        public enum Type
        {
            Waiting,
            NormalPlay,
            Orgasm
        }
    }
}
