using System.Linq;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
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
            view.ClearLayerContainer(behaviour.OwnerLayer, true);
            
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
           // temp fix just clreaing the whole layer, as this is called BEFORE the other drawer this one clears it once

           
            // Trigger Position
            var triggerBase = new TriggerElementArea(nodeData,nodeData.Area);
            
            // Stores using the behavior as key
            view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, triggerBase);
            
            
            #region BundleGraph View
            
            switch (nodeData)
            {
                case DataKill dataKill:
                    if(!dataKill.bundlesToKill.Any()) break;
                    foreach (var bundle in dataKill.bundlesToKill)
                    {
                        if (bundle is null || !bundle.Valid()) continue;
                        
                        var visual = new TriggerElementArea(nodeData, bundle.Area, false);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataStealth dataStealth:
                    if(dataStealth.bundlesObservers == null || !dataStealth.bundlesObservers.Any()) break;
                    foreach (var bundle in dataStealth.bundlesObservers)
                    {
                        if (bundle is null || !bundle.Valid()) continue;
                        
                        var visual = new TriggerElementArea(nodeData, bundle.Area, false);
                            
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataTake dataTake:
                    if (dataTake.bundleToTake.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataTake.bundleToTake.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataRead dataRead:
                    if (dataRead.bundleToRead.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataRead.bundleToRead.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataGive dataGive:
                    if (dataGive.bundleGiveTo.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataGive.bundleGiveTo.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataReport dataReport:
                    if (dataReport.bundleReportTo.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataReport.bundleReportTo.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataSpy dataSpy:
                    if (dataSpy.bundleToSpy.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataSpy.bundleToSpy.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
                    }
                    break;
                
                case DataListen dataListen:
                    if (dataListen.bundleListenTo.Valid())
                    {
                        var visual = new TriggerElementArea(nodeData, dataListen.bundleListenTo.Area);
                        view.AddElementToLayerContainer(behaviour.OwnerLayer, behaviour, visual);
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
                foreach (var graphElement in view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile).Where(graphElement => graphElement != null))
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

                var elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
        }

      
    }        
    
}
