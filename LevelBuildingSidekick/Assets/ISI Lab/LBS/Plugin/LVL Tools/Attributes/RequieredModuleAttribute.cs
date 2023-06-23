using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequieredModuleAttribute : Attribute
{
    public List<Type> types;

    public RequieredModuleAttribute(params Type[] types) // cmabiar estos a solo un tipo LBSRequiredAtt (??)
    {
        this.types = types.GetDerivedTypes(typeof(LBSModule)).ToList();
    }
}

public class RequieredBehaviourAttribute : Attribute // cmabiar estos a solo un tipo LBSRequiredAtt (??)
{
    public List<Type> types;

    public RequieredBehaviourAttribute(params Type[] type)
    {
        this.types = type.GetDerivedTypes(typeof(LBSBehaviour)).ToList();
    }
}

public class RequieredAssistantAttribute : Attribute
{
    public List<Type> types;

    public RequieredAssistantAttribute(params Type[] type) // cmabiar estos a solo un tipo LBSRequiredAtt (??)
    {
        this.types = type.GetDerivedTypes(typeof(LBSAssistantAI)).ToList();
    }
}