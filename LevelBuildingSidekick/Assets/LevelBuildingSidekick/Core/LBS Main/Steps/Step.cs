using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace LevelBuildingSidekick
{
    public abstract class Step: Controller
    {
        private LevelRepresentationController _LevelRepresentationController;
        public LevelRepresentationController LevelRepresentation
        {
            get
            {
                return _LevelRepresentationController;
            }
            set
            {
                _LevelRepresentationController = value;
            }
        }

        protected Step(Data data) : base(data)
        {
        }
    }
}

