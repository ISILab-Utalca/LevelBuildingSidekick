using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Manipulators
{
    public class AddClosedObstacle : AddObstacle
    {
        public override void AddObstacleAction()
        {
            triggerTile.AddObstacle(obstacleTile, Components.PathOSObstacleConnections.Category.CLOSE);
        }
    }

}