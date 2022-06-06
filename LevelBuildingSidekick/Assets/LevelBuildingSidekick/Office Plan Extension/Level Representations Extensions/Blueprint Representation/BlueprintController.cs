using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintController : LevelRepresentationController
{
    public BlueprintController(Data data) : base(data)
    {
        View = new BlueprintView(this);
    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
    }
}
