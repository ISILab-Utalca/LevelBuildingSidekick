using ISILab.Commons.Utility.Editor;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.Editor.Windows
{
    [UxmlElement]
    public partial class LBSWaitTaskOverlay: VisualElement
    {
        [UxmlAttribute]
        public float Step
        {
            get => step;
            private set => step = value;
        }

        [UxmlAttribute]
        public int Delay
        {
            get => delay;
            private set => delay = value;
        }


        VisualElement taskIcon;
        private float step = 1.0f;
        private int delay = 0;
        private float rotationDegres = 0f;

        public LBSWaitTaskOverlay() : base()
        {
            VisualTreeAsset vta = DirectoryTools.GetAssetByName<VisualTreeAsset>(nameof(LBSWaitTaskOverlay));
            vta?.CloneTree(this);
            
            taskIcon = this.Q<VisualElement>("CenterIcon");


            this.schedule.Execute(() =>
            {
                taskIcon.style.rotate =  new Rotate(rotationDegres);
            }).Every(32);

        }
        
        
        
    }
    
}

