using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("GrammarAssistant", typeof(GrammarAssistant))]
    public class GrammarAssistantEditor : LBSCustomEditor, IToolProvider
    {
        private GrammarAssistant m_GrammarAssistant;
        
        public GrammarAssistantEditor()
        {

        }

        public GrammarAssistantEditor(GrammarAssistant target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public sealed override void SetInfo(object target)
        {
            this.target = target as GrammarAssistant;
            m_GrammarAssistant = LBSLayerHelper.GetObjectFromLayerChild<GrammarAssistant>(target);
        }

        public void SetTools(ToolKit toolkit)
        {
            
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
  
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);
            
            return this;
        }
    }
}