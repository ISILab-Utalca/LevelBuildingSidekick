using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelBuildingSidekick
{
    public abstract class LevelRepresentationController : Controller
    {
        public ToolkitController Toolkit { get; set; }
        protected LevelRepresentationController(Data data) : base(data)
        {
        }

        public override void LoadData()
        {
            var data = Data as LevelRepresentationData;

            var toolkit = Activator.CreateInstance(data.toolkitData.ControllerType, new object[] {data.toolkitData, this});
            if(toolkit is ToolkitController)
            {
                Toolkit = toolkit as ToolkitController;
            }
        }

        public override void Update()
        {
            Toolkit.Update();
        }
    }
}

