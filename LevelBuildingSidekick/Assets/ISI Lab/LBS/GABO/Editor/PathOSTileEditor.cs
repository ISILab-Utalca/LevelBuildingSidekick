using ISILab.LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISILab.LBS.Modules;
using ISILab.LBS.Editor;
using UnityEngine.UIElements;
using ISILab.LBS.VisualElements.Editor;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSTile", typeof(PathOSTile))]
    public class PathOSTileEditor : LBSCustomEditor
    {
        public override void SetInfo(object target)
        {
            this.target = target;// as PathOSTile; //<------- Es necesario castearlo desde object....?

            // Eventos que activan "Repaint" (*NOTA*: [-=] antes de [+=] evita repetir hooks)
            var castedTarget = target as PathOSTile;
            // - Al producirse cambios en el mapa
            castedTarget.Owner.OnAddTile -= (PathOSModule m, PathOSTile t) => Repaint();
            castedTarget.Owner.OnAddTile += (PathOSModule m, PathOSTile t) => Repaint();
            castedTarget.Owner.OnApplyEventTile -= (PathOSModule m, PathOSTile t) => Repaint();
            castedTarget.Owner.OnApplyEventTile += (PathOSModule m, PathOSTile t) => Repaint();
            castedTarget.Owner.OnRemoveTile -= (PathOSModule m, PathOSTile t) => Repaint();
            castedTarget.Owner.OnRemoveTile += (PathOSModule m, PathOSTile t) => Repaint();
            // - Al agregar un obstaculo al tile
            castedTarget.OnAddObstacle -= Repaint;
            castedTarget.OnAddObstacle += Repaint;
            // - Al remover un obstaculo al tile
            castedTarget.OnRemoveObstacle -= Repaint;
            castedTarget.OnRemoveObstacle += Repaint;
            // - Al agregar un tag dinamico al tile
            castedTarget.OnAddDynamicTag -= Repaint;
            castedTarget.OnAddDynamicTag += Repaint;
            // - Al remover un tag dinamico del tile
            castedTarget.OnRemoveDynamicTag -= Repaint;
            castedTarget.OnRemoveDynamicTag += Repaint;
            // - Conversion y reversion: ObstacleTrigger
            castedTarget.OnConvertingToObstacleTrigger -= Repaint;
            castedTarget.OnConvertingToObstacleTrigger += Repaint;
            castedTarget.OnRevertingFromObstacleTrigger -= Repaint;
            castedTarget.OnRevertingFromObstacleTrigger += Repaint;
            // - Conversion y reversion: ObstacleObject
            castedTarget.OnConvertingToObstacleObject -= Repaint;
            castedTarget.OnConvertingToObstacleObject += Repaint;
            castedTarget.OnRevertingFromObstacleObject -= Repaint;
            castedTarget.OnRevertingFromObstacleObject += Repaint;
            // - Conversion y reversion: DynamicTagTrigger
            castedTarget.OnConvertingToDynamicTagTrigger -= Repaint;
            castedTarget.OnConvertingToDynamicTagTrigger += Repaint;
            castedTarget.OnRevertingFromDynamicTagTrigger -= Repaint;
            castedTarget.OnRevertingFromDynamicTagTrigger += Repaint;
            // Conversion y reversion: DynamicTagObject
            castedTarget.OnConvertingToDynamicTagObject -= Repaint;
            castedTarget.OnConvertingToDynamicTagObject += Repaint;
            castedTarget.OnRevertingFromDynamicTagObject -= Repaint;
            castedTarget.OnRevertingFromDynamicTagObject += Repaint;

            CreateVisualElement();
        }

        //GABO TODO: TERMINAR
        protected override VisualElement CreateVisualElement()
        {
            var castedTarget = target as PathOSTile;
            var panel = new PathOSTriggerInfoPanel();

            // Actualizar panel con informacion del tile
            panel.Refresh(castedTarget);
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