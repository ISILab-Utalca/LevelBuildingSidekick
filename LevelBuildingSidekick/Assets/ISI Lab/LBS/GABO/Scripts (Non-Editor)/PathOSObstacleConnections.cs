using ISILab.LBS.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Components
{
    // Conexiones entre un PathOSTile de tipo DynamicObstacleTrigger y los
    // respectivos DynamicObstacleObject que afecta.
    [System.Serializable]
    public class PathOSObstacleConnections
    {
        #region ENUMS
        [System.Serializable]
        public enum Category
        {
            None,
            OPEN,
            CLOSE
        }
        #endregion

        #region FIELDS
        [SerializeField, SerializeReference]
        private PathOSTile obstacleTriggerTile;
        [SerializeField]
        private List<(PathOSTile, Category)> obstacles = new();
        [SerializeField]
        public bool IsNull = false;
        #endregion

        #region CONSTRUCTORS
        public PathOSObstacleConnections(PathOSTile trigger, List<(PathOSTile, Category)> obs)
        {
            // Obstacle tile check
            foreach (var obstacle in obs)
            {
                if (!obstacle.Item1.IsDynamicObstacleObject)
                {
                    Debug.LogWarning("PathOSObstacleConnection: Lista tiene tile no-obstaculo!");
                }
                return;
            }

            obstacleTriggerTile = trigger;
            this.obstacles = obs;
        }
        // "NULL" Constructor: Represents a "null" connections object. Prevents serialization problems
        // with Unity by replacing traditional "null" value.
        public PathOSObstacleConnections(bool isNull)
        {
            if (!isNull) { Debug.LogError("Null constructor should always set 'isNull' as true!"); }
            this.IsNull = true;
        }
        #endregion

        #region PROPERTIES
        public PathOSTile ObstacleTriggertile { get => obstacleTriggerTile; set => obstacleTriggerTile = value; }
        public List<(PathOSTile, Category)> Obstacles { get => obstacles; set => obstacles = value; }
        #endregion

        #region METHODS
        public (PathOSTile, Category)? GetObstacle(int x, int y)
        {
            var o = obstacles?.Find(o => o.Item1.Position == new Vector2Int(x, y));
            if (o == (null, Category.None)) { return null; }
            return o;
        }

        public (PathOSTile, Category)? GetObstacle(PathOSTile tile)
        {
            var o = obstacles?.Find(o => o.Item1 == tile);
            if (o == (null, Category.None)) { return null; }
            return o;
        }

        public void AddObstacle(PathOSTile obstacleTile, Category category)
        {
            // Tile tipo "obstaculo" check
            if (!obstacleTile.IsDynamicObstacleObject)
            {
                Debug.LogWarning("PathOSObstacleConnection.AddObstacle(): Tile no es obstaculo!");
                return;
            }
            // Tile repetida check
            if (obstacles.Exists(o => obstacleTile.Position == o.Item1.Position))
            {
                Debug.LogWarning("PathOSObstacleConnection.AddObstacle(): Obstaculo ya existe!");
                return;
            }

            obstacles.Add((obstacleTile, category));
        }

        public void RemoveObstacle(int x, int y)
        {
            var toRemove = obstacles.Find(o => o.Item1.Position == new Vector2Int(x, y));
            if (toRemove == (null, null))
            {
                Debug.LogWarning($"PathOSObstacleConnection.RemoveObstacle():" +
                    $"No existe tile en la posicion {x}, {y} para remover!");
                return;
            }
            obstacles.Remove(toRemove);
        }

        // Imprimir tiles asociados
        public override string ToString()
        {
            string s = "";
            s += base.ToString();
            s += $"DynamicObstacleTrigger: {obstacleTriggerTile.Tag.Label} {obstacleTriggerTile.Position}\n";
            s += "DynamicObstacleObjects asociados:\n";
            foreach(var obstacle in obstacles)
            {
                PathOSTile currTile = obstacle.Item1;
                s += $"{currTile.Tag.Label} {currTile.Position} {obstacle.Item2.ToString()}\n";
            }
            return s;
        }
        #endregion
    }
}
