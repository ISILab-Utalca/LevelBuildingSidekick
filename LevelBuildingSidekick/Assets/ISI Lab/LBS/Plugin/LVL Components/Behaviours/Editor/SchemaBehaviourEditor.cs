using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("SchemaBehaviour", typeof(SchemaBehaviour))]
public class SchemaBehaviourEditor : LBSCustomEditor
{
    SchemaBehaviour schema;
    public List<LBSManipulator> manipulators;

    public SchemaBehaviourEditor()
    {
        manipulators.Add(new AddConnection());
        manipulators.Add(new RemoveTile());

    }

    public SchemaBehaviourEditor(object target) : base(target)
    {
        schema = target as SchemaBehaviour;

        manipulators.Add(new AddSchemaTile());
        manipulators.Add(new RemoveSchemaTile());
        manipulators.Add(new AddAreaConnection());
        manipulators.Add(new RemoveAreaConnection());
        manipulators.Add(new SetTileConnection());

        Init();

        Add(CreateVisualElement());
    }

    public void Init()
    {
        foreach(var m in manipulators)
        {
            //m.Init( ,schema.Owner, schema);
        }
    }

    public override void SetInfo(object target)
    {

    }

    protected override VisualElement CreateVisualElement()
    {
        var contentContainer = new VisualElement();
        foreach (var m in manipulators)
        {
            var type = m.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>().Where(t => t.Item2.Any(v => v.type == type));
            if (ves.Count() == 0)
            {
                throw new Exception("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
            }

            var ve = Activator.CreateInstance(ves.First().Item1, new object[] { m });
            if (!(ve is VisualElement))
            {
                throw new Exception("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
            }

            contentContainer.Add(ve as VisualElement);
        }
        return contentContainer;
    }
}
