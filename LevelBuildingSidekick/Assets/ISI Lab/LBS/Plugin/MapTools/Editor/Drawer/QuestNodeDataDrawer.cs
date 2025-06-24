using System.Linq;
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
            if (!Equals(LBSMainWindow.Instance._selectedLayer, layer)) return;
            if (behaviour.SelectedQuestNode?.NodeData is not { } nodeData) return;
            
            // Selected Node Trigger View
            var position = layer.FixedToPosition(nodeData.Position, true);
            var statusColor = behaviour.SelectedQuestNode.GrammarCheck ? Correct : GrammarWrong;
            
            // Checks if a new selection must be drawn
            var nt = behaviour.RetrieveNewTiles();
            if (nt == null || !nt.Any()) return;
            
            //Debug.Log("\n --node: " + nd.Owner.ID + "--");
            /*
             * TODO: Replace this within the switch and pass the visualElement corresponding
             * to the type in the switch. Perhaps use the attribute created for actions}
             * but apply on visual Elements.
             */
            // view.AddElement(behaviour.OwnerLayer, behaviour, type);
            
            // Trigger Position
            var triggerBase = new TriggerElement(
                    position, 
                    new Vector2(nodeData.Size,nodeData.Size), 
                    nodeData, 
                    statusColor);
            
            // Stores using the behavior as key
            view.AddElement(behaviour.OwnerLayer, behaviour, triggerBase);
            
            
            #region BundleGraph View
            
            switch (nodeData)
            {
                case DataKill dataKill:
                    if(!dataKill.bundlesToKill.Any()) break;
                    foreach (var bundle in dataKill.bundlesToKill)
                    {
                        if (!bundle.Valid()) continue;
                        
                        position = layer.FixedToPosition(bundle.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            bundle.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataStealth dataStealth:
                    if(dataStealth.bundlesObservers == null || !dataStealth.bundlesObservers.Any()) break;
                    foreach (var bundle in dataStealth.bundlesObservers)
                    {
                        if (!bundle.Valid()) continue;
                        
                        position = layer.FixedToPosition(bundle.Position, true);
                        var visual = new TriggerElement(
                            position,
                            bundle.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                            
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataTake dataTake:
                    if (dataTake.bundleToTake.Valid())
                    {
                        position = layer.FixedToPosition(dataTake.bundleToTake.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataTake.bundleToTake.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataRead dataRead:
                    if (dataRead.bundleToRead.Valid())
                    {
                        position = layer.FixedToPosition(dataRead.bundleToRead.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataRead.bundleToRead.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataGive dataGive:
                {
                    if (dataGive.bundleGiveTo.Valid())
                    {
                        position = layer.FixedToPosition(dataGive.bundleGiveTo.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataGive.bundleGiveTo.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                 
                }
                    break;
                
                case DataReport dataReport:
                    if (dataReport.bundleReportTo.Valid())
                    {
                        position = layer.FixedToPosition(dataReport.bundleReportTo.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataReport.bundleReportTo.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataSpy dataSpy:
                    if (dataSpy.bundleToSpy.Valid())
                    {
                        position = layer.FixedToPosition(dataSpy.bundleToSpy.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataSpy.bundleToSpy.GetElementSize(), 
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataListen dataListen:
                    if (dataListen.bundleListenTo.Valid())
                    {
                        position = layer.FixedToPosition(dataListen.bundleListenTo.Position, true);
                        var visual = new TriggerElement(
                            position, 
                            dataListen.bundleListenTo.GetElementSize(),
                            nodeData, 
                            nodeData.Color);
                        
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
            }
            
            #endregion
        }


        private sealed class TriggerElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;

            public TriggerElement(Vector2 position, Vector2 tileSize, BaseQuestNodeData data, Color color)
            {
                _data = data;

                // Scale the tile size by the base size (assuming BaseSize is a constant somewhere)
                Vector2 pixelSize = tileSize * BaseSize;

                // Properly position the element using SetPosition to avoid layout offset
                SetPosition(new Rect(position.x, position.y, pixelSize.x, pixelSize.y));

                // Set border thickness
                style.borderBottomWidth = BorderThickness;
                style.borderTopWidth = BorderThickness;
                style.borderLeftWidth = BorderThickness;
                style.borderRightWidth = BorderThickness;

                // Color configuration
                Color backgroundColor = color;
                backgroundColor.a = BackgroundOpacity;

                style.backgroundColor = backgroundColor;
                style.borderBottomColor = color;
                style.borderTopColor = color;
                style.borderRightColor = color;
                style.borderLeftColor = color;

                RegisterCallback<MouseMoveEvent>(OnMouseMove);
                RegisterCallback<MouseUpEvent>(OnMouseUp);
            }


            private void OnMouseMove(MouseMoveEvent e)
            {
                // Only move when left mouse button is pressed
                if (e.pressedButtons == 0 || e.button != 0) return;
                if (!MainView.Instance.HasManipulator<Select>()) return;

                var currentRect = GetPosition();
                var delta = e.mouseDelta / MainView.Instance.viewTransform.scale;
                var newPos = new Rect(currentRect.x + delta.x, currentRect.y + delta.y, currentRect.width, currentRect.height);
                SetPosition(newPos);

                // Update node data position (convert to grid if needed)
                var gridPos = LBSMainWindow._gridPosition;
                _data!.Position = gridPos;
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                if (_data.Owner?.Graph?.OwnerLayer is null) return;
                var qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(_data.Owner.Graph.OwnerLayer);
                qnb.DataChanged(_data.Owner);
        }


        }
    }
}