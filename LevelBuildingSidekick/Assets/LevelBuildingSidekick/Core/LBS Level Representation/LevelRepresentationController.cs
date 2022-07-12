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
            /*var data = Data as LevelRepresentationData;
            if(data.toolkit != null)
            {
                var toolkit = Activator.CreateInstance(data.toolkit.ControllerType, new object[] { data.toolkit, this });
                if (toolkit is ToolkitController)
                {
                    Toolkit = toolkit as ToolkitController;
                }
            }*/
            
        }


        public override void Update()
        {
            if(Toolkit != null)
            {
                Toolkit.Update();
            }
        }
    }
}

