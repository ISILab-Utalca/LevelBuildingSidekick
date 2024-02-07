using LBS.Assisstants;
using UnityEngine.UIElements;
using ISILab.Commons.Utility.Editor; // FIX: See if this class should have access to editor stuff

public class AIAgentPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<AIAgentPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region VIEW FIELDS
    private Label label;
    private Button details;
    private Button execute;
    #endregion

    #region FIELDS
    private LBSAssistant agent;
    #endregion

    #region CONSTRUCTORS
    public AIAgentPanel()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIAgentPanel");
        visualTree.CloneTree(this);

        label = this.Q<Label>(name: "Name");
        details = this.Q<Button>(name: "Details");
        execute = this.Q<Button>(name: "Execute");
    }
    #endregion
}
