using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelBuildingSidekick
{
    public abstract class Step: Controller
    {
        LevelRepresentationController levelRepresentation;

        protected Step(Data data) : base(data)
        {
        }
    }
}

