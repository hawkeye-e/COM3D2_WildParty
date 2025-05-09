﻿using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    internal class Debug
    {
        private static ManualLogSource Log = WildParty.Log;


        //Function for finding out the skill id we want for each personality (Assumption: the name displayed is unique)
        public static void PrintAvailableSkillForMaid(Maid maid)
        {
            foreach (var list in Yotogis.Skill.skill_data_list)
            {
                foreach (var kvp in list)
                {
                    if (kvp.Value.IsExecMaid(maid.status) && kvp.Value.IsExecPersonal(maid.status.personal))
                    {
                        Log.LogInfo("id: " + kvp.Value.id + ", name: " + kvp.Value.name);
                    }
                }
            }
        }

        //Function for finding out the skill id we want for each personality (Assumption: the name displayed is unique)
        public static void MapAvailableSkillForMaid(Maid maid)
        {
            Dictionary<string, int> mylist = new Dictionary<string, int>();
            mylist.Add("【二人きり】セックス正常位", -1);
            mylist.Add("【二人きり】アナルセックス正常位", -1);
            mylist.Add("【二人きり】セックス後背位", -1);
            mylist.Add("【二人きり】アナルセックス後背位", -1);
            mylist.Add("【二人きり】セックス騎乗位A", -1);
            mylist.Add("【二人きり】アナルセックス騎乗位A", -1);
            mylist.Add("【二人きり】だいしゅきホールドセックス正常位", -1);
            mylist.Add("【二人きり】だいしゅきホールドアナルセックス正常位", -1);
            mylist.Add("【二人きり】抱きつきセックス後背位", -1);
            mylist.Add("【二人きり】抱きつきアナルセックス後背位", -1);
            mylist.Add("【二人きり】グラインドセックス騎乗位", -1);
            mylist.Add("【二人きり】グラインドアナルセックス騎乗位", -1);
            mylist.Add("【二人きり】抱え込み正常位", -1);
            mylist.Add("【二人きり】抱え込みアナル正常位", -1);
            mylist.Add("【二人きり】セックス寄り添い側位", -1);
            mylist.Add("【二人きり】アナルセックス寄り添い側位", -1);
            mylist.Add("【二人きり】セックス腕持ち後背位", -1);
            mylist.Add("【二人きり】アナルセックス腕持ち後背位", -1);
            mylist.Add("【二人きり】セックス背面騎乗位B", -1);
            mylist.Add("【二人きり】アナルセックス背面騎乗位B", -1);
            mylist.Add("【二人きり】寝バックA", -1);
            mylist.Add("【二人きり】アナル寝バックA", -1);
            mylist.Add("【二人きり】まんぐりセックス", -1);
            mylist.Add("【二人きり】まんぐりアナルセックス", -1);
            mylist.Add("【二人きり】セックス開脚側位", -1);
            mylist.Add("【二人きり】アナルセックス開脚側位", -1);
            mylist.Add("【二人きり】立位", -1);
            mylist.Add("【二人きり】アナル立位", -1);
            mylist.Add("【二人きり】セックス見つめ合い対面座位A", -1);
            mylist.Add("【二人きり】アナルセックス見つめ合い対面座位A", -1);
            mylist.Add("【二人きり】セックス手繋ぎ対面騎乗位", -1);
            mylist.Add("【二人きり】アナルセックス手繋ぎ対面騎乗位", -1);
            mylist.Add("【二人きり】膝立ちセックス後背位", -1);
            mylist.Add("【二人きり】膝立ちアナルセックス後背位", -1);
            mylist.Add("【二人きり】ディープ正常位", -1);
            mylist.Add("【二人きり】ディープアナル正常位", -1);
            mylist.Add("【二人きり】セックス開脚背面立位", -1);
            mylist.Add("【二人きり】アナルセックス開脚背面立位", -1);
            mylist.Add("【二人きり】手つなぎ正常位", -1);
            mylist.Add("【二人きり】手つなぎアナル正常位", -1);
            mylist.Add("【二人きり】背面駅弁", -1);
            mylist.Add("【二人きり】背面アナル駅弁", -1);
            mylist.Add("【二人きり】抱え込み座位", -1);
            mylist.Add("【二人きり】抱え込みアナル座位", -1);
            mylist.Add("【二人きり】ブリッジセックス正常位", -1);
            mylist.Add("【二人きり】ブリッジアナルセックス正常位", -1);
            mylist.Add("【二人きり】犬後背位セックス", -1);
            mylist.Add("【二人きり】犬後背位アナルセックス", -1);
            mylist.Add("【二人きり】セックス密着騎乗位", -1);
            mylist.Add("【二人きり】アナルセックス密着騎乗位", -1);
            mylist.Add("【二人きり】松葉崩しセックス", -1);
            mylist.Add("【二人きり】松葉崩しアナルセックス", -1);
            mylist.Add("【二人きり】腕持ち立ち後背位", -1);
            mylist.Add("【二人きり】腕持ちアナル立ち後背位", -1);
            mylist.Add("【二人きり】押さえつけセックス正常位", -1);
            mylist.Add("【二人きり】押さえつけアナルセックス正常位", -1);
            mylist.Add("【二人きり】開脚セックス正常位", -1);
            mylist.Add("【二人きり】開脚アナルセックス正常位", -1);





            //var maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            foreach (var list in Yotogis.Skill.skill_data_list)
            {
                foreach (var kvp in list)
                {
                    if (kvp.Value.IsExecMaid(maid.status) && kvp.Value.IsExecPersonal(maid.status.personal))
                    {
                        if (mylist.ContainsKey(kvp.Value.name))
                        {
                            mylist[kvp.Value.name] = kvp.Value.id;
                        }

                        //Log.LogInfo("id: " + kvp.Value.id + ", name: " + kvp.Value.name);
                    }
                }
            }

            foreach (var kvp in mylist)
            {
                //Log.LogInfo("Name: " + kvp.Key + ", id: " + kvp.Value);
                Log.LogInfo("id: " + kvp.Value);
            }
        }



        public static void PrintDetail(object a)
        {
            if (a == null) return;

            foreach (var prop in a.GetType().GetProperties())
            {
                try
                {

                    object value = prop.GetValue(a, null);
                    if (value != null)
                    {
                        if (value is Transform)
                            Log.Log(LogLevel.Info, prop.Name + "=" + value + ", name: " + ((Transform)value).name + ", active: " + ((Transform)value).gameObject.active);
                        else if (value is GameObject)
                            Log.Log(LogLevel.Info, prop.Name + "=" + value + ", name: " + ((GameObject)value).name + ", active: " + ((GameObject)value).active);
                        else
                            Log.Log(LogLevel.Info, prop.Name + "=" + value);
                    }
                    else
                        Log.Log(LogLevel.Info, prop.Name + " is null!!");

                }
                catch { }
            }

            foreach (var prop in a.GetType().GetFields())
            {
                try
                {

                    object value = prop.GetValue(a);
                    if (value != null)
                    {
                        if (value is Transform)
                            Log.Log(LogLevel.Info, prop.Name + "=" + value + ", name: " + ((Transform)value).name + ", active: " + ((Transform)value).gameObject.active);
                        else if (value is GameObject)
                            Log.Log(LogLevel.Info, prop.Name + "=" + value + ", name: " + ((GameObject)value).name + ", active: " + ((GameObject)value).active);
                        //else if (value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                        //{
                        //    foreach(var s in value)
                        //    {

                        //    }
                        //}
                        else
                            Log.Log(LogLevel.Info, prop.Name + "=" + value);
                    }
                    else
                        Log.Log(LogLevel.Info, prop.Name + " is null!!");

                }
                catch { }
            }

            Log.LogInfo("================");
        }

        public static void PrintTransformTreeNameOnly(Transform t)
        {
            if (t != null)
            {
                Log.LogInfo(t.gameObject.name + ", id: " + t.GetInstanceID() + ", pos: " + t.position + ", local pos: " + t.localPosition + ", rot: " + t.rotation.eulerAngles + ", type: " + t.gameObject.GetType());
                //Debug.PrintDetail(t.)
                
                for (int i = 0; i < t.childCount; i++)
                {
                    PrintTransformTreeNameOnly(t.GetChild(i));
                }
            }
        }

        public static void PrintTransformTreeNameOnly(Transform t, string currentPath)
        {
            if (t != null)
            {
                Log.LogInfo("Path: " + currentPath + ".[" + t.name + "]");

                for (int i = 0; i < t.childCount; i++)
                {
                    PrintTransformTreeNameOnly(t.GetChild(i), currentPath + ".[" + t.name + "]");
                }
            }
        }

        internal static void PrintTransformTree(Transform t, string currentPath = "")
        {
            if (t != null)
            {
                Log.LogInfo("Path: " + currentPath);
                Log.LogInfo("Name: " + t.name);
                Log.LogInfo("Active: Self: " + t.gameObject.activeSelf + ", Hierarchy: " + t.gameObject.activeInHierarchy);
                //Log.LogInfo("Layer: " + t.gameObject.layer);

                var panel = t.GetComponent<UIPanel>();
                if (panel != null)
                {
                    Log.LogInfo("panel sortingOrder: " + panel.sortingOrder  + ", depth: " + panel.depth);
                }
                var widget = t.GetComponent<UIWidget>();
                if (widget != null)
                {
                    Log.LogInfo("widget sortingOrder: " + widget.drawCall?.sortingOrder + ", depth: " + widget.depth);
                }

                //PrintDetail(t.gameObject);
                //PrintDetail(t);
                GetComponentTypes(t);

                //var mono = t.GetComponent<MonoBehaviour>();
                //if (mono != null)
                //{
                //    Log.LogInfo("GetScriptClassName: " + mono.GetScriptClassName());
                //}
                var text = t.GetComponent<Text>();
                if (text != null)
                {
                    Log.LogInfo("Text: " + text.text);
                }

                Log.LogInfo("Position: " + t.position);
                Log.LogInfo("LocalPosition: " + t.localPosition);
                ////Log.LogInfo("Rotation: " + t.rotation.eulerAngles);
                ////Log.LogInfo("LocalRotation: " + t.localRotation.eulerAngles);
                ////var r = t.GetComponent<RectTransform>();
                ////if (r != null)
                ////{
                ////    Log.LogInfo("Width: " + r.rect.width + ", height: " + r.rect.height);
                ////    Log.LogInfo("bottom: " + r.rect.bottom + ", top: " + r.rect.top);
                ////}
                Log.LogInfo("Child Count: " + t.childCount);
                if (t.parent != null)
                    Log.LogInfo("Parent: " + t.parent.name);

                Log.LogInfo("");
                for (int i = 0; i < t.GetChildCount(); i++)
                {
                    Log.LogInfo("Visiting the child of [" + t.name + "]");
                    PrintTransformTree(t.GetChild(i), currentPath + ".[" + t.name + "]");
                }
                Log.LogInfo("");
            }
        }

        private static void GetComponentTypes(Transform t)
        {
            Component[] components = t.gameObject.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                Log.LogInfo(component.ToString());
            }

        }

        internal static void PrintTransformTreeUpward(Transform t, string currentPath = "", string stopAt = null)
        {
            Log.LogInfo("Name: " + t.name + ", active: " + t.gameObject.active);
            GetComponentTypes(t);
            var mono = t.GetComponent<MonoBehaviour>();
            //if (mono != null)
            //{
            //    Log.LogInfo("GetScriptClassName: " + mono.GetScriptClassName());
            //}

            if (t.parent != null && (stopAt == null || t.name != stopAt))
            {
                Log.LogInfo("Visiting the parent of [" + t.name + "]" + ", position: " + t.position);
                PrintTransformTreeUpward(t.parent, "[" + t.name + "]." + currentPath);
            }
        }
    }
}
