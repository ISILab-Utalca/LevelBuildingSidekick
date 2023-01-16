using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName ="ISILab/LBS/Layer Template")]
public class LayerTemplates : ScriptableObject
{
    public LBSLayer layer;

    public LBSLayer Construct()
    {
        var layer = new LBSLayer();
        return layer;
    }
}

public abstract class LayoutConstructor 
{
    public abstract LBSLayer Construct();
}

public class InteriorConstructor : LayoutConstructor
{
    public override LBSLayer Construct()
    {
        var layer = new LBSLayer();
        
        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Interior";
        layer.Name = "Interior layer";
        layer.iconPath = "Icons/interior-design";

        return layer;
    }
}

public class ExteriorConstructor : LayoutConstructor
{
    public override LBSLayer Construct()
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Exterior";
        layer.Name = "Exterior layer";
        layer.iconPath = "Icons/pine-tree";

        return layer;
    }
}

public class PopulationConstructor : LayoutConstructor
{
    public override LBSLayer Construct()
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Exterior";
        layer.Name = "Population layer"; 
        layer.iconPath = "Icons/ghost";

        return layer;
    }
}