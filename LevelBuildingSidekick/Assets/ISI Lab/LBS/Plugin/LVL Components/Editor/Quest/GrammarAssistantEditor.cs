using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using LBS;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("GrammarAssistant", typeof(GrammarAssistant))]
public class GrammarAssistantEditor : LBSCustomEditor, IToolProvider
{

    public GrammarAssistantEditor()
    {

    }

    public GrammarAssistantEditor(GrammarAssistant target) : base(target) 
    {
        CreateVisualElement();
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        


    }

    public void SetTools(ToolKit toolkit)
    {
    }

    protected override VisualElement CreateVisualElement()
    {
        return this;
    }
}
