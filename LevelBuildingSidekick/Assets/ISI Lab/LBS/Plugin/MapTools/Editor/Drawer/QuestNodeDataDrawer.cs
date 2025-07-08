using System.Diagnostics;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Settings;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
        
        private const float BaseSize = 100f;
        private const float BorderThickness = 0.05f;
        private const float BackgroundOpacity = 0.33f;
        
        
        private static readonly Color GrammarWrong = LBSSettings.Instance.view.warningColor;
        private static readonly Color Correct = LBSSettings.Instance.view.successColor;
        
        /// <summary>
        /// Draws the information that corresponds to the quest node behavior selected node.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="teselationSize"></param>
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not QuestNodeBehaviour behaviour) return;
            if (behaviour.OwnerLayer is not { } layer) return;

            layer.OnChange += () =>
            {
                view.ClearLayerComponentView(layer, behaviour);
            };
            
            if (!Equals(LBSMainWindow.Instance._selectedLayer, layer)) return;
            if (behaviour.SelectedQuestNode?.NodeData is not { } nodeData) return;
            
            // Selected Node Trigger View 
            var statusColor = behaviour.SelectedQuestNode.GrammarCheck ? Correct : GrammarWrong;
            nodeData.Resize();
            
            //TODO: Use the new drawing system... maybe?
            
           // var nt = behaviour.RetrieveNewTiles();
           // if (nt == null || !nt.Any()) return;
           view.ClearLayerView(behaviour.OwnerLayer, true);
           
            // Trigger Position
            var triggerBase = new TriggerElement(nodeData,nodeData.Area, statusColor);
            
            // Stores using the behavior as key
            view.AddElement(behaviour.OwnerLayer, behaviour, triggerBase);
            
            
            #region BundleGraph View
            
            switch (nodeData)
            {
                case DataKill dataKill:
                    if(!dataKill.bundlesToKill.Any()) break;
                    foreach (var bundle in dataKill.bundlesToKill)
                    {
                        if (bundle is null || !bundle.Valid()) continue;
                        
                        var visual = new TriggerElement(nodeData, bundle.Area, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataStealth dataStealth:
                    if(dataStealth.bundlesObservers == null || !dataStealth.bundlesObservers.Any()) break;
                    foreach (var bundle in dataStealth.bundlesObservers)
                    {
                        if (bundle is null || !bundle.Valid()) continue;
                        
                        var visual = new TriggerElement(nodeData, bundle.Area, nodeData.Color);
                            
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataTake dataTake:
                    if (dataTake.bundleToTake.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataTake.bundleToTake.Area, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataRead dataRead:
                    if (dataRead.bundleToRead.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataRead.bundleToRead.Area, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataGive dataGive:
                    if (dataGive.bundleGiveTo.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataGive.bundleGiveTo.Area, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataReport dataReport:
                    if (dataReport.bundleReportTo.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataReport.bundleReportTo.Area ,nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataSpy dataSpy:
                    if (dataSpy.bundleToSpy.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataSpy.bundleToSpy.Area ,nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataListen dataListen:
                    if (dataListen.bundleListenTo.Valid())
                    {
                        var visual = new TriggerElement(nodeData, dataListen.bundleListenTo.Area, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
            }
            
            #endregion
            
  
        }

        public override void ShowVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not QuestNodeBehaviour behaviour) return;
            
            foreach (object tile in behaviour.Keys)
            {
                foreach (var graphElement in view.GetElements(behaviour.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.style.display = DisplayStyle.Flex;
                }
            }
        }
        public override void HideVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not QuestNodeBehaviour behaviour) return;
            
            foreach (object tile in behaviour.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(behaviour.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
        }
            
        private sealed class TriggerElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;

            public TriggerElement(BaseQuestNodeData data ,Rect area, Color color)
            {
                _data = data;
                
                VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("TriggerElementArea");
                visualTree.CloneTree(this);
                
                
                // Scale the tile size by the base size (assuming BaseSize is a constant somewhere)
                //Vector2 pixelSize = tileSize * BaseSize; todo
                var position = LBSMainWindow.Instance._selectedLayer.FixedToPosition(new Vector2Int((int)area.x,(int)area.y), true);
                Rect drawArea = new Rect(position, new Vector2(area.width*BaseSize, area.height*BaseSize));
                
                // Properly position the element using SetPosition to avoid layout offset
                SetPosition(drawArea);

                VisualElement triggerElementGizmo = this.Q<VisualElement>("TriggerElementSelector");
                VisualElement targetIcon = this.Q<VisualElement>("TargetIcon");
                // Set border thickness
                // style.borderBottomWidth = BorderThickness;
                // style.borderTopWidth = BorderThickness;
                // style.borderLeftWidth = BorderThickness;
                // style.borderRightWidth = BorderThickness;

                // Color configuration
                Color backgroundColor = color;
                targetIcon.style.unityBackgroundImageTintColor = backgroundColor;
                
                backgroundColor.a = 0.2f;
                triggerElementGizmo.style.backgroundColor = backgroundColor;
                triggerElementGizmo.style.unityBackgroundImageTintColor = backgroundColor;
                
                triggerElementGizmo.style.borderBottomColor = color;
                triggerElementGizmo.style.borderTopColor = color;
                triggerElementGizmo.style.borderRightColor = color;
                triggerElementGizmo.style.borderLeftColor = color;    
                
                // backgroundColor.a = BackgroundOpacity;
                //
                // Texture2D bgTexture = Macros.LBSAssetMacro.LoadAssetByGuid<Texture2D>("da949edd419a40347b7e7d382b96a7e2");
                // VectorImage handle = Macros.LBSAssetMacro.LoadAssetByGuid<VectorImage>("Nice");
                // style.backgroundImage = new StyleBackground(handle);
                //
                // style.backgroundImage = bgTexture;
                // style.unityBackgroundImageTintColor = backgroundColor;
                

                triggerElementGizmo.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                triggerElementGizmo.RegisterCallback<MouseUpEvent>(OnMouseUp);
                
                VisualElement handle = this.Q<VisualElement>("ScaleHandle");
                handle.RegisterCallback<MouseMoveEvent>(OnHandleRectMove);
                
                
            }


            private void OnMouseMove(MouseMoveEvent e)
            {
                // Only move when left mouse button is pressed
                if (e.pressedButtons == 0 || e.button != 0) return;
                if (!MainView.Instance.HasManipulator<SelectManipulator>()) return;

                var currentRect = GetPosition();
                var delta = e.mouseDelta / MainView.Instance.viewTransform.scale;
                var newPos = new Rect(currentRect.x + delta.x, currentRect.y + delta.y, currentRect.width, currentRect.height);
                SetPosition(newPos);

                // Update node data position (convert to grid if needed)
                var gridPos = LBSMainWindow._gridPosition;
                float newX = gridPos.x;
                float newY = gridPos.y;
                
                Rect newRect = new Rect(newX, newY, _data.Area.width, _data.Area.height);
                _data!.Area = newRect;
            
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                if (_data.Owner?.Graph?.OwnerLayer is null) return;
                var qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(_data.Owner.Graph.OwnerLayer);
                
                SetPosition(_data!.Area);
                qnb.DataChanged(_data.Owner);
            }


            private void OnHandleRectMove(MouseMoveEvent _evt)
            {
                if (_evt.pressedButtons == 0 || _evt.button != 0) return;
                
                Vector2 delta = _evt.mouseDelta / MainView.Instance.viewTransform.scale;
                delta = delta.normalized;
                Debug.Log(delta);
                
                Rect currentRect = GetPosition();
                Rect newRect = new Rect(
                    currentRect.x,
                    currentRect.y,
                    currentRect.width + delta.x,
                    currentRect.height + delta.y
                    );
                SetPosition(newRect);
                
                
                
            }
            
        }
      
    }        
    
}
