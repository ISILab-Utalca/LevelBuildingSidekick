{
  "$id": "1",
  "$type": "LBSLevelData, LBS",
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
          "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSModule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "5",
              "$type": "LBS.Components.TileMap.TileMapModule, LBS",
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
              "id": "TileMapModule"
            },
            {
              "$id": "8",
              "$type": "BundleTileMap, LBS",
              "tiles": {
                "$id": "9",
                "$type": "System.Collections.Generic.List`1[[TileBundlePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "10",
                    "$type": "TileBundlePair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "bData": {
                      "$id": "11",
                      "$type": "LBS.Components.TileMap.BundleData, LBS",
                      "characteristics": {
                        "$id": "12",
                        "$type": "System.Collections.Generic.List`1[[LBSCharacteristic, LBS]], mscorlib",
                        "$values": []
                      },
                      "bundleTag": "Goblin",
                      "Characteristics": {
                        "$type": "System.Collections.Generic.List`1[[LBSCharacteristic, LBS]], mscorlib",
                        "$values": []
                      }
                    },
                    "rotation": {
                      "x": 1.0,
                      "y": 0.0
                    },
                    "Rotation": {
                      "x": 1.0,
                      "y": 0.0
                    }
                  }
                ]
              },
              "id": "BundleTileMap",
              "Tiles": {
                "$type": "System.Collections.Generic.List`1[[TileBundlePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$ref": "10"
                  }
                ]
              }
            }
          ]
        },
        "behaviours": {
          "$id": "13",
          "$type": "System.Collections.Generic.List`1[[LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": [
            {
              "$id": "14",
              "$type": "PopulationBehaviour, LBS",
              "name": "Population Behavior"
            }
          ]
        },
        "assitants": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[LBS.Assisstants.LBSAssistant, LBS]], mscorlib",
          "$values": [
            {
              "$id": "16",
              "$type": "AssistantMapElite, LBS",
              "name": ""
            }
          ]
        },
        "generatorRules": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[LBS.Generator.LBSGeneratorRule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "18",
              "$type": "LBS.Generator.PopulationRuleGenerator, LBS"
            }
          ]
        },
        "settings": {
          "$id": "19",
          "$type": "LBS.Generator.Generator3D+Settings, LBS",
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
    "$type": "System.Collections.Generic.List`1[[LBSQuest, LBS]], mscorlib",
    "$values": []
  }
}