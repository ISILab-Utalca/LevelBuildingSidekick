using ISILab.LBS.Internal;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [CreateAssetMenu(fileName = "PathOSStorage", menuName = "ISILab/LBS/PathOS/PathOSStorage")]
    public class PathOSStorage : ScriptableObject
    {
        #region FIELDS
        public PathOSAgent agentPrefab;
        public PathOSManager managerPrefab;
        public PathOSWorldCamera worldCameraPrefab;
        public ScreenshotManager screenshotCameraPrefab;
        #endregion

        //#region SINGLETON
        [System.NonSerialized]
        private static PathOSStorage instance;

        public static PathOSStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PathOSStorage>("PathOSStorage");
                }
                return instance;
            }
        }
        //#endregion

        #region METHODS
        private void Awake()
        {
            //instance = this;
            if (instance == null)
            {
                instance = Resources.Load<PathOSStorage>("PathOSStorage");
            }
            else
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            //instance = this;
            if (instance == null)
            {
                instance = Resources.Load<PathOSStorage>("PathOSStorage");
            }
            else
            {
                instance = this;
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        #endregion

    }
}
