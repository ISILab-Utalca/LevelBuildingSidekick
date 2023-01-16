using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName ="ISILab/LBS/Layer Template")]
public class LayerTemplate : ScriptableObject
{
    [ContextMenuItem("Set as Interior", "InteriorConstruct")]
    [ContextMenuItem("Set as Exterior", "ExteriorConstruct")]
    [ContextMenuItem("Set as Population", "PopulationConstruct")]
    //[ContextMenuItem("Set as Quest", "InteriorConstruct")]
    public LBSLayer layer;
    
    private void InteriorConstruct()
    {
        layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Interior";
        layer.Name = "Interior layer";
        layer.iconPath = "Icons/interior-design";
    }

    private void ExteriorConstruct()
    {
        layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Exterior";
        layer.Name = "Exterior layer";
        layer.iconPath = "Icons/pine-tree";
    }

    private void PopulationConstruct()
    {
        layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Population";
        layer.Name = "Population layer";
        layer.iconPath = "Icons/ghost";
    }
}
