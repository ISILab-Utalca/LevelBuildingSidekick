using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelBuildingSidekick
{
    public class PSEditorController : Step
    {
        public PSEditorController(Data data) : base(data)
        {
            View = new PSEditorView(this);
            //LoadData();
        }

        public override void LoadData()
        {
            var data = Data as PSEditorData;
            var level =  Activator.CreateInstance(data.levelData.ControllerType, new object[] { data.levelData });
            //Debug.Log("Hi: " + level);
            if (level is LevelRepresentationController)
            {
                LevelRepresentation = (level as LevelRepresentationController);
                //Level.Data = data.levelData;
            }
        }

        public override void Update()
        {
            LevelRepresentation.Update();
        }
    }
}

