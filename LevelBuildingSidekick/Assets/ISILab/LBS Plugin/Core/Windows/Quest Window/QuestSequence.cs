using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestSequence : VisualElement
{
    public new class UxmlFactory : UxmlFactory<QuestSequence, VisualElement.UxmlTraits> { }

    public QuestSequence()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestSequenceUXML");
        visualTree.CloneTree(this);

    }
}
