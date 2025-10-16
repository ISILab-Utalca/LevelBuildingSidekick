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


        [UxmlAttribute]
        public bool ShowRect
        {
            get => showRect;
            set
            {
                showRect = value;
                this.style.display = showRect ? DisplayStyle.Flex : DisplayStyle.None;
                
                if (showRect) {rotationSchedule?.Resume();}
                else { rotationSchedule?.Pause();}
            }
        }

        VisualElement taskIcon;
        private IVisualElementScheduledItem rotationSchedule;
        
        private float step = 1.0f;
        private int delay = 0;
        private float rotationDegres = 0f;
        private bool showRect = true;

        public LBSWaitTaskOverlay() : base()
        {
            VisualTreeAsset vta = DirectoryTools.GetAssetByName<VisualTreeAsset>(nameof(LBSWaitTaskOverlay));
            vta?.CloneTree(this);
            this.AddToClassList("TaskOverlay");
            
            taskIcon = this.Q<VisualElement>("CenterIcon");
            
            rotationSchedule = this.schedule.Execute(() =>
            {
                rotationDegres += step;
                rotationDegres %= 360; 
                taskIcon.style.rotate =  new Rotate(rotationDegres);
                taskIcon.MarkDirtyRepaint();
            }).Every(64L).StartingIn(0L);
            rotationSchedule.Resume();
            
        }
    }
    
}

