using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ISILab.AI.Optimization;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using UnityEngine;

namespace ISILab.AI.Wrappers
{
    public class OptimizableModules : IOptimizable
    {
        #region FIELDS
        private List<LBSModule> modules;
        private double fitness;
        #endregion

        #region PROPERTIES
        public List<LBSModule> Modules => new List<LBSModule>(modules);

        public double Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public double xFitness { get; set; }
        public double yFitness { get; set; }

        #endregion

        #region CONSTRUCTORS
        public OptimizableModules(List<LBSModule> layer, double fitness = -1)
        {
            this.modules = layer;
            this.fitness = fitness;
        }
        #endregion

        #region METHODS
        public object Clone()
        {
            var clone = new List<LBSModule>(this.modules.Clone());
            return clone;
        }

        public IOptimizable CreateNew()
        {
            return Clone() as OptimizableModules;
        }
        #endregion
    }
}