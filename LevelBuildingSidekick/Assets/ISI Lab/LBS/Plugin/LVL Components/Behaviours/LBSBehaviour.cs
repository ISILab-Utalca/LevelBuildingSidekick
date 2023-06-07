using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class LBSBehaviour
{
    [JsonIgnore]
    protected LBSLayer Owner; // no se si esto tenggo que inicializarlo con un Init() o en el constructor (?)

    public LBSBehaviour()
    {

    }

    public List<Type> GetRequieredModules ()
    {
        var toR = new List<Type>();
        Type tipo = this.GetType();

        object[] atts = tipo.GetCustomAttributes(true);

        foreach (var att in atts)
        {
            if (att is RequieredBehavioursAttribute)
            {
                toR.AddRange((att as RequieredBehavioursAttribute).types);
            }
        }
        return toR;
    }
}

[System.Serializable]
[RequieredBehaviours(typeof(Exterior))]
public class SimpleConectedBehaviour : LBSBehaviour
{

}

[System.Serializable]
[RequieredBehaviours(typeof(LBSRoomGraph),typeof(LBSSchema))]
public class SchemaBehaviour : LBSBehaviour
{
    public string TEST2 = "Adios mundo :C";
}


public class RequieredBehavioursAttribute : Attribute
{
    public List<Type> types;

    public RequieredBehavioursAttribute(params Type[] types)
    {
        this.types = types.GetDerivedTypes(typeof(LBSBehaviour)).ToList();
    }
}

public static class TypeExtensions
{
    public static IEnumerable<Type> GetDerivedTypes(this IEnumerable<Type> types, Type baseType)
    {
        return types.Where(t => baseType.IsAssignableFrom(t) && t != baseType);
    }
}
