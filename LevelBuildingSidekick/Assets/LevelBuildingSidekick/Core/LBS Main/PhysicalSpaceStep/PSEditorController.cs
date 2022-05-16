using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public class PSEditorController : Step
    {
        public LevelRepresentationController level;

        public PSEditorController()
        {
            View = new PSEditorView(this);
        }

        public override void LoadData()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
        }
    }
}

