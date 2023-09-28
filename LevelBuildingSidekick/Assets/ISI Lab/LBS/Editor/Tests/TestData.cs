using LBS.Components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using Utility;
using System.IO;
using LBS.Settings;
using System.Linq;
using LBS.Components.TileMap;
using UnityEngine.Tilemaps;
using System;
using LBS.Bundles;

public class Test_Simple_Module
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
    public void Save_And_Load_TileMap_Module()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add some data
        tileMap.AddTile(new LBSTile(new Vector2(0, 0)));

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_TileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_TileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Connected_TileMap()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap module
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add ConnectedTileMap module
        var connectedTileMap = new ConnectedTileMapModule();
        layer.AddModule(connectedTileMap);

        // Add some data
        var tile = new LBSTile(new Vector2(0, 0));
        tileMap.AddTile(tile);
        connectedTileMap.AddPair(tile, new List<string>() { "Grass", "Path", "Grass", "Path" }, new List<bool>() { true, true, true, true });

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_ConnectedTileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_ConnectedTileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Sectorized_TileMap()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap module
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add Sectorized module
        var sectoerized = new SectorizedTileMapModule();
        layer.AddModule(sectoerized);

        // Add some data
        var tile = new LBSTile(new Vector2(0, 0));
        tileMap.AddTile(tile);
        var zone = new Zone("Zone-1", Color.red);
        sectoerized.AddZone(zone);
        sectoerized.AddTile(tile, zone);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_SectorizedTileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_SectorizedTileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Connected_Zones()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add Sectorized module
        var sectoerized = new SectorizedTileMapModule();
        layer.AddModule(sectoerized);

        // Add Connected Zones
        var connectedZones = new ConnectedZonesModule();
        layer.AddModule(connectedZones);

        // Add some data
        var zone1 = new Zone("Zone-1", Color.red);
        var zone2 = new Zone("Zone-2", Color.blue);
        sectoerized.AddZone(zone1);
        sectoerized.AddZone(zone2);
        connectedZones.AddEdge(zone1, zone2);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_ConnectedZones.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_ConnectedZones.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Constraints_Zones()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add Constraints zones
        var constraint = new ConstrainsZonesModule();
        layer.AddModule(constraint);

        // Add some data
        var zone1 = new Zone("Zone-1", Color.red);
        var zone2 = new Zone("Zone-2", Color.blue);
        constraint.AddPair(zone1, new Vector2(3, 3), new Vector2(4, 4));
        constraint.AddPair(zone2, new Vector2(5, 5), new Vector2(6, 6));

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_ConstraintsZones.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_ConstraintsZones.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Bundle_TileMap()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add BundleTile
        var bundleMap = new BundleTileMap();
        layer.AddModule(bundleMap);

        // Get some bundle
        var bundle = LBSAssetsStorage.Instance.Get<Bundle>()[0];
        var dir = Directions.Bidimencional.All[0];

        // Add some data
        var tile = new LBSTile(new Vector2(0, 0));
        bundleMap.AddTile(tile, new BundleData("data", new List<LBSCharacteristic>(bundle.Characteristics)), dir);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_BundleTileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_BundleTileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }
}

public class Test_Template_Layers
{
    // Test folder path
    private static string path = LBSSettings.Instance.test.TestFolderPath;

    [Test]
    public void Save_And_Load_Interior_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Get interior presset
        var template = LBSAssetsStorage.Instance.Get<LayerTemplate>().First(t => t.name.Contains("Interior"));

        // Clone Interior presset
        var layer = template.layer.Clone() as LBSLayer;
        lvl.AddLayer(layer);

        // Add some data
        var schemaBH = layer.Behaviours[0] as SchemaBehaviour;
        schemaBH.AddZone();
        schemaBH.AddZone();
        schemaBH.AddTile(new Vector2Int(0, 0), schemaBH.Zones[0]);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Interior_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Interior_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);

    }

    [Test]
    public void Save_And_Load_Exterior_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Get interior presset
        var template = LBSAssetsStorage.Instance.Get<LayerTemplate>().First(t => t.name.Contains("Exterior"));

        // Clone Interior presset
        var layer = template.layer.Clone() as LBSLayer;
        lvl.AddLayer(layer);

        // Get Bundle
        var bundle = LBSAssetsStorage.Instance.Get<Bundle>().First(b => b.name.Contains("Exterior_Plains"));

        // Add some data
        var exteriorBH = layer.Behaviours[0] as ExteriorBehaviour;
        exteriorBH.TargetBundle = bundle.Name;
        var tile = new LBSTile(new Vector2(0, 0));
        exteriorBH.AddTile(tile);
        exteriorBH.SetConnection(tile, 0, "Grass", true);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Exterior_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Exterior_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Population_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Get interior presset
        var template = LBSAssetsStorage.Instance.Get<LayerTemplate>().First(t => t.name.Contains("Population"));

        // Clone Interior presset
        var layer = template.layer.Clone() as LBSLayer;
        lvl.AddLayer(layer);

        // Get Bundle
        var bundle = LBSAssetsStorage.Instance.Get<Bundle>().First(b => b.name.Contains("Goblin"));

        // Add some data
        var populationBH = layer.Behaviours[0] as PopulationBehaviour;
        populationBH.AddTile(new Vector2Int(0, 0), bundle);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Exterior_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Exterior_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);

    }
}