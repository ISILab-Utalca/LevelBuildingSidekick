using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Settings;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class ExpansionHeader : VisualElement
    {
        
        public readonly SuggestionActionButton ButtonConvert;
        
        public ExpansionHeader()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ExpansionHeader");
            visualTree.CloneTree(this);

            ButtonConvert = this.Q<SuggestionActionButton>(name: "ButtonConvert");
            ButtonConvert.SetIconColor(LBSSettings.Instance.view.warningColor);
        }
        
    }
}