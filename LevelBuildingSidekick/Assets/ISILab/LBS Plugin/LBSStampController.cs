using LBS.View;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSStampController : LBSViewController
{
    LBSStampGroupData data;
    FreeStampView view;

    public LBSStampController(LBSStampGroupData data) : base(data)
    {
        this.data = data;

        var stamps = DirectoryTools.GetScriptablesByType<StampPresset>();

        foreach (var stamp in stamps)
        {
            contextActions.Add(new ContextAction(
            "Create Stamp/" + stamp.name,
            (dma,view, evt) => { CreateStamp(evt,view,stamp); }
            ));
        }

        contextActions.Add(new ContextAction(
            "Clear",
            (dma, view, evt) => { 
                data.Clear();
                view.DeleteElements(view.graphElements);
            }));

        contextActions.Add(new ContextAction(
            "Print",
            (dma, view, evt) => { PrintData(); }
            ));
    }

    public void CreateStamp(ContextualMenuPopulateEvent evt, LBSBaseView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        var pos = (evt.localMousePosition - viewPos) / view.scale;

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new StampView(newStamp));
    }

    public void PrintData()
    {
        data.Print();
    }

}
