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
        "iconPath": "Icon/Default",
        "id": "LBSLayer",
        "name": "Layer name",
        "modules": {
          "$id": "4",
          "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSModule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "5",
              "$type": "SectorizedTileMapModule, LBS",
              "zones": {
                "$id": "6",
                "$type": "System.Collections.Generic.List`1[[Zone, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "7",
                    "$type": "Zone, LBS",
                    "id": "Zone-1",
                    "color": {
                      "r": 1.0,
                      "g": 0.0,
                      "b": 0.0,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.0,
                      "y": 0.0
                    },
                    "tagsBundles": {
                      "$id": "8",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  },
                  {
                    "$id": "9",
                    "$type": "Zone, LBS",
                    "id": "Zone-2",
                    "color": {
                      "r": 0.0,
                      "g": 0.0,
                      "b": 1.0,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.0,
                      "y": 0.0
                    },
                    "tagsBundles": {
                      "$id": "10",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  }
                ]
              },
              "pairs": {
                "$id": "11",
                "$type": "System.Collections.Generic.List`1[[TileZonePair, LBS]], mscorlib",
                "$values": []
              },
              "id": "SectorizedTileMapModule"
            },
            {
              "$id": "12",
              "$type": "ConnectedZonesModule, LBS",
              "edges": {
                "$id": "13",
                "$type": "System.Collections.Generic.List`1[[ZoneEdge, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "14",
                    "$type": "ZoneEdge, LBS",
                    "first": {
                      "$ref": "7"
                    },
                    "second": {
                      "$ref": "9"
                    }
                  }
                ]
              },
              "id": "ConnectedZonesModule"
            }
          ]
        },
        "behaviours": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assitants": {
          "$id": "16",
          "$type": "System.Collections.Generic.List`1[[LBS.Assisstants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[LBS.Generator.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "18",
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
    "$id": "19",
    "$type": "System.Collections.Generic.List`1[[LBSQuest, LBS]], mscorlib",
    "$values": []
  }
}