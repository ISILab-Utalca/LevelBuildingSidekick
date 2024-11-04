using ISILab.Commons.Utility.Editor;
using ISILab.LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//GABO TODO: Terminar
namespace ISILab.LBS.VisualElements
{
    public class PathOSTriggerInfoPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<PathOSTriggerInfoPanel, UxmlTraits> { }
        #endregion

        #region FIELDS
        #endregion

        #region FIELDS VIEW
        #endregion
        private ListView obstacleList;
        private ListView tagList;

        #region CONSTRUCTORS
        public PathOSTriggerInfoPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTriggerInfoPanel");
            visualTree.CloneTree(this);


        }
            #endregion


        }
}
