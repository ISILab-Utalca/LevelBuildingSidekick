using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.AI.Assistants.Editor
{
    [LBSCustomEditor("Testing Assistant", typeof(TestingAssistant))]
    public class TestingAssistantEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS

        //private PathOSWindow pathOSOriginalWindow;

        private TestingAssistant assistant;

        #endregion

        #region PROPERTIES

        public PathOSWindow PathOSOriginalWindow { get => assistant.PathOSOriginalWindow; set => assistant.PathOSOriginalWindow = value; }

        #endregion

        #region CONSTRUCTORS

        public TestingAssistantEditor(object target) : base(target) 
        {
            assistant = target as TestingAssistant;
            CreateVisualElement();
        }

        #endregion

        public override void SetInfo(object paramTarget)
        {
            assistant = paramTarget as TestingAssistant;
        }

        protected override VisualElement CreateVisualElement()
        {
            Clear();

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("TestingAssistantEditor");
            visualTree.CloneTree(this);

            var parent = this.Q<VisualElement>();

            // Add the original PathOSWindow (IMGUI-based), create new if there's no instance.
            PathOSWindow[] oldWindows = Resources.FindObjectsOfTypeAll<PathOSWindow>();
            if (PathOSOriginalWindow == null)
            {
                if (oldWindows.Length == 0)
                {
                    PathOSOriginalWindow = ScriptableObject.CreateInstance<PathOSWindow>();
                }
                else
                {
                    PathOSOriginalWindow = oldWindows[0];
                }
            }
            IMGUIContainer container = new IMGUIContainer(PathOSOriginalWindow.OnGUI);
            parent.Add(container);
            //container.RegisterCallback<DetachFromPanelEvent>(evt => OnDetach());
            //container.StretchToParentSize();
            return this;
        }

        public void SetTools(ToolKit toolkit)
        {
            
        }
    }
}