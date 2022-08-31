using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

namespace LBS.View
{
    public class FreeStampView : LBSBaseView
    {
        public new class UxmlFactory : UxmlFactory<FreeStampView, GraphView.UxmlTraits> { }

        public FreeStampView()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("PoPulationUXML");
            //styleSheets.Add(styleSheet);

            var stamps = LBSController.CurrentLevel.data.GetRepresentation<LBSStampGroupData>();

            controller = new LBSStampController(stamps);
        }

        public override void Populate<T>(T data)
        {
            throw new System.NotImplementedException();
        }
    }
}