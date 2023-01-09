using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Generator
{
    public abstract class Generator
    {
        public abstract GameObject Generate();

        public abstract void Init(LevelData levelData);
    }

    public enum PivotType //PyshicStep
    {
        Center,
        Edge
    }
}