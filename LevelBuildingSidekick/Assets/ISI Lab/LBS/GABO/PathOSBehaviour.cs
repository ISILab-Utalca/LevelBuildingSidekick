using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    public class PathOSBehaviour : LBSBehaviour
    {
        // TODO: Implementar los iconos correspondientes a cada enum como paleta en el Behaviour
        #region ENUMS
        public enum EntityType
        {
            ET_NONE = 0,
            ET_GOAL_OPTIONAL = 100,
            ET_GOAL_MANDATORY = 110,
            ET_GOAL_COMPLETION = 120,
            ET_RESOURCE_ACHIEVEMENT = 150,
            ET_RESOURCE_PRESERVATION_LOW = 160,
            ET_RESOURCE_PRESERVATION_MED = 170,
            ET_RESOURCE_PRESERVATION_HIGH = 180,
            ET_HAZARD_ENEMY_LOW = 200,
            ET_HAZARD_ENEMY_MED = 210,
            ET_HAZARD_ENEMY_HIGH = 220,
            ET_HAZARD_ENEMY_BOSS = 230,
            ET_HAZARD_ENVIRONMENT = 250,
            ET_POI = 300,
            ET_POI_NPC = 350
        };
        #endregion

        #region FIELDS
        [SerializeField, JsonIgnore]
        TileMapModule tileMap;
        #endregion

        public PathOSBehaviour(Texture2D icon, string name) : base(icon, name)
        {

        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }
    }
}
