using LBS.ElementView;
using LBS;
using LBS.Graph;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

// esto deberia llamarse population o algo asi
public class LBSStampController : LBSRepController<LBSStampGroupData>
{

    public LBSStampController(LBSGraphView view,LBSStampGroupData data) : base(view ,data)
    {

    }

    public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
    {
        cmpe.menu.AppendAction("Stamp/Print", (dma) => { data.Print(); });

        var mousePos = cmpe.localMousePosition;

        var stamps = DirectoryTools.GetScriptablesByType<StampPresset>();
        foreach (var stamp in stamps)
        {
            cmpe.menu.AppendAction("Stamp/Create/" + stamp.name, (dma) => CreateStamp(mousePos, view, stamp));
        }

        cmpe.menu.AppendAction("Stamp/Clear", (dma) => { 
            data.Clear();
            view.DeleteElements(elements);
            });
    }

    public override void PopulateView(MainView view)
    {
        data.GetStamps().ForEach(s => {
            var v = new LBSStampView(s, view);
            elements.Add(v);
            view.AddElement(v);
        });
    }

    public virtual void CreateStamp(Vector2 pos, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        pos = (pos - viewPos) / view.scale;

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new LBSStampView(newStamp,this.view));
    }

    public override string GetName()
    {
        return "Stamp Layer";
    }
}
