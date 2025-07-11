using LBS;
using LBS.VisualElements;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using ISILab.Extensions;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Drawers;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;
using NUnit.Framework.Internal;

//This is meant to be an updated version of AssistantMapEliteEditor that uses PopulationAssistantWindow as its basis.
//It's necessary for it to use the running commands for the algorithm, so it's here.
namespace ISILab.LBS.AI.Assistants.Editor
{
    [LBSCustomEditor("Assistant Map Elite", typeof(AssistantMapElite))]
    public class PopulationAssistantEditor : LBSCustomEditor
    {

        public PopulationAssistantEditor(object target) : base(target)
        {
            SetInfo(target);
        }

        public void Run()
        {
            var assistant = target as AssistantMapElite;
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
        }

        protected override VisualElement CreateVisualElement()
        {
            //We can use this later to add the windows themselves!!
            var ve = new VisualElement();
            ve.style.display = DisplayStyle.None;
            return ve;
        }
    }
}