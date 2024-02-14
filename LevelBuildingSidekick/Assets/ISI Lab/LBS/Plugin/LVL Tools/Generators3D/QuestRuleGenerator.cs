using LBS.Components;
using LBS.Generator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRuleGenerator : LBSGeneratorRule
{

    public QuestRuleGenerator()
    {

    }

    public override List<Message> CheckViability(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        throw new System.NotImplementedException();
    }

    public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
    {
        var pivot = new GameObject(layer.ID);
        var observer = pivot.AddComponent<QuestObserver>();

        CloneRefs.Start();
        var quest = layer.GetModule<QuestGraph>().Clone() as QuestGraph;
        CloneRefs.End();

        var triggers = new List<QuestStep>();

        foreach (var node in quest.QuestNodes)
        {
            if (node == quest.Root)
                continue;

            var go = new GameObject(node.ID);

            go.transform.position = node.Target.Rect.position;
            go.transform.parent = observer.transform; 

            var trigger = go.AddComponent<QuestTrigger>();
            trigger.Init(new Vector3(node.Target.Rect.width, 1, node.Target.Rect.height));

            go.SetActive(false);

            triggers.Add(new QuestStep(node, trigger));
        }

        observer.Init(quest, triggers);

        return pivot;
    }
}
