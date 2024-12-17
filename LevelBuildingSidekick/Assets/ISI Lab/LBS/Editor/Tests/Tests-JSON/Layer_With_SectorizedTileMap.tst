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
        "iconPath": "Icon/Default",
        "id": "LBSLayer",
        "name": "Layer name",
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
              "$type": "ISILab.LBS.Modules.SectorizedTileMapModule, LBS",
              "zones": {
                "$id": "9",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Components.Zone, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "10",
                    "$type": "ISILab.LBS.Components.Zone, LBS",
                    "id": "Zone-1",
                    "color": {
                      "r": 1.0,
                      "g": 0.0,
                      "b": 0.0,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.5,
                      "y": 0.5
                    },
                    "insideStyles": {
                      "$id": "11",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    },
                    "outsideStyles": {
                      "$id": "12",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  }
                ]
              },
              "pairs": {
                "$id": "13",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.TileZonePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "14",
                    "$type": "ISILab.LBS.Modules.TileZonePair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "zone": {
                      "$ref": "10"
                    }
                  }
                ]
              },
              "id": "SectorizedTileMapModule",
              "owner": {
                "$ref": "3"
              }
            }
          ]
        },
        "behaviours": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assistants": {
          "$id": "16",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "18",
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
    "$id": "19",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": []
  }
}