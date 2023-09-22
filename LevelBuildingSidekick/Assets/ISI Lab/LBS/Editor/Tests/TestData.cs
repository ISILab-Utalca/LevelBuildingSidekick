using LBS.Components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using Utility;
using System.IO;
using LBS.Settings;

public class TestData
{
    // Test folder path
    private static string path = LBSSettings.Instance.test.TestFolderPath;

    [Test]
    public void Save_And_Load_Empty_Level_Data()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Empty_Level.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Empty_Level.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Empty_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        lvl.AddLayer(new LBSLayer());

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Empty_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Empty_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Interior_Layer()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void Save_And_Load_Exterior_Layer()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void Save_And_Load_Population_Layer()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void Save_And_Load_XXX_Layer()
    {
        Assert.IsTrue(false);
    }

    // [TEST] Chequear que las referencias entre modulos sigan funcionando
    // [TEST] Cheaquear que los eventos esten conectados cuando corresponda
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX

}