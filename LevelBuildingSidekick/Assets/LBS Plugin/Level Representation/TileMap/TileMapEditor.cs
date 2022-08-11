using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TileMapEditor : EditorWindow
{
    private MapDataScriptable map;

    private TileGridView tileGridView;
    private Label notSelectedLabel;
    private InspectorView inspectorView;

    [MenuItem("LBS/TileMap Window...")]
    public static void OpenWindow()
    {
        TileMapEditor wnd = GetWindow<TileMapEditor>();
        wnd.titleContent = new GUIContent("TileMapEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TEST/UXML/TileMapEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TEST/USS/TileMapEditor.uss");
        root.styleSheets.Add(styleSheet);

        tileGridView = root.Q<TileGridView>();
        notSelectedLabel = root.Q<Label>("NotSelected");
        inspectorView = root.Q<InspectorView>();
    }

    private void OnSelectionChange()
    {
        var map = Selection.activeObject as MapDataScriptable;
        if(map != null)
        {
            notSelectedLabel.visible = false;
            tileGridView.visible = true;
            tileGridView.SetView(map);
        }
        else
        {
            notSelectedLabel.visible = true;
            tileGridView.visible = false;
        }
    }
}