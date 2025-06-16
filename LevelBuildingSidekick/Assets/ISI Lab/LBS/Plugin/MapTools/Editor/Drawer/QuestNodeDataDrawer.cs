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
        
        private const float baseSize = 100f;
        private const float borderThickness = 0.05f;

        public QuestNodeBehaviourDrawer() : base() { }
        /// <summary>
        /// Draws the information that corresponds to the quest node behavior selected node.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="teselationSize"></param>
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not QuestNodeBehaviour behaviour) return;
            
            if (!Equals(LBSMainWindow.Instance._selectedLayer, behaviour.OwnerLayer)) return;
            
            var layer = behaviour.OwnerLayer;
            if (layer == null) return;
            
            var qn = behaviour.SelectedQuestNode;
            var nodeData = qn?.NodeData;
            if (nodeData == null) return;
            
            //Debug.Log("\n --node: " + nd.Owner.ID + "--");
            /*
             * TODO: Replace this within the switch and pass the visualElement corresponding
             * to the type in the switch. Perhaps use the attribute created for actions}
             * but apply on visual Elements.
             */
            // view.AddElement(behaviour.OwnerLayer, behaviour, type);
            var position = layer.FixedToPosition(nodeData.Position, true);
            
            // Trigger Position
            var triggerBase = new TriggerElement(position, baseSize*nodeData.Size, nodeData, LBSSettings.Instance.view.colorTrigger);
            view.AddElement(behaviour.OwnerLayer, behaviour, triggerBase);
            
            // Positions per data type only if its a BundleGraph!
            switch (nodeData)
            {
                case DataKill datakill:
                    if(!datakill.BundlesToKill.Any()) break;
                    foreach (var bundle in datakill.BundlesToKill)
                    {
                        if (bundle.Valid())
                        {
                            position = layer.FixedToPosition(bundle.Position, true);
                            var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                            view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                        }
                    }
                    break;
                
                case DataStealth dataStealth:
                    if(!dataStealth.BundlesObservers.Any()) break;
                    foreach (var bundle in dataStealth.BundlesObservers)
                    {
                        if (bundle.Valid())
                        {
                            position = layer.FixedToPosition(bundle.Position, true);
                            var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                            view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                        }
                    }
                    break;
                
                case DataTake dataTake:
                    if (dataTake.BundleToTake.Valid())
                    {
                        position = layer.FixedToPosition(dataTake.BundleToTake.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataRead dataRead:
                    if (dataRead.BundleToRead.Valid())
                    {
                        position = layer.FixedToPosition(dataRead.BundleToRead.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataGive dataGive:
                {
                    if (dataGive.BundleGive.Valid())
                    {
                        position = layer.FixedToPosition(dataGive.BundleGive.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    
                    if (dataGive.BundleGiveTo.Valid())
                    {
                        position = layer.FixedToPosition(dataGive.BundleGiveTo.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                 
                }
                    break;
                
                case DataReport dataReport:
                    if (dataReport.BundleReportTo.Valid())
                    {
                        position = layer.FixedToPosition(dataReport.BundleReportTo.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataSpy dataSpy:
                    if (dataSpy.BundleToSpy.Valid())
                    {
                        position = layer.FixedToPosition(dataSpy.BundleToSpy.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataListen dataListen:
                    if (dataListen.BundleListenTo.Valid())
                    {
                        position = layer.FixedToPosition(dataListen.BundleListenTo.Position, true);
                        var visual = new TriggerElement(position, baseSize*nodeData.Size, nodeData, nodeData.Color);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
            }
            
        }
        

        public class TriggerElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;

            public TriggerElement(Vector2 position, float length, BaseQuestNodeData data, Color color)
            {
                _data = data;

                // Properly position the element using SetPosition to avoid layout offset
                SetPosition(new Rect(position.x, position.y, length, length));

                // Calculate radius and border thickness
                float radius = length / 2;
                float thickness = borderThickness;
                
                //style.borderBottomLeftRadius = radius;
                //style.borderBottomRightRadius = radius;
                //style.borderTopLeftRadius = radius;
                //style.borderTopRightRadius = radius;

                // Set border thickness
                style.borderBottomWidth = thickness;
                style.borderTopWidth = thickness;
                style.borderLeftWidth = thickness;
                style.borderRightWidth = thickness;

                // Color configuration
                Color backgroundColor = color;
                backgroundColor.a = 0.33f;

                style.backgroundColor = backgroundColor;
                style.borderBottomColor = color;
                style.borderTopColor = color;
                style.borderRightColor = color;
                style.borderLeftColor = color;

                // Optional: if using a child VisualElement for background visuals
                // var visual = new VisualElement();
                // visual.style.flexGrow = 1;
                // visual.style.backgroundColor = backgroundColor;
                // Add(visual);

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