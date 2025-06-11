using System.Linq;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
        private static readonly Color colorTrigger = new Color(0.93f, 0.81f, 0.42f, 1f);
        private static readonly Color colorKill = new Color(0.93f, 0.33f, 0.42f);
        private static readonly Color colorObserver = new Color(0.45f, 0.07f, 0.7f);
        private static readonly Color colorTake = new Color(0.16f, 0.7f, 0.57f);
        private static readonly Color colorRead = new Color(0.51f, 1f, 0.9f);
        
        private static readonly Color colorGive = new Color(1f, 0.72f, 0.92f);
        private static readonly Color colorGiveTo = new Color(1f, 0.45f, 0.91f);
        
        private static readonly Color colorReport = new Color(0.41f, 0.63f, 1f);
        private static readonly Color colorSpy = new Color(0.78f, 0.79f, 1f);
        private static readonly Color colorListen = new Color(0.52f, 1f, 0.05f);
        
        // Circle drawn to display an element on the graph
        private const float BaseSize = 100f;
        // Multiplied by the CircleSize (1 == filled circle)
        private const float borderThickness = 0.025f;

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
            var position = layer.FixedToPosition(nodeData.position, true);
            
            // Trigger Position
            var triggerBase = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorTrigger);
            view.AddElement(behaviour.OwnerLayer, behaviour, triggerBase);
            
            // Positions per data type only if its a BundleGraph!
            switch (nodeData)
            {
                case DataKill datakill:
                    if(!datakill.bundlesToKill.Any()) break;
                    foreach (var bundle in datakill.bundlesToKill)
                    {
                        if (bundle.Valid())
                        {
                            position = layer.FixedToPosition(bundle.position, true);
                            var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorKill);
                            view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                        }
                    }
                    break;
                
                case DataStealth dataStealth:
                    if(!dataStealth.bundlesObservers.Any()) break;
                    foreach (var bundle in dataStealth.bundlesObservers)
                    {
                        if (bundle.Valid())
                        {
                            position = layer.FixedToPosition(bundle.position, true);
                            var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorObserver);
                            view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                        }
                    }
                    break;
                
                case DataTake dataTake:
                    if (dataTake.bundleToTake.Valid())
                    {
                        position = layer.FixedToPosition(dataTake.bundleToTake.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorTake);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataRead dataRead:
                    if (dataRead.bundleToRead.Valid())
                    {
                        position = layer.FixedToPosition(dataRead.bundleToRead.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorRead);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataGive dataGive:
                {
                    if (dataGive.bundleGive.Valid())
                    {
                        position = layer.FixedToPosition(dataGive.bundleGive.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorGive);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    
                    if (dataGive.bundleGiveTo.Valid())
                    {
                        position = layer.FixedToPosition(dataGive.bundleGiveTo.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorGiveTo);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                 
                }
                    break;
                
                case DataReport dataReport:
                    if (dataReport.bundleReportTo.Valid())
                    {
                        position = layer.FixedToPosition(dataReport.bundleReportTo.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorReport);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataSpy dataSpy:
                    if (dataSpy.bundleToSpy.Valid())
                    {
                        position = layer.FixedToPosition(dataSpy.bundleToSpy.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorSpy);
                        view.AddElement(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataListen dataListen:
                    if (dataListen.bundleListenTo.Valid())
                    {
                        position = layer.FixedToPosition(dataListen.bundleListenTo.position, true);
                        var visual = new TriggerElement(position, BaseSize*nodeData.size, nodeData, colorListen);
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
                _data!.position = gridPos;
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                var qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(_data.Owner.Graph.OwnerLayer);
                qnb.DataChanged(_data.Owner);
        }


        }
    }
}