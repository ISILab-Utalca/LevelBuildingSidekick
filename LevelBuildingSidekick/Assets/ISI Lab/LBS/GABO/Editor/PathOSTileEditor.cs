using ISILab.LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISILab.LBS.Modules;
using ISILab.LBS.Editor;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSTile", typeof(PathOSTile))]
    public class PathOSTileEditor : LBSCustomEditor
    {
        public override void SetInfo(object target)
        {
            this.target = target as PathOSTile;
            (this.target as PathOSTile).OnAddObstacle += Repaint;
            CreateVisualElement();
        }

        //GABO TODO: TERMINAR
        protected override VisualElement CreateVisualElement()
        {
            var panel = new PathOSTriggerInfoPanel();
            // Agregar obstaculos (si no hay, no hace nada)
            panel.AddObstacles(target as PathOSTile);
            Add(panel);

            return this;
        }

        public override void Repaint()
        {
            Clear();
            CreateVisualElement();
        }
    }

}