using ISI_Lab.Commons.Utility;
using ISILab.LBS.Internal;
using PathOS;
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

        [System.Serializable]
        public struct SimulationEntityData
        {
            //public EntityType entityType;
            public Texture2D image;
            public Color color;

            public SimulationEntityData(/*EntityType type, */Texture2D img, Color col)
            {
                //entityType = type;
                image = img;
                color = col;
            }
        }

        public SimulationEntityData agentData;

        public LBSDictionary<EntityType, SimulationEntityData> entityDataPool = new LBSDictionary<EntityType, SimulationEntityData>();
         
        //public LBSDictionary<EntityType, Texture2D> entitySpritePool = new LBSDictionary<EntityType, Texture2D>();
        
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


        public Texture2D GetEntityImage(EntityType entity) => entityDataPool[entity].image;
        public Color GetEntityColor(EntityType entity) => entityDataPool[entity].color;

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
