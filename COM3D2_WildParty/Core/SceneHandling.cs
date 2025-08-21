using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Shop;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class SceneHandling
    {
        public static void AddObjectToWorld(WorldObject objData)
        {
            GameObject addedObject = null;
            if (objData.IsCustom)
            {
                addedObject = Helper.CustomObjectLoader.GetCustomObject(objData.Src);
            }
            else
            {
                addedObject = GameMain.Instance.BgMgr.AddPrefabToBg(objData.Src, objData.ObjectID, "", Vector3.zero, Vector3.zero);
            }

            if (addedObject != null)
            {

                addedObject.transform.position = objData.PosRot.Pos;
                addedObject.transform.rotation = objData.PosRot.Rot;
                if (objData.Scale > 0)
                    addedObject.transform.localScale = new Vector3(objData.Scale, objData.Scale, objData.Scale);

                if (objData.IsCustom)
                    StateManager.Instance.AddedCustomGameObjectList.Add(objData.ObjectID, addedObject);
                else
                    StateManager.Instance.AddedGameObjectList.Add(objData.ObjectID, addedObject);
            }
        }
    }
}
