using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomTreeView : TreeView
    {
        // Store the current TreeView data as strings
        private List<TreeViewItemData<string>> treeData = new();

        public LBSCustomTreeView()
        {
            ClearClassList();
            AddToClassList("lbs-tree-view");

            // Default load on attach
            RegisterCallback<AttachToPanelEvent>(_ => BuildTreeFromStringData(treeData));
        }

        #region Default Tree

        private void LoadDefaultTree()
        {
            // Clear previous data
            treeData.Clear();

            // Create some default demo data
            for (int i = 0; i < 10; i++)
            {
                int itemIndex = i * 10 + i;

                var subItems = new List<TreeViewItemData<string>>(10);
                for (int j = 0; j < 10; j++)
                    subItems.Add(new TreeViewItemData<string>(itemIndex + j + 1, $"Data {i + 1}-{j + 1}"));

                var item = new TreeViewItemData<string>(itemIndex, $"Data {i + 1}", subItems);
                treeData.Add(item);
            }

            BuildTreeFromStringData(treeData);
        }

        #endregion

        #region Build Tree from string data

        public void BuildTreeFromStringData(List<TreeViewItemData<string>> items)
        {
            if (items == null || items.Count == 0) return;

            treeData = items;

            makeItem = () =>
            {
                var l = new Label();
                l.AddToClassList("lbs-tree-view");
                return l;
            };
            bindItem = (e, i) =>
            {
                e.AddToClassList("lbs-tree-view-item");

                var itemData = GetItemDataForIndex<string>(i);
                var id = GetIdForIndex(i);
                ((Label)e).text = $"ID {id} - {itemData}";
            };

            SetRootItems(treeData);
            selectionType = SelectionType.Multiple;
            Rebuild();

            // Optional callbacks
            itemsChosen += selectedItems =>
            {
                Debug.Log("Items chosen: " + string.Join(", ", selectedItems));
            };

            selectedIndicesChanged += selectedIndices =>
            {
                var log = "IDs selected: ";
                foreach (var index in selectedIndices)
                    log += $"{GetIdForIndex(index)}, ";
                Debug.Log(log.TrimEnd(',', ' '));
            };
        }

        #endregion

        #region Build Tree from generic T

        public void BuildTreeFromGenericData<T>(List<TreeViewItemData<T>> items)
        {
            if (items == null || items.Count == 0) return;

            // Convert any T to string recursively
            List<TreeViewItemData<string>> stringItems = new();
            foreach (var item in items)
                stringItems.Add(ConvertToStringTree(item));

            BuildTreeFromStringData(stringItems);
        }

        private TreeViewItemData<string> ConvertToStringTree<T>(TreeViewItemData<T> item)
        {
            var children = new List<TreeViewItemData<string>>();
            if (item.children != null)
            {
                foreach (var child in item.children)
                    children.Add(ConvertToStringTree(child));
            }

            string strData = item.data != null ? item.data.ToString() : $"Datatype {item.data?.GetType()}, is missing the ToString() implementation. Can't display on tree.";
            return new TreeViewItemData<string>(item.id, strData, children);
        }

        #endregion
    }
}
