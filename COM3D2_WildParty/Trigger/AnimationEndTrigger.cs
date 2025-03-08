using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.Trigger
{
    //When the target starts the defined animation name, execute the event deletgate
    internal class AnimationEndTrigger
    {
        private string TargetGUID;
        private List<string> CurrentAnimationNameList;
        private int ExtraWaitingTimeInSecond = 0;
        private EventDelegate ToBeExecuted;
        private bool IsExecuted = false;

        public AnimationEndTrigger(Maid maid, EventDelegate toBeExecuted, int extraWaitingTimeInSecond = 0)
        {
            CurrentAnimationNameList = new List<string>();

            TargetGUID = maid.status.guid;
            ToBeExecuted = toBeExecuted;
            ExtraWaitingTimeInSecond = extraWaitingTimeInSecond;

            var m_AnimCache = HarmonyLib.Traverse.Create(maid.body0).Field(Constant.DefinedClassFieldNames.TBodyAnimCache).GetValue<Dictionary<string, byte>>();
            foreach (var key in m_AnimCache.Keys)
            {
                if(maid.GetAnimation().IsPlaying(key))
                    CurrentAnimationNameList.Add(key);
            }
        }

        public bool CheckTrigger(Maid maid)
        {
            if (maid == null)
                return false;
            if (IsExecuted)
                return false;
            
            if (maid.status.guid == TargetGUID)
            {

                bool notPlaying = true;
                foreach(var animName in CurrentAnimationNameList)
                {
                    if (maid.GetAnimation().IsPlaying(animName))
                        notPlaying = false;
                }

                if (notPlaying)
                {
                    IsExecuted = true;
                    if (ExtraWaitingTimeInSecond > 0)
                    {
                        TimeEndTrigger trigger = new TimeEndTrigger();
                        trigger.DueTime = DateTime.Now.AddSeconds(StateManager.Instance.AnimationChangeTrigger.ExtraWaitingTimeInSecond);
                        trigger.ToBeExecuted = StateManager.Instance.AnimationChangeTrigger.ToBeExecuted;
                        StateManager.Instance.AnimationChangeTrigger = null;

                        StateManager.Instance.TimeEndTriggerList.Add(trigger);
                    }
                    else
                    {
                        ToBeExecuted.Execute();
                    }

                    return true;
                }

            }
            return false;
        }
    }
}
