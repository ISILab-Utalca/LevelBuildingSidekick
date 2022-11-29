using LBS;
using LBS.Manipulators;
using LBS.Representation;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;




[Overlay(typeof(WFCWindow), ID, "WFC Tools", "-", defaultDisplay = true)]
public class WFCTools : Overlay
{
    private const string ID = "WFCToolsOverlay";

    private Button allMode;

    private Button addTileButton;
    private Button eraseTileButton;
    private Button addConectionButton;
    private Button eraseConectionButton;

    public static List<ColorTag> colorTags = new List<ColorTag>(); // (?) parche?
    public struct ColorTag // (!) continua el parche de arriba pero con mas sentido
    {
        public string tag;
        public Color color;

        public ColorTag(string tag, Color color)
        {
            this.tag = tag;
            this.color = color;
        }
    }
    public ColorTag current; // (!) mas parche
    public int index = -1; // (!) mas parche

    public static Color GetColor(string tag) // (!!!) mas parche
    {
        for (int i = 0; i < colorTags.Count; i++)
        {
            if (colorTags[i].tag.Equals(tag))
                return colorTags[i].color;
        }
        return Color.black;
    }

    public override VisualElement CreatePanelContent()
    {
        colorTags = new List<ColorTag>();
        var parche = DirectoryTools.GetScriptable<LBSTags>("WFC Tags");
        parche.Alls.ForEach(t => {
            colorTags.Add(new ColorTag(t, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))));
            }); // (!) mas parche como el de arriba 

        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("WFCTools");
        visualTree.CloneTree(root);

        var window = EditorWindow.GetWindow<WFCWindow>();
        var controller = window.GetController<WFCController>();
        var data = controller.GetData() as LBSTileMapData;

        // All mode 
        allMode = root.Q<Button>("AllMode");  // (!) esto terminara desapareciendo
        allMode.clicked += () => window.MainView.SetBasicManipulators();

        // Add tile
        addTileButton = root.Q<Button>("AddTileButton");
        addTileButton.clicked += () =>
        {
            var manipulator = new AddTileManipulator(data, controller);
            manipulator.OnEndAction += () => { window.RefreshView();};
            window.MainView.SetManipulator(manipulator);
        };

        // Erase tile
        eraseTileButton = root.Q<Button>("EraseTileButton");
        eraseTileButton.clicked += () =>
        {
           var manipulator2 = new DeleteTileManipulator(data, controller);
           manipulator2.OnEndAction += () => { window.RefreshView(); };
           window.MainView.SetManipulator(manipulator2);
        };

        // Add conection
        addConectionButton = root.Q<Button>("AddConectionButton");
        addConectionButton.clicked += () =>
        {
            index = (index + 1) % colorTags.Count;
            current = colorTags[index];
            addConectionButton.text = "addConectionButton: " + current.tag;

            var manipulator = new AddConnectionManipulator(current,data, controller);
            manipulator.OnEndAction += () => { window.RefreshView(); };
            window.MainView.SetManipulator(manipulator);
        };

        // Erase connection
        eraseConectionButton = root.Q<Button>("EraseConectionButton");
        eraseConectionButton.clicked += () =>
        {
            var manipulator = new RemoveConnectionManipulator(data, controller);
            manipulator.OnEndAction += () => { window.RefreshView(); };
            window.MainView.SetManipulator(manipulator);
        };

        return root;
    }
}
