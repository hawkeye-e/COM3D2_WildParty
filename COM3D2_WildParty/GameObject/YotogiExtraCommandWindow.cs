using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.CustomGameObject
{
    class YotogiExtraCommandWindow
    {
        #region Dimensions and Display Positions
        const float PANEL_WIDTH = 400f;
        const float PANEL_HEIGHT = 800f;
        const float SCROLLBAR_WIDTH = 20f;
        const float CONTENT_HOLDER_WIDTH = 380f;
        const float CONTENT_HOLDER_HEIGHT = 760f;

        const float PANEL_POSITION_X = 0f;
        const float PANEL_POSITION_Y = -780f;

        const float SCROLLBAR_THUMB_POSITION_X = 235f;
        //        const float SCROLLBAR_THUMB_POSITION_Y = -257f;
        const float SCROLLBAR_THUMB_POSITION_Y = 257f;

        static Vector3 COMMAND_WINDOW_LOCATION = new Vector3(510f, 380f, 0f);

        static Vector3 CONTENT_HOLDER_LOCATION = new Vector3(120f, 506f, 5f);
        static Vector3 SCROLL_BAR_UP_BUTTON_LOCATION = new Vector3(245f, 535f);
        static Vector3 SCROLL_BAR_DOWN_BUTTON_LOCATION = new Vector3(245f, -250f);

        const float BUTTON_DISTANCE = -35f;
        #endregion

        #region Path strings
        const string MODDED_OBJECT_NAME = "Mod_ExtraCommandPanel";
        const string MODDED_CONTENT_HOLDER_NAME = "Parent";
        
        const string ORIGINAL_CONTENT_HOLDER_PATH = "Content/CategoryUnitParent";
        const string MODDED_CONTENT_HOLDER_PATH = "Content/Parent";
        const string ARROW_PATH = "Arrow";
        const string BASE_PATH = "Base";
        const string SCROLL_BAR_PATH = "Scroll Bar";
        const string SCROLL_BAR_FOREGROUND_PATH = "Scroll Bar/Foreground";
        const string SCROLL_BAR_UP_BUTTON = "Scroll Bar/Up";
        const string SCROLL_BAR_DOWN_BUTTON = "Scroll Bar/Down";
        const string DRAG_MAT_PATH = "DragMat";
        const string CONTENT_PATH = "Content";
        #endregion

        #region Display Order Settings
        //Sorting order: decide who display on top. Depth: decide who capture the button click first
        const int CONTENT_SORTING_ORDER = 15;
        const int CONTENT_DEPTH = 60;

        const int SCOLL_BAR_SORTING_ORDER = 20;
        const int SCOLL_BAR_DEPTH = 65;
        const int SCOLL_BAR_THUMB_DEPTH = 66;
        #endregion

        internal enum Mode
        {
            Hidden,
            PositionList,
            MassPositionList,
            FormationList,
            MaidList,
            MaidAsManList,
        }

        public Transform transform;
        private GameObject MainObject;
        private bool _requireInit = true;
        private Mode _currentMode = Mode.Hidden;

        public YotogiExtraCommandWindow(GameObject toBeCloned, bool isClonedFromMasterCopy = true)
        {
            var clone = GameObject.Instantiate(toBeCloned);
            this.MainObject = clone;

            //Set it to active in order to avoid being destroy automatically, but at the same time we dont want it to appear on the screen so scale it to 0
            clone.SetActive(true);
            this.transform = clone.transform;
            
            SetVisible(false);
            
            //clone.transform.localScale = new Vector3(0f, 0f, 0f);

            //for (int i = 0; i < clone.transform.childCount; i++)
            //    clone.transform.GetChild(i).localScale = new Vector3(0f, 0f, 0f);

            //rename to avoid confusion when debug
            clone.name = MODDED_OBJECT_NAME;
            string contentHolderPath = ORIGINAL_CONTENT_HOLDER_PATH;
            if (isClonedFromMasterCopy)
                contentHolderPath = MODDED_CONTENT_HOLDER_PATH;
            //var contentParent = clone.transform.Find(ORIGINAL_CONTENT_HOLDER_PATH);
            var contentParent = clone.transform.Find(contentHolderPath);

            contentParent.name = MODDED_CONTENT_HOLDER_NAME;
            
            //we dont need that arrow
            if (!isClonedFromMasterCopy) { 
                var arrow = clone.transform.Find(ARROW_PATH);
                arrow.gameObject.SetActive(false);
                arrow.SetParent(null);
            }
            
            //config the positions a bit. we will have to do the remaining once it is attached to the yotogi scene
            var baseRect = clone.transform.Find(BASE_PATH).GetComponent<UIRect>();
            baseRect.SetRect(PANEL_POSITION_X, PANEL_POSITION_Y, PANEL_WIDTH, PANEL_HEIGHT);

            var scrollbarRect = clone.transform.Find(SCROLL_BAR_PATH).GetComponent<UIRect>();
            scrollbarRect.SetRect(PANEL_POSITION_X, PANEL_POSITION_Y, PANEL_WIDTH, PANEL_HEIGHT);

            var scrollbarForeground = clone.transform.Find(SCROLL_BAR_FOREGROUND_PATH).GetComponent<UIRect>();
            scrollbarForeground.SetRect(SCROLLBAR_THUMB_POSITION_X, -SCROLLBAR_THUMB_POSITION_Y, SCROLLBAR_WIDTH, PANEL_HEIGHT);

            var DragMatRect = clone.transform.Find(DRAG_MAT_PATH).GetComponent<UIRect>();
            DragMatRect.SetRect(PANEL_POSITION_X, PANEL_POSITION_Y, PANEL_WIDTH, PANEL_HEIGHT);

            var pnl = clone.transform.Find(CONTENT_PATH).GetComponent<UIPanel>();
            pnl.SetRect(PANEL_POSITION_X, PANEL_POSITION_Y, CONTENT_HOLDER_WIDTH, CONTENT_HOLDER_HEIGHT);

            this.transform = clone.transform;
            this._requireInit = true;
        }

        //Call this function once the window is attached to the scene.
        public void InitStructureAfterAttaching()
        {
            var contentParent = transform.Find(MODDED_CONTENT_HOLDER_PATH);
            contentParent.transform.localPosition = CONTENT_HOLDER_LOCATION;

            var content = transform.Find(CONTENT_PATH);
            content.GetComponent<UIPanel>().sortingOrder = CONTENT_SORTING_ORDER;
            content.GetComponent<UIPanel>().depth = CONTENT_DEPTH;


            var scrollBar = transform.Find(SCROLL_BAR_PATH);
            scrollBar.GetComponent<UIPanel>().sortingOrder = SCOLL_BAR_SORTING_ORDER;
            scrollBar.GetComponent<UIPanel>().depth = SCOLL_BAR_DEPTH;

            var scrollDownButton = transform.Find(SCROLL_BAR_DOWN_BUTTON);
            scrollDownButton.transform.localPosition = SCROLL_BAR_DOWN_BUTTON_LOCATION;
            scrollDownButton.GetComponent<UIWidget>().depth = SCOLL_BAR_DEPTH;

            var scrollUpButton = transform.Find(SCROLL_BAR_UP_BUTTON);
            scrollUpButton.transform.localPosition = SCROLL_BAR_UP_BUTTON_LOCATION;
            scrollUpButton.GetComponent<UIWidget>().depth = SCOLL_BAR_DEPTH;

            var scrollForeground = transform.Find(SCROLL_BAR_FOREGROUND_PATH);
            scrollForeground.GetComponent<UIWidget>().depth = SCOLL_BAR_THUMB_DEPTH;

            transform.localPosition = COMMAND_WINDOW_LOCATION;
            _requireInit = false;
        }

        public bool IsRequireInit()
        {
            return _requireInit;
        }

        public bool IsVisible()
        {
            return transform.localScale == Vector3.one;
        }

        public void SetVisible(bool isVisible)
        {
            Vector3 scaleVector = isVisible? Vector3.one : Vector3.zero;
            
            //Dont update the active flag here as it could make the game object destroyed by the engine. Instead set the scale only.
            transform.localScale = scaleVector;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).localScale = scaleVector;

            if (!isVisible)
                _currentMode = Mode.Hidden;
        }

        public void SetActive(bool isEnable)
        {
            transform.gameObject.SetActive(isEnable);
        }

        public Mode GetMode()
        {
            return _currentMode;
        }

        public void ShowContent(List<GameObject> buttons, Mode mode)
        {
            var contentParent = transform.Find(MODDED_CONTENT_HOLDER_PATH);

            //remove the existing buttons
            RemoveContent();

            //add the buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.SetParent(contentParent.transform, false);
                buttons[i].transform.localPosition = new Vector3(0f, i * BUTTON_DISTANCE, i * 0f);
            }

            _currentMode = mode;
        }

        public void RemoveContent()
        {
            var contentParent = transform.Find(MODDED_CONTENT_HOLDER_PATH);
            for (int i = contentParent.childCount -1 ; i >= 0; i--)
            {
                var btn = contentParent.GetChild(i);
                btn.gameObject.SetActive(false);
                btn.SetParent(null);
                btn.localScale = Vector3.zero;
                GameObject.Destroy(btn.gameObject);
            }
        }

        public void ResetScrollPosition()
        {
            var scrollView = transform.Find(CONTENT_PATH);
            scrollView.GetComponent<UIScrollView>().ResetPosition();
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this.MainObject);
        }
    }
}
