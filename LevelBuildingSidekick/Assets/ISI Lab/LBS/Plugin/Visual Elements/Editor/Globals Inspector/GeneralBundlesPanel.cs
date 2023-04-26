using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralBundlesPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<GeneralBundlesPanel, VisualElement.UxmlTraits> { }
    #endregion

    private ObjectField iconField;
    private TextField textField;
    private ColorField colorField;

    private Foldout extraFoldout;
    private VisualElement extraContent;

    private ListView childList;
    private ObjectField fatherField;

    private Bundle target;

    public GeneralBundlesPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("GeneralBundlesPanel");
        visualTree.CloneTree(this);

        this.iconField = this.Q<ObjectField>("IconField");
        iconField.RegisterCallback<ChangeEvent<Object>>( t => target.ID.Icon = t.newValue as Texture2D);

        this.textField = this.Q<TextField>("NameField");
        textField.RegisterCallback<ChangeEvent<string>>(t => target.ID.Label = target.ID.name = t.newValue);

        this.colorField = this.Q<ColorField>("ColorField");
        colorField.RegisterCallback<ChangeEvent<Color>>(t => target.ID.Color = t.newValue);

        this.extraFoldout = this.Q<Foldout>("ExtraFoldout");
        extraFoldout.RegisterCallback<ChangeEvent<bool>>(t => extraContent.style.display = (t.newValue)? DisplayStyle.Flex : DisplayStyle.None);

        this.extraContent = this.Q<VisualElement>("ExtraContent");
    }

    public void SetInfo(Bundle target)
    {
        this.target = target;

        if(target.ID == null)
        {
            target.ID = CreateID();
        }

        iconField.value = target.ID.Icon;
        textField.value = target.ID.Label;
        colorField.value = target.ID.Color;
    }

    private LBSIdentifier CreateID()
    {
        var all = Utility.DirectoryTools.GetScriptables<LBSIdentifier>().ToList();
        var nSO = ScriptableObject.CreateInstance<LBSIdentifier>();

        var settings = LBSSettings.Instance;

        var name = ISILab.Commons.Commons.CheckNameFormat(all.Select(b => b.name), "new ID");

        AssetDatabase.CreateAsset(nSO, settings.tagFolderPath + "/" + name + ".asset");
        // nSO.name = name;
        // AssetDatabase.AddObjectToAsset(nSO, target);
        AssetDatabase.SaveAssets();

        return nSO;
    }
}
