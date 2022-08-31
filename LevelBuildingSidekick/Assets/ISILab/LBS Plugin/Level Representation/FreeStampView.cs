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
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("PoPulationUXML");
            //styleSheets.Add(styleSheet);

            var stamps = LBSController.CurrentLevel.data.GetRepresentation<LBSStampGroupData>();

            controller = new LBSStampController(stamps);
        }

        public override void Populate<T>(T value)
        {
            var data = value as LBSStampGroupData;
            if (data == null)
                Debug.LogWarning("[Error]: The information you are trying to upload cannot be displayed in this view.");

            DeleteElements(graphElements);
            data.GetStamps().ForEach(s => AddElementView(s));
        }

        public void AddElementView(StampData data)
        {
            var view = new StampView(data);
            AddElement(view);
        }
    }
}