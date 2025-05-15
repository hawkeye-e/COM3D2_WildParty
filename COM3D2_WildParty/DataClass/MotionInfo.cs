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
        public string RandomMotion = "";
        public string CustomMotionFile = "";
        public bool IsLoopMotion = true;
        public bool IsBlend = true;
        public bool IsQueued = false;

        public MotionInfo() { }
        public MotionInfo(string scriptFile, string scriptLabel, string motionFile, string motionTag, bool isLoop = true, bool isBlend = true, bool isQueued = false)
        {
            ScriptFile = scriptFile;
            ScriptLabel = scriptLabel;
            MotionFile = motionFile;
            MotionTag = motionTag;
            IsLoopMotion = isLoop;
            IsBlend = isBlend;
            IsQueued = isQueued;
        }

        public MotionInfo(MotionInfo clone)
        {
            ScriptFile = clone.ScriptFile;
            ScriptLabel = clone.ScriptLabel;
            MotionFile = clone.MotionFile;
            MotionTag = clone.MotionTag;
            IsLoopMotion = clone.IsLoopMotion;
            IsBlend = clone.IsBlend;
            IsQueued = clone.IsQueued;
        }
    }
}
