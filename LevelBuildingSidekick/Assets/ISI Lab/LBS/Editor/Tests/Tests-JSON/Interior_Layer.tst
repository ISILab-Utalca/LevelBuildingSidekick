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
        "iconPath": "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/interior-design.png",
        "id": "Interior",
        "name": "Layer Interior",
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
              "$type": "ISILab.LBS.Modules.ConnectedTileMapModule, LBS",
              "connectedDirections": 4,
              "pairs": {
                "$id": "9",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.TileConnectionsPair, LBS]], mscorlib",
                "$values": []
              },
              "id": "ConnectedTileMapModule",
              "owner": {
                "$ref": "3"
              }
            },
            {
              "$id": "10",
              "$type": "ISILab.LBS.Modules.SectorizedTileMapModule, LBS",
              "zones": {
                "$id": "11",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Components.Zone, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "12",
                    "$type": "ISILab.LBS.Components.Zone, LBS",
                    "id": "Zone: 0",
                    "color": {
                      "r": 0.1875,
                      "g": 0.6875,
                      "b": 0.625,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.5,
                      "y": 0.5
                    },
                    "insideStyles": {
                      "$id": "13",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    },
                    "outsideStyles": {
                      "$id": "14",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  },
                  {
                    "$id": "15",
                    "$type": "ISILab.LBS.Components.Zone, LBS",
                    "id": "Zone: 1",
                    "color": {
                      "r": 0.6875,
                      "g": 0.6875,
                      "b": 0.6875,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.0,
                      "y": 0.0
                    },
                    "insideStyles": {
                      "$id": "16",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    },
                    "outsideStyles": {
                      "$id": "17",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  }
                ]
              },
              "pairs": {
                "$id": "18",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.TileZonePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "19",
                    "$type": "ISILab.LBS.Modules.TileZonePair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "zone": {
                      "$ref": "12"
                    }
                  }
                ]
              },
              "id": "SectorizedTileMapModule",
              "owner": {
                "$ref": "3"
              }
            },
            {
              "$id": "20",
              "$type": "ISILab.LBS.Modules.ConnectedZonesModule, LBS",
              "edges": {
                "$id": "21",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Components.ZoneEdge, LBS]], mscorlib",
                "$values": []
              },
              "id": "ConnectedZonesModule",
              "owner": {
                "$ref": "3"
              }
            },
            {
              "$id": "22",
              "$type": "ISILab.LBS.Modules.ConstrainsZonesModule, LBS",
              "pairs": {
                "$id": "23",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.ConstraintPair, LBS]], mscorlib",
                "$values": []
              },
              "id": "ConstrainsZonesModule",
              "owner": {
                "$ref": "3"
              }
            }
          ]
        },
        "behaviours": {
          "$id": "24",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": [
            {
              "$id": "25",
              "$type": "ISILab.LBS.Behaviours.SchemaBehaviour, LBS",
              "visible": true,
              "name": "Schema behaviour",
              "Connections": {
                "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                "$values": [
                  "Wall",
                  "Door",
                  "Empty",
                  "Wall",
                  "Door",
                  "Empty",
                  "Wall",
                  "Door",
                  "Empty",
                  "Wall",
                  "Door",
                  "Empty"
                ]
              }
            }
          ]
        },
        "assistants": {
          "$id": "26",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "27",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "28",
              "$type": "ISILab.LBS.Generators.SchemaRuleGenerator, LBS",
              "deltaWall": 1.0
            },
            {
              "$id": "29",
              "$type": "ISILab.LBS.Generators.SchemaRuleGeneratorExteriror, LBS",
              "deltaWall": 1.0
            }
          ]
        },
        "settings": {
          "$id": "30",
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
    "$id": "31",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": []
  }
}