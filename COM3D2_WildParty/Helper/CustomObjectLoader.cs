using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    //This class is to load and instantiate custom prefab that included in the PrefabResources.
    internal class CustomObjectLoader
    {
        private const string IMAGE_OBJECT_PREFIX = "Thumb";
        private const string NAME_TAG_PARENT_PREFIX = "Canvas";
        private const string NAME_TAG_OBJECT = "NameText";
        private const string NAME_PLATE_OBJECT_PREFIX = "NamePlate";
        private const string HEART_OBJECT_PREFIX = "Heart";

        private class CustomPrefabData
        {
            public byte[] PrefabData;
            public byte[] MaterialData;
            public string AssetName;

            public CustomPrefabData(byte[] data, byte[] mat, string asset)
            {
                PrefabData = data;
                MaterialData = mat;
                AssetName = asset;
            }
        }


        private static Dictionary<string, CustomPrefabData> CustomPrefabList = new Dictionary<string, CustomPrefabData>
        {
            { "WallAssWall", new CustomPrefabData(ModResources.PrefabResources.wallass, ModResources.PrefabResources.wallassmat, "wall_ass") },
        };

        public static GameObject GetCustomObject(string prefabID)
        {
            GameObject result = null;
            if (CustomPrefabList.ContainsKey(prefabID))
                result = InstantiatePrefab(prefabID);

            CheckSpecialHandling(prefabID, result);

            return result;
        }

        private static void CheckSpecialHandling(string prefabID, GameObject gameObject)
        {
            if (!CustomPrefabList.ContainsKey(prefabID) || gameObject == null)
                return;

            if (prefabID == "WallAssWall")
            {
                //TODO: make it in a loop
                //special handling for buttock wall, add thumbnail to the object
                for (int i=0; i < StateManager.Instance.SelectedMaidsList.Count; i++)
                {
                    Maid maid = StateManager.Instance.SelectedMaidsList[i];
                    var thumbTransform = gameObject.transform.Find(IMAGE_OBJECT_PREFIX + i.ToString());
                    if (thumbTransform != null)
                    {
                        Texture2D card = maid.GetThumCard();
                        if (card != null)
                        {
                            var rend = thumbTransform.gameObject.GetComponent<Renderer>();
                            rend.material.mainTexture = card;
                        }
                    }

                    var nameTextParentTransform = gameObject.transform.Find(NAME_TAG_PARENT_PREFIX + i.ToString());
                    if (nameTextParentTransform != null)
                    {
                        var nameTextTransform = nameTextParentTransform.Find(NAME_TAG_OBJECT);
                        var nameText = nameTextTransform.gameObject.GetComponent<UnityEngine.UI.Text>();
                        nameText.text = maid.status.callName;
                    }

                }

                for (int i= StateManager.Instance.SelectedMaidsList.Count; i < 5; i++)
                {
                    var thumbTransform = gameObject.transform.Find(IMAGE_OBJECT_PREFIX + i.ToString());
                    thumbTransform.localScale = Vector3.zero;

                    var nameTextParentTransform = gameObject.transform.Find(NAME_TAG_PARENT_PREFIX + i.ToString());
                    nameTextParentTransform.localScale = Vector3.zero;

                    var heartTransform = gameObject.transform.Find(HEART_OBJECT_PREFIX + i.ToString());
                    heartTransform.localScale = Vector3.zero;

                    var namePlateTransform = gameObject.transform.Find(NAME_PLATE_OBJECT_PREFIX + i.ToString());
                    namePlateTransform.localScale = Vector3.zero;
                }
                
            }
        }



        private static GameObject InstantiatePrefab(string prefabID)
        {
            CustomPrefabData prefabData = CustomPrefabList[prefabID];

            var materialsAB = AssetBundle.LoadFromMemory(prefabData.MaterialData);
            var prefabAB = AssetBundle.LoadFromMemory(prefabData.PrefabData);

            GameObject go = Util.InstantiateFromBundle(prefabAB, prefabData.AssetName);

            //Update the shader to use the custom shader of the game so that the texture is displayed correctly.
            Shader defaultShader = Shader.Find(Constant.DefaultShader);
            RecurSetShader(go.transform, defaultShader);

            go.SetActive(true);

            prefabAB.Unload(false);
            materialsAB.Unload(false);

            return go;
        }

        private static void RecurSetShader(Transform transform, Shader shader)
        {
            var rend = transform.gameObject.GetComponent<Renderer>();
            if (rend != null)
                if (rend.material != null)
                    rend.material.shader = shader;
            for (int i = 0; i < transform.childCount; i++)
            {
                RecurSetShader(transform.GetChild(i), shader);
            }
        }
    }
}
