using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    class MotionInfo
    {
        public string ScriptFile;
        public string ScriptLabel;
        public string MotionFile;
        public string MotionTag;
        public bool IsLoopMotion = true;

        //If script label is not known, or would like to set the anime to a single character but the anime is supposed to be applied to more than 1 character, set this flag to false
        public bool UseMotionScriptLoad = true;

        public MotionInfo() { }
        
        public MotionInfo(string scriptFile, string scriptLabel, string motionFile, string motionTag, bool isLoop = true, bool UseMotionScriptLoad = true)
        {
            ScriptFile = scriptFile;
            ScriptLabel = scriptLabel;
            MotionFile = motionFile;
            MotionTag = motionTag;
            IsLoopMotion = isLoop;
            this.UseMotionScriptLoad = UseMotionScriptLoad;
        }
    }
}
