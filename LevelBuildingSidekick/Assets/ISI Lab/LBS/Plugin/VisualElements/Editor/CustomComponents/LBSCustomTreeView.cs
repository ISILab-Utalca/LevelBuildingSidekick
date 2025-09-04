using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
        
    [UxmlElement]
    public partial class LBSCustomTreeView : TreeView
    {
        private List<TreeViewItemData<string>> treeData = new();

        [UxmlAttribute] 
        public ScriptableObject field;

        public LBSCustomTreeView() : base()
        {
            RegisterCallbackOnce<MouseDownEvent>(evt => RefreshTree());
           // field.RegisterCallback<MouseDownEvent>(evt => RefreshTree());
        }

        private void RefreshTree()
        {

            var rootItems = new List<TreeViewItemData<string>>(10);
            for (var i = 0; i < 10; i++)
            {
                var itemIndex = i * 10 + i;

                var treeViewSubItemsData = new List<TreeViewItemData<string>>(10);
                for (var j = 0; j < 10; j++)
                    treeViewSubItemsData.Add(new TreeViewItemData<string>(itemIndex + j + 1, $"Data {i+1}-{j+1}"));

                var treeViewItemData = new TreeViewItemData<string>(itemIndex, $"Data {i+1}", treeViewSubItemsData);
                rootItems.Add(treeViewItemData);
            };
            

            itemsChosen += (selectedItems) =>
            {
                Debug.Log("Items chosen: " + string.Join(", ", selectedItems));
            };


            selectedIndicesChanged += (selectedIndices) =>
            {
                var log = selectedIndices.Aggregate("IDs selected: ", (current, index) => current + $"{GetIdForIndex(index)}, ");
                Debug.Log(log.TrimEnd(',', ' '));
            };



            makeItem = () => new Label();
            bindItem = (e, i) =>
            {
                var itemData = GetItemDataForIndex<string>(i);
                ((Label)e).text = itemData;
            };

            SetRootItems(rootItems);
            selectionType = SelectionType.Multiple;
            Rebuild();
        }


    }
}