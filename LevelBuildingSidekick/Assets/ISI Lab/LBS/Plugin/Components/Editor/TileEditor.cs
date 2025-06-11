using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components.TileMap;
using ISILab.LBS.Editor;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Tile", typeof(LBSTile))]
    public class TileEditor : LBSCustomEditor
    {
        private Vector2Field field;

        public TileEditor()
        {
            CreateVisualElement();
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var t = paramTarget as LBSTile;

            field.value = t.Position;
        }

        protected override VisualElement CreateVisualElement()
        {
            field = new Vector2Field("Position");
            field.SetEnabled(false);
            Add(field);
            style.flexGrow = 1;

            return this;
        }
    }
}