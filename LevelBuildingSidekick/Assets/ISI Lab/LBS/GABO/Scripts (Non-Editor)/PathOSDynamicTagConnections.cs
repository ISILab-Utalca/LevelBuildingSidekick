using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ISILab.LBS.Components.PathOSObstacleConnections;

namespace ISILab.LBS.Components
{
    [System.Serializable]
    // Conexiones entre un PathOSTile de tipo DynamicTagTrigger y los
    // respectivos DynamicTagObject que transforma (asignandoles un nuevo PathOSTag)
    public class PathOSDynamicTagConnections
    {
        #region FIELDS
        [SerializeField, SerializeReference]
        private PathOSTile tagTriggerTile;
        [SerializeField]
        private List<(PathOSTile, PathOSTag)> dynamicTagObjects = new();
        [SerializeField]
        public bool IsNull = false;
        #endregion

        #region CONSTRUCTORS
        public PathOSDynamicTagConnections(PathOSTile trigger, List<(PathOSTile, PathOSTag)> dynamicTagObjs)
        {
            // Dynamic Tag Object check
            foreach (var dynamicTagObject in dynamicTagObjs)
            {
                if (!dynamicTagObject.Item1.IsDynamicTagObject)
                {
                    Debug.LogWarning("PathOSDynamicTagConnections: Lista tiene tile no-DynamicTagObject!");
                }
                return;
            }

            tagTriggerTile = trigger;
            this.dynamicTagObjects = dynamicTagObjs;
        }
        // "NULL" Constructor: Represents a "null" connections object. Prevents serialization problems
        // with Unity by replacing traditional "null" value.
        public PathOSDynamicTagConnections(bool isNull)
        {
            if (!isNull) { Debug.LogError("Null constructor should always set 'isNull' as true!"); }
            this.IsNull = true;
        }
        #endregion

        #region PROPERTIES
        public PathOSTile TagTriggerTile { get => tagTriggerTile; set => tagTriggerTile = value; }
        public List<(PathOSTile, PathOSTag)> DynamicTagObjects { get => dynamicTagObjects; set => dynamicTagObjects = value; }
        #endregion

        #region METHODS
        public (PathOSTile, PathOSTag)? GetDynamicTag(int x, int y)
        {
            var o = dynamicTagObjects.Find(o => o.Item1.Position == new Vector2Int(x, y));
            if (o == (null, null)) { return null; }
            return o;
        }
        public (PathOSTile, PathOSTag)? GetDynamicTag(PathOSTile tile)
        {
            var o = dynamicTagObjects.Find(t => t.Item1 == tile);
            if (o == (null, null)) { return null; }
            return o;
        }

        public void AddDynamicTag(PathOSTile tagTile, PathOSTag tag)
        {
            // Tile tipo "tag" check
            if (!tagTile.IsDynamicTagObject)
            {
                Debug.LogWarning("PathOSDynamicTagConnections.AddDynamicTag(): Tile no es DynamicTagObject!");
                return;
            }
            // Tile repetida check
            if (dynamicTagObjects.Exists(o => tagTile.Position == o.Item1.Position))
            {
                Debug.LogWarning("PathOSDynamicTagConnections.AddDynamicTag(): Tag ya existe!");
                return;
            }

            dynamicTagObjects.Add((tagTile, tag));
        }

        public void RemoveDynamicTag(int x, int y)
        {
            var toRemove = dynamicTagObjects.Find(o => o.Item1.Position == new Vector2Int(x, y));
            if (toRemove == (null, null))
            {
                Debug.LogWarning($"PathOSDynamicTagConnections.RemoveDynamicTag():" +
                    $"No existe tile en la posicion {x}, {y} para remover!");
                return;
            }
            dynamicTagObjects.Remove(toRemove);
        }

        // Imprimir tiles asociados
        public override string ToString()
        {
            string s = "";
            s += base.ToString();
            s += $"DynamicTagTrigger: {tagTriggerTile.Tag.Label} {tagTriggerTile.Position}\n";
            s += "DynamicTagObjects asociados:\n";
            foreach (var dynamicTag in dynamicTagObjects)
            {
                PathOSTile currTile = dynamicTag.Item1;
                s += $"{currTile.Tag.Label} {currTile.Position} {dynamicTag.Item2.Label}\n";
            }
            return s;
        }
        #endregion
    }


}