using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ConfMore : Button
    {
        public new class UxmlFactory : UxmlFactory<ConfMore, VisualElement.UxmlTraits> { }

        public ConfMore()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConfigMoreUXML");
            visualTree.CloneTree(this);
        }
    }
}
