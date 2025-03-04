using ISILab.LBS.Characteristics;
using LBS.Bundles;
using UnityEngine;

namespace ISI_Lab.LBS.Plugin.MapTools.Generators3D
{
    /// <summary>
    /// Component added to any game objects created by the LBS tool.
    /// Main function is to contain a reference to the object's bundle, for different functionalities.
    /// </summary>
    public class LBSGenerated : MonoBehaviour
    {
        [SerializeField]
        private Bundle bundleRef;

        public Bundle BundleRef
        {
            get => bundleRef;
            set => bundleRef = value;
        }
    
        public bool HasLBSTag(string tag)
        {
            var characteristics = bundleRef.Characteristics;
            foreach (var lbsChar in characteristics)
            {
                if (lbsChar is not LBSTagsCharacteristic lbstagChar) continue;
                if (lbstagChar.Value == null) continue;
                if (lbstagChar.Value.label != tag) continue;
                return true;
            }

            return false;
        }
    }
}
