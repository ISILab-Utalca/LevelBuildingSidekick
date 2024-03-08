using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Settings
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ISILab/LBS/Internal/LBS Settings", fileName = "LBS Settings")]
    public class LBSSettings : ScriptableObject
    {
        #region SINGLETON
        private static LBSSettings instance;

        /// <summary>
        /// Singleton instance of "LBSSettings".<br/>
        /// <b>[WARNING]:</b> The use of the <b>SET</b> method for this property is at your own risk.
        /// </summary>
        public static LBSSettings Instance
        {
            get
            {
                // si es igual a null lo busco en carpeta
                if (instance == null)
                    instance = Resources.Load<LBSSettings>("LBS Settings");

                // si sigue siendo null lo creo
                if (instance == null)
                    instance = ScriptableObject.CreateInstance<LBSSettings>();

                return instance;
            }

            set // que esto sea publico es un problema por que cualquier cosa lo puede acceder, pero
            {
                instance = value;
            }
        }
        #endregion

        public Paths paths = new Paths();
        public General general = new General();
        public Interface view = new Interface();
        public Test test = new Test();

        [System.Serializable]
        public class Test
        {
            public string TestFolderPath = "";
        }

        [System.Serializable]
        public class General
        {
            public float zoomMax = 10;
            public float zoomMin = 0.1f;

            [SerializeField]
            Vector2 tileSize = new Vector2(50, 50);

            public Vector2 TileSize
            {
                get => tileSize;
                set => tileSize = value;
            }

            public Action<float, float> OnChangeZoomValue;
            public Action<Vector2> OnChangeTileSize;
        }

        [System.Serializable]
        public class Paths
        {
            // Controller Paths
            public string settingsPath = "Assets/ISI Lab/LBS/Plugin/Internal/Settings/Resources/LBS Settings.asset";
            public string storagePath = "Assets/ISI Lab/LBS/Plugin/Internal/Editor/LBS Storage.asset";
            public string pressetsPath = "Assets/ISI Lab/LBS/Presets/LBS Presets.asset";
            public string backUpPath = "Assets/ISI Lab/LBS/Plugin/Internal/Resources/LBSBackUp.asset";

            // Folders data storages
            public string bundleFolderPath = "Assets/ISI Lab/LBS/Data/Bundles";
            public string tagFolderPath = "Assets/ISI Lab/LBS/Data/Tags";

            // Folders extra storages
            public string iconPath = "Assets/ISI Lab/LBS/Plugin/Internal/Icons";

            // Folders presets
            public string layerPressetFolderPath = "Assets/ISI Lab/LBS/Presets/Layers";
            public string assistantPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Assistants";
            public string Generator3DPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Generators3D";
            public string bundlesPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Bundles";

        }

        [System.Serializable]
        public class Interface
        {
            public Color toolkitSelected = new Color(254f / 255f, 118f / 255f, 105f / 255f);

            public Color behavioursColor = new Color(135f / 255f, 215f / 255f, 246f / 255f);
            public Color assitantsColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);

            public Color bundlesColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);
            public Color tagsColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);
        }
    }



}

