using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public abstract class ManipulateTeselation : LBSManipulator
    {
        protected TileMapModule module;

        public ManipulateTeselation() : base()
        {
            Feedback = new AreaFeedback();
            Feedback.fixToTeselation = true;
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            module = layer.GetModule<TileMapModule>();
            Feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
            
        }
    }
}