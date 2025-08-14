using ISILab.LBS.Generators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS.Settings
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
        public Generator3D generator = new Generator3D();

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

            public String baseLayerName = "New Layer";

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
            public string backUpPath = "Assets/ISI Lab/LBS/Plugin/Internal/Resources/BackUp/LBSBackUp.asset";

            // Folders data storages
            public string bundleFolderPath = "Assets/ISI Lab/LBS/Data/Bundles";
            public string tagFolderPath = "Assets/ISI Lab/LBS/Data/Tags";
            public string meshFolderPath = "Assets/ISI Lab/LBS/Data/Meshes";

            // Folders extra storages
            public string iconPath = "Assets/ISI Lab/LBS/Plugin/Internal/Icons";

            // Folders presets
            public string layerPressetFolderPath = "Assets/ISI Lab/LBS/Presets/Layers";
            public string assistantPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Assistants";
            public string assistantOptimizerPresetPath = "Assets/ISI Lab/LBS/Presets/Optimizers";
            public string assistantEvaluatorPresetPath = "Assets/ISI Lab/LBS/Presets/Evaluators";
            public string Generator3DPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Generators3D";
            public string bundlesPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Bundles";

            //public string savedMapsPresetPath = "Assets/ISI Lab/LBS/Presets/SavedMaps";

        }

        [System.Serializable]
        public class Interface
        {
            public enum InterfaceTheme {Dark, Light, Alt}
            
            [SerializeField]
            public InterfaceTheme LBSTheme = InterfaceTheme.Dark;
            
            public Color toolkitNormal = new Color(0.28f, 0.28f, 0.28f);
            public Color newToolkitSelected = new Color(0.21f, 0.48f, 0.96f);
            
            public Color behavioursColor = new Color(0.53f, 0.84f, 0.96f);
            public Color assistantColor = new Color(0.76f, 0.96f, 0.44f);
            
            public Color bundlesColor = new Color(0.5f, 0.69f, 0.98f);
            public Color tagsColor = new Color(0.93f, 0.81f, 0.42f);

            public Color warningColor = new Color(1f, 0.76f, 0.03f);
            public Color errorColor = new Color(0.81f, 0.13f, 0.31f);
            public Color okColor = Color.white;
            public Color successColor = new Color(0f, 1f, 0.68f);
            public Color calloutColor = new Color(151/255f, 71/255f, 1.0f);
            
            #region Quest Node Colors
            public Color colorTrigger = new Color(0f, 1f, 0.68f);
            public Color colorKill = new Color(0.93f, 0.33f, 0.42f);
            public Color colorStealth = new Color(0.45f, 0.07f, 0.7f);
            public Color colorTake = new Color(0.16f, 0.7f, 0.57f);
            public Color colorRead = new Color(0.51f, 1f, 0.9f);
        
            public Color colorGive = new Color(1f, 0.72f, 0.92f);
            [FormerlySerializedAs("colorGiveTo")] public Color colorExchange = new Color(1f, 0.45f, 0.91f);
        
            public Color colorReport = new Color(0.41f, 0.63f, 1f);
            public Color colorSpy = new Color(0.78f, 0.79f, 1f);
            public Color colorListen = new Color(0.52f, 1f, 0.05f);
            #endregion
        }
        
    }



}

