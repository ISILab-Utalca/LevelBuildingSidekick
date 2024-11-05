using ISILab.LBS.Behaviours;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public abstract class AddObstacle : LBSManipulator
    {
        #region FIELDS
        PathOSBehaviour behaviour;
        protected PathOSTile triggerTile;
        protected PathOSTile obstacleTile;
        #endregion

        #region CONSTRUCTORS
        public AddObstacle() : base()
        {
            feedback = new ConnectedLine();
            feedback.fixToTeselation = true;
        }
        #endregion

        #region METHODS
        public override void Init(LBSLayer layer, object provider)
        {
            behaviour = provider as PathOSBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
        {
            var pos = behaviour.Owner.ToFixedPosition(startPosition);
            triggerTile = behaviour.GetTile(pos.x, pos.y);
        }

        // GABO TODO: TERMINAR
        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var pos = behaviour.Owner.ToFixedPosition(endPosition);
            obstacleTile = behaviour.GetTile(pos.x, pos.y);

            // GABO TODO: Arreglar, tal y como en AddPathOSTile!!!!
            // Inicio logica UNDO
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Add Obstacle");

            // Chequeos
            // Chequeo nulo
            if (obstacleTile == null || triggerTile == null) { Debug.LogWarning("Alguno de los tiles dados es nulo!"); return; }
            // Chequeo de propiedad correcta
            if (!obstacleTile.IsDynamicObstacleObject || !triggerTile.IsDynamicObstacleTrigger) { Debug.LogWarning("Alguno de los tiles no es del tipo correcto!"); return; }
            // Chequeo de repeticion
            var currObstacles = triggerTile.GetObstacles();
            foreach (var obstacle in currObstacles)
            {
                if (obstacle.Item1 == obstacleTile) { Debug.LogWarning($"Tile-obstaculo X:{obstacle.Item1.X} Y:{obstacle.Item1.Y} Label:{obstacle.Item1.Tag.Label} repetido!"); return; }
            }

            // Accion a realizar al agregar el obstaculo
            AddObstacleAction();

            // Final logica UNDO
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }

        // Accion a realizar al agregar el obstaculo (debe implementarse)
        public abstract void AddObstacleAction();

        #endregion
    }
}
