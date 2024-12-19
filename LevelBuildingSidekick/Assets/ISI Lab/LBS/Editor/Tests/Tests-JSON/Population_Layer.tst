{
  "$id": "1",
  "$type": "ISILab.LBS.LBSLevelData, LBS",
  "layers": {
    "$id": "2",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": [
      {
        "$id": "3",
        "$type": "LBS.Components.LBSLayer, LBS",
        "visible": true,
        "blocked": false,
        "iconPath": "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png",
        "id": "Population",
        "name": "Layer Population",
        "modules": {
          "$id": "4",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.LBSModule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "5",
              "$type": "ISILab.LBS.Modules.TileMapModule, LBS",
              "tiles": {
                "$id": "6",
                "$type": "System.Collections.Generic.List`1[[LBS.Components.TileMap.LBSTile, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "7",
                    "$type": "LBS.Components.TileMap.LBSTile, LBS",
                    "x": 0,
                    "y": 0
                  }
                ]
              },
              "id": "TileMapModule",
              "owner": {
                "$ref": "3"
              }
            },
            {
              "$id": "8",
              "$type": "ISILab.LBS.Modules.BundleTileMap, LBS",
              "tiles": {
                "$id": "9",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.TileBundlePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "10",
                    "$type": "ISILab.LBS.Modules.TileBundlePair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "bData": {
                      "$id": "11",
                      "$type": "LBS.Components.TileMap.BundleData, LBS",
                      "characteristics": {
                        "$id": "12",
                        "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Characteristics.LBSCharacteristic, LBS]], mscorlib",
                        "$values": [
                          {
                            "$id": "13",
                            "$type": "ISILab.LBS.Characteristics.LBSTagsCharacteristic, LBS",
                            "tagName": ""
                          }
                        ]
                      },
                      "bundleName": "Goblin"
                    },
                    "rotation": {
                      "x": 1.0,
                      "y": 0.0
                    }
                  }
                ]
              },
              "id": "BundleTileMap",
              "owner": {
                "$ref": "3"
              }
            }
          ]
        },
        "behaviours": {
          "$id": "14",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": [
            {
              "$id": "15",
              "$type": "ISILab.LBS.Behaviours.PopulationBehaviour, LBS",
              "visible": true,
              "name": "Population Behavior"
            }
          ]
        },
        "assistants": {
          "$id": "16",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "18",
              "$type": "ISILab.LBS.Generators.PopulationRuleGenerator, LBS"
            }
          ]
        },
        "settings": {
          "$id": "19",
          "$type": "ISILab.LBS.Generators.Generator3D+Settings, LBS",
          "scale": {
            "x": 2.0,
            "y": 2.0
          },
          "resize": {
            "x": 0.0,
            "y": 0.0
          },
          "position": {
            "x": 0.0,
            "y": 0.0,
            "z": 0.0
          },
          "name": "DEFAULT"
        }
      }
    ]
  },
  "quests": {
    "$id": "20",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": []
  }
}