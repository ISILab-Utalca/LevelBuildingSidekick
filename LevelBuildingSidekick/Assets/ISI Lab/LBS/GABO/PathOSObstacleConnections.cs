using ISILab.LBS.Modules;
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
        public enum Category
        {
            OPEN,
            CLOSE
        }
        #endregion

        #region FIELDS
        private PathOSTile obstacleTriggerTile;
        private List<(PathOSTile, Category)> obstacles = new();
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
        #endregion

        #region PROPERTIES
        public PathOSTile ObstacleTriggertile { get => obstacleTriggerTile; set => obstacleTriggerTile = value; }
        public List<(PathOSTile, Category)> Obstacles { get => obstacles; set => obstacles = value; }
        #endregion

        #region METHODS
        public (PathOSTile, Category) GetObstacle(int x, int y)
        {
            return obstacles.Find(o => o.Item1.Position == new Vector2Int(x, y));
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
