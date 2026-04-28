using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    //This class is supposed to store the info we need for determining the fetish condition
    class YotogiIdleMaidsHandling
    {
        public int TiredThreshold;      //if the maid's Mind below this value, switch to use TiredMotions
        public List<StandardMotionInfo> StandardMotions;
        public List<IdleMotionInfo> TiredMotions;


        public class IdleMotionInfo
        {
            public MotionInfo MotionInfo;
            public PosRotVectorFormat Offset;
            public string VoiceType;
            public int ExciteLevel;                 //Fix the excite level voice type for this motion; Negative value for depending on the excite level of the maid
            public string FaceType;
        }

        public class StandardMotionInfo
        {
            public List<IdleMotionInfo> MotionList;
            [JsonProperty]
            private int[] SensualRange;

            public int MinSensual { get { return SensualRange[0]; } }
            public int MaxSensual { get { return SensualRange[1]; } }
            
        }


    }
}
