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

        // Paths
        public string settingsPath = "Assets/ISI Lab/LBS/Plugin/Internal/Settings/Resources/LBS Settings.asset";
        public string storagePath = "Assets/ISI Lab/LBS/Plugin/Internal/Editor/LBS Storage.asset";

        // Folders storages
        public string bundleFolderPath = "Assets/ISI Lab/LBS/Data/Bundles";
        public string tagFolderPath = "Assets/ISI Lab/LBS/Data/Tags";

        // Folder presets
        public string layerPressetFolderPath = "Assets/ISI Lab/LBS/Presets/Layers";
        public string assistantPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Assistants";
        public string Generator3DPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Generators3D";
        public string bundlesPresetFolderPath = "Assets/ISI Lab/LBS/Presets/Bundles";

        public Modules modules = new Modules();
        public Layers layers = new Layers();
        public Generators3D generator3D = new Generators3D();
        public Assisstants assisstent = new Assisstants();

        [System.Serializable]
        public class Modules
        {
            //Teselation
            public Vector2 tileSize = new Vector2(100,100);
        }

        [System.Serializable]
        public class Layers
        {

        }

        [System.Serializable]
        public class Generators3D
        {

        }

        [System.Serializable]
        public class Assisstants
        {

        }
    }



}
