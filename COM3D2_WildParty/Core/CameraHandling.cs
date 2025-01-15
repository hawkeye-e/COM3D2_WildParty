using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.Core
{
    class CameraHandling
    {
        //Try to focus on the main maid
        internal static void SetCameraLookAt(Maid maid)
        {
            SetCameraLookAt(maid, ConfigurableValue.YotogiCameraWork.CameraVectorOffset, ConfigurableValue.YotogiCameraWork.TargetVectorOffset, ConfigurableValue.YotogiCameraWork.CameraDistance,
                ConfigurableValue.YotogiCameraWork.DefaultHorizontalRotation, ConfigurableValue.YotogiCameraWork.DefaultVerticalRotation);
        }

        internal static void SetCameraLookAt(Maid maid, Vector3 offset, Vector3 targetOffset, float distance = 0f, float horizontalRotation = 0f, float vecticalRotation = 0f)
        {
            if (maid != null)
            {
                Vector3 cameraPos = maid.transform.position + offset;
                Vector3 targetPos = maid.transform.position + targetOffset;
                
                GameMain.Instance.MainCamera.SetPos(cameraPos);
                GameMain.Instance.MainCamera.SetTargetPos(targetPos);
                GameMain.Instance.MainCamera.SetDistance(distance);

                GameMain.Instance.MainCamera.SetAroundAngle(new Vector2(horizontalRotation, vecticalRotation));
            }

        }

        internal static void AnimateCameraToLookAt(Maid maid)
        {
            AnimateCameraToLookAt(maid, ConfigurableValue.YotogiCameraWork.TargetVectorOffset, ConfigurableValue.YotogiCameraWork.CameraDistance,
                ConfigurableValue.YotogiCameraWork.DefaultHorizontalRotation, ConfigurableValue.YotogiCameraWork.DefaultVerticalRotation);
        }

        internal static void AnimateCameraToLookAt(Maid maid, Vector3 offset, float distance = 0f, float horizontalRotation = 0f, float verticalRotation = 0f, float animationTime = 2f, string easeType = Constant.CameraEaseType.EaseOutCubic)
        {
            if (maid != null)
            {
                Vector3 cameraPos = maid.transform.position + offset;

                AnimateCameraToLookAt(cameraPos, distance, horizontalRotation, verticalRotation, animationTime, easeType);
            }
        }

        internal static void AnimateCameraToLookAt(Vector3 targetPosition, float distance = 0f, float horizontalRotation = 0f, float verticalRotation = 0f, float animationTime = 2f, string easeType = Constant.CameraEaseType.EaseOutCubic)
        {
            Vector2 vector = GameMain.Instance.MainCamera.GetAroundAngle();
            vector = new Vector2(Util.RoundDegree(vector.x), Util.RoundDegree(vector.y));
            Vector2 aroundAngle = new Vector2(horizontalRotation, verticalRotation);

            //Follows the logic of WfCameraMoveSupport.StartCameraPosition. It is best to keep the around angle value between [0,360] or else it will rotate more than 1 full circle which looks weird.
            if (Mathf.Abs(aroundAngle.y - 360f - vector.y) < Mathf.Abs(aroundAngle.y - vector.y))
            {
                aroundAngle.y -= 360f;
            }
            else if (Mathf.Abs(aroundAngle.y + 360f - vector.y) < Mathf.Abs(aroundAngle.y - vector.y))
            {
                aroundAngle.y += 360f;
            }

            if (Mathf.Abs(aroundAngle.x - 360f - vector.x) < Mathf.Abs(aroundAngle.x - vector.x))
            {
                aroundAngle.x -= 360f;
            }
            else if (Mathf.Abs(aroundAngle.x + 360f - vector.x) < Mathf.Abs(aroundAngle.x - vector.x))
            {
                aroundAngle.x += 360f;
            }


            StateManager.Instance.TargetCameraAfterAnimation = new CameraView(targetPosition, distance, aroundAngle);

            GameMain.Instance.MainCamera.StartAnimationTo(false, Vector3.zero, 0f, Vector2.zero, targetPosition, distance, aroundAngle, animationTime, easeType, true, 0);
        }
    }
}
