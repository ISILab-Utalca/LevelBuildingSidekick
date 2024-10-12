using ISILab.LBS.Behaviours;
using ISILab.LBS;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Characteristics;
using LBS.VisualElements;
using ISILab.LBS.Internal;
using LBS.Bundles;
using System.Linq;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSBehaviour", typeof(PathOSBehaviour))]
    public class PathOSBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        // Palletes
        private SimplePallete bundlePallete;

        #endregion 
        public override void SetInfo(object target)
        {
            throw new System.NotImplementedException();
        }

        protected override VisualElement CreateVisualElement()
        {
            bundlePallete = new SimplePallete();
            Add(bundlePallete);
            bundlePallete.SetName("PathOS+");

            SetBundlePallete();

            return this;
        }

        // GABO TODO: Terminarrrrrr
        private void SetBundlePallete()
        {
            bundlePallete.name = "Bundles";
            Texture2D icon = Resources.Load<Texture2D>("Icons/TinyIconPathOSModule16x16");
            bundlePallete.SetIcon(icon, Color.white);

            // Get proper bundles
            List<Bundle> allBundles = LBSAssetsStorage.Instance.Get<Bundle>();
            List<Bundle> pathOSBundles = (List<Bundle>)allBundles.Where(b => b.GetCharacteristics<LBSTagsCharacteristic>().Count > 0);

        }

        
        //GABO TODO
        public void SetTools(ToolKit toolkit)
        {
            throw new System.NotImplementedException();
        }
    }
}
