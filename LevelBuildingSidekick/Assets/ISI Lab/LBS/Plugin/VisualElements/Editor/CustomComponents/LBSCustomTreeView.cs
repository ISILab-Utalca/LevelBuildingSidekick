using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using LBS.Bundles;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using TreeView = UnityEngine.UIElements.TreeView;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomTreeView : TreeView, INotifyValueChanged<List<LBSTreeData>>
    {
        
        internal static readonly BindingId valueProperty = (BindingId) nameof (value);
        
        // Store the current TreeView data as strings
        private List<TreeViewItemData<string>> treeDataString = new();
        private List<TreeViewItemData<LBSTreeData>> treeDataRaw = new();
        private List<LBSTreeData> treeData;

        [UxmlObjectReference("TreeViewItems")]
        public List<LBSTreeData> value
        {
            get => treeData;
            set
            {
                if (treeData == value) return;
                SetValueWithoutNotify(value);
                
                if (value.Count > 0 )
                {
                    treeDataRaw.Clear();
                    treeDataString.Clear();
                    
                    foreach (LBSTreeData treeData in value)
                    {
                        
                        treeDataString.Add(treeData.AsTreeDataString());
                        //treeDataRaw.Add(treeData.AsTreeDataRaw());
                    }
                    BuildTreeFromStringData(treeDataString);
                    
                    //BuildTreeFromGenericData(treeDataRaw);
                }
                this.NotifyPropertyChanged(valueProperty);
            }
        }


        public LBSCustomTreeView()
        {
            ClearClassList();
            AddToClassList("lbs-tree-view");

            // // Default load on attach
            // RegisterCallback<AttachToPanelEvent>(_ => BuildTreeFromStringData(treeData));
        }

        #region Default Tree

        private void LoadDefaultTree()
        {
            // Clear previous data
            treeDataString.Clear();

            // Create some default demo data
            for (int i = 0; i < 10; i++)
            {
                int itemIndex = i * 10 + i;
            
                var subItems = new List<TreeViewItemData<string>>(10);
                for (int j = 0; j < 10; j++)
                    subItems.Add(new TreeViewItemData<string>(itemIndex + j + 1, $"Data {i + 1}-{j + 1}"));
            
                var item = new TreeViewItemData<string>(itemIndex, $"Data {i + 1}", subItems);
                treeDataString.Add(item);
            }

            BuildTreeFromStringData(treeDataString);
        }

        #endregion

        #region Build Tree from string data

        public void BuildTreeFromStringData(List<TreeViewItemData<string>> items)
        {
            if (items == null || items.Count == 0) return;

            treeDataString = items;

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
                ((Label)e).text = $"ID::{id}::{itemData}";
            };

            SetRootItems(treeDataString);
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

        public void SetValueWithoutNotify(List<LBSTreeData> newValue)
        {
            treeData = newValue;
        }
    }
    
    
    [UxmlObject]
    public partial class LBSTreeData
    {
        //Automatic create an object global unique id
        private Guid uniqueID;
        public LBSTreeData()
        {
            uniqueID = Guid.NewGuid();
            //Debug.Log(uniqueID.ToString());
            //Debug.Log(uniqueID.GetHashCode());
            ItemName = "Sample Tree Item";
            Id = uniqueID.GetHashCode();
        }
        
        public LBSTreeData(int _id): this(){
            Id = _id;
            ItemName = this.ToString();
        }
        
        [UxmlAttribute("id")] public int Id;
        
        [UxmlAttribute ("item-name")] public string ItemName;
        
        [UxmlObjectReference]
        public List<LBSTreeData> Children;

        public virtual TreeViewItemData<string> AsTreeDataString()
        {
            List<TreeViewItemData<string>> childData  = new();
            if (Children == null)
            {
                return new TreeViewItemData<string>(Id, ItemName);
            } else {
                
                foreach (LBSTreeData child in Children)
                {
                    childData.Add(child.AsTreeDataString()); // Recursive hell
                }
                return new TreeViewItemData<string>(Id, ItemName, childData);
            }
        }

        public virtual TreeViewItemData<LBSTreeData> AsTreeDataRaw()
        {
            List<TreeViewItemData<LBSTreeData>> childData = new();
            if (Children == null)
            {
                return new TreeViewItemData<LBSTreeData>(Id, this);
            } else {
                
                foreach (LBSTreeData child in Children)
                {
                    childData.Add(new TreeViewItemData<LBSTreeData>(child.Id, child)); // Recursive hell
                }
                return new TreeViewItemData<LBSTreeData>(Id, this, childData);
            }
        }
        
        public override string ToString()
        {
            return $"{ItemName}::{Id}";
        }

        public LBSTreeData FromString(string _rawValue)
        {
            LBSTreeData data = new LBSTreeData();
            string[] rawSplit = _rawValue.Split("::");
            data.Id = int.Parse(rawSplit[0]);
            data.ItemName = rawSplit[1];
            
            return data;
        }
    }
}
