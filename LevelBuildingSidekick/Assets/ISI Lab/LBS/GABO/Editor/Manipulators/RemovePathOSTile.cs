using ISILab.LBS;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RemovePathOSTile : LBSManipulator
{
    #region FIELDS
    PathOSBehaviour behaviour;
    #endregion

    #region CONSTRUCTORS
    public RemovePathOSTile() : base()
    {
        feedback = new AreaFeedback();
        feedback.fixToTeselation = true;
    }
    #endregion

    #region METHODS
    public override void Init(LBSLayer layer, object owner)
    {
        behaviour = owner as PathOSBehaviour;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        //GABO TODO: ARREGLAR LOGICA UNDO
        // Inicio logica UNDO
        var x = LBSController.CurrentLevel;
        EditorGUI.BeginChangeCheck();
        Undo.RegisterCompleteObjectUndo(x, "Remove PathOS Tile");

        // Remover PathOSTiles mediante PathOSBehaviour
        var corners = behaviour.Owner.ToFixedPosition(StartPosition, EndPosition);
        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                behaviour.RemoveTile(i, j);
            }
        }

        // Final logica UNDO
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(x);
        }
    }
    #endregion
}
