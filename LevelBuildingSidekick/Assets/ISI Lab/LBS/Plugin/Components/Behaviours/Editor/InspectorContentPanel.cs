using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Editor;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Visual Element Class that displays the different behaviors within the behavior's panel
    /// </summary>
    public class InspectorContentPanel : VisualElement
    {
        // View
        private readonly VisualElement _content;

        public InspectorContentPanel(LBSCustomEditor content, string name, VectorImage icon, Color color)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BehaviourContent");
            visualTree.CloneTree(this);
            
            var foldout1 = this.Q<Foldout>();
            foldout1.RegisterCallback<ChangeEvent<bool>>(FoldoutPressed);
            
            var icon1 = this.Q<VisualElement>("Icon");
            icon1.style.backgroundImage = new StyleBackground(icon);
            icon1.style.unityBackgroundImageTintColor = new StyleColor(color);
            
            var label1 = this.Q<Label>();
            label1.text = name;
            
            var menu1 = this.Q<Button>();
            var cmm = new ContextualMenuManipulator(content.ContextMenu);
            cmm.target = menu1;
            
            _content = this.Q<VisualElement>("Content");
            _content.Add(content);
        }

        private void FoldoutPressed(ChangeEvent<bool> evt)
        {
            _content.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}