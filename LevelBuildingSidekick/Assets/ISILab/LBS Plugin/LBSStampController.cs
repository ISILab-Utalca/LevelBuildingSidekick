using LBS.ElementView;
using LBS.View;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

// esto deberia llamarse population o algo asi
public class LBSStampController : LBSRepController<LBSStampGroupData>
{

    public LBSStampController(GraphView view,LBSStampGroupData data) : base(view ,data)
    {

    }

    public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
    {
        cmpe.menu.AppendAction("Stamp/Print", (dma) => { data.Print(); });

        var stamps = DirectoryTools.GetScriptablesByType<StampPresset>();
        foreach (var stamp in stamps)
        {
            cmpe.menu.AppendAction("Stamp/Create/" + stamp.name, (dma) => CreateStamp(cmpe, view, stamp));
        }

        cmpe.menu.AppendAction("Stamp/Clear", (dma) => { 
            data.Clear();
            view.DeleteElements(elements);
            });
    }

    public override void PopulateView(MainView view)
    {
        data.GetStamps().ForEach(s => {
            var v = new StampView(s);
            elements.Add(v);
            view.AddElement(v);
        });
    }

    public void CreateStamp(ContextualMenuPopulateEvent evt, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        var pos = (evt.localMousePosition - viewPos) / view.scale;

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new StampView(newStamp));
    }
}
