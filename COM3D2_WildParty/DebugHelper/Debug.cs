using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    internal class Debug
    {
        private static ManualLogSource Log = WildParty.Log;



        //Function for finding out the skill id we want for each personality (Assumption: the name displayed is unique)
        public static void MapAvailableSkillForMaid(Maid maid)
        {
            Dictionary<string, int> mylist = new Dictionary<string, int>();
            mylist.Add("寝取らせ輪姦4P奉仕", -1);
            mylist.Add("寝取らせ輪姦3P両穴駅弁", -1);
            mylist.Add("寝取らせ輪姦3P両穴後背位", -1);
            mylist.Add("寝取らせ輪姦3P両穴セックス", -1);
            mylist.Add("【ドスケベビッチ】乱交4P奉仕", -1);
            mylist.Add("【ドスケベビッチ】乱交3P2穴駅弁", -1);
            mylist.Add("【ドスケベビッチ】乱交4P台上正常位", -1);
            mylist.Add("【ドスケベビッチ】乱交4P台上アナル正常位", -1);




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
            var c1 = t.GetComponent<Renderer>(); if (c1 != null) Log.LogInfo("has Renderer");
            var c2 = t.GetComponent<MeshFilter>(); if (c2 != null) Log.LogInfo("has MeshFilter");
            var c3 = t.GetComponent<LODGroup>(); if (c3 != null) Log.LogInfo("has LODGroup");
            var c4 = t.GetComponent<Behaviour>(); if (c4 != null) Log.LogInfo("has Behaviour");

            var c5 = t.GetComponent<Transform>(); if (c5 != null) Log.LogInfo("has Transform");
            var c28 = t.GetComponent<RectTransform>(); if (c28 != null) Log.LogInfo("has RectTransform");
            //var c6 = t.GetComponent<CanvasRenderer>(); if (c6 != null) Log.LogInfo("has CanvasRenderer");
            var c7 = t.GetComponent<Component>(); if (c7 != null) Log.LogInfo("has Component");
            var c8 = t.GetComponent<RectTransform>(); if (c8 != null) Log.LogInfo("has RectTransform");

            var c9 = t.GetComponent<BillboardRenderer>(); if (c9 != null) Log.LogInfo("has BillboardRenderer");
            var c10 = t.GetComponent<LineRenderer>(); if (c10 != null) Log.LogInfo("has LineRenderer");
            var c11 = t.GetComponent<SkinnedMeshRenderer>(); if (c11 != null) Log.LogInfo("has SkinnedMeshRenderer");
            var c12 = t.GetComponent<MeshRenderer>(); if (c12 != null) Log.LogInfo("has MeshRenderer");
            var c13 = t.GetComponent<SpriteRenderer>(); if (c13 != null) Log.LogInfo("has SpriteRenderer");
            var c14 = t.GetComponent<Animator>(); if (c14 != null) Log.LogInfo("has Animator");
            var c15 = t.GetComponent<MonoBehaviour>(); if (c15 != null) Log.LogInfo("has MonoBehaviour");

            var c17 = t.GetComponent<VerticalLayoutGroup>(); if (c17 != null) Log.LogInfo("has VerticalLayoutGroup");
            var c18 = t.GetComponent<HorizontalLayoutGroup>(); if (c18 != null) Log.LogInfo("has HorizontalLayoutGroup");
            var c19 = t.GetComponent<LayoutGroup>(); if (c19 != null) Log.LogInfo("has LayoutGroup");
            var c20 = t.GetComponent<GridLayoutGroup>(); if (c20 != null) Log.LogInfo("has GridLayoutGroup");
            var c21 = t.GetComponent<ContentSizeFitter>(); if (c21 != null) Log.LogInfo("has ContentSizeFitter");
            //var c22 = t.GetComponent<Canvas>(); if (c22 != null) Log.LogInfo("has Canvas");
            var c23 = t.GetComponent<ContentSizeFitter>(); if (c23 != null) Log.LogInfo("has ContentSizeFitter");
            
            var c24 = t.GetComponent<Toggle>(); if (c24 != null) Log.LogInfo("has Toggle");
            //var c25 = t.GetComponent<UI_ToggleEx>(); if (c25 != null) Log.LogInfo("has UI_ToggleEx");
            //var c26 = t.GetComponent<UI_ToggleOnOffEx>(); if (c26 != null) Log.LogInfo("has UI_ToggleOnOffEx");
            var c27 = t.GetComponent<Selectable>(); if (c27 != null) Log.LogInfo("has Selectable");
            var c29 = t.GetComponent<Button>(); if (c29 != null) Log.LogInfo("has Button");
            var c30 = t.GetComponent<UnityEngine.EventSystems.EventTrigger>(); if (c30 != null) Log.LogInfo("has EventTrigger");
            var c31 = t.GetComponent<Image>(); if (c31 != null) Log.LogInfo("has Image");
            var c32 = t.GetComponent<Text>(); if (c32 != null) Log.LogInfo("has Text");

            var c33 = t.GetComponent<UIButton>(); if (c33 != null) Log.LogInfo("has UIButton");
            var c34 = t.GetComponent<UILabel>(); if (c34 != null) Log.LogInfo("has UILabel");
            //var c35 = t.GetComponent<BoxCollider>(); if (c35 != null) Log.LogInfo("has BoxCollider");
            var c36 = t.GetComponent<UIPanel>(); if (c36 != null) Log.LogInfo("has UIPanel");
            var c37 = t.GetComponent<UIScrollView>(); if (c37 != null) Log.LogInfo("has UIScrollView");
            var c38 = t.GetComponent<UIScrollBar>(); if (c38 != null) Log.LogInfo("has UIScrollBar");
            var c39 = t.GetComponent<UIRect>(); if (c39 != null) Log.LogInfo("has UIRect");
            var c40 = t.GetComponent<UIWidget>(); if (c40 != null) Log.LogInfo("has UIWidget");
            var c41 = t.GetComponent<UISprite>(); if (c41 != null) Log.LogInfo("has UISprite");
            var c42 = t.GetComponent<UITexture>(); if (c42 != null) Log.LogInfo("has UITexture");
            var c43 = t.GetComponent<UIDrawCall>(); if (c43 != null) Log.LogInfo("has UIDrawCall");
            var c44 = t.GetComponent<UIEventTrigger>(); if (c44 != null) Log.LogInfo("has UIEventTrigger");
            var c45 = t.GetComponent<UIEventListener>(); if (c45 != null) Log.LogInfo("has UIEventListener");
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
