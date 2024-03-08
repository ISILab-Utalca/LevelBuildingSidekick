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
        "iconPath": "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/interior-design.png",
        "id": "Interior",
        "name": "Layer Interior",
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
              "$type": "ConnectedTileMapModule, LBS",
              "connectedDirections": 4,
              "pairs": {
                "$id": "9",
                "$type": "System.Collections.Generic.List`1[[TileConnectionsPair, LBS]], mscorlib",
                "$values": []
              },
              "id": "ConnectedTileMapModule"
            },
            {
              "$id": "10",
              "$type": "SectorizedTileMapModule, LBS",
              "zones": {
                "$id": "11",
                "$type": "System.Collections.Generic.List`1[[Zone, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "12",
                    "$type": "Zone, LBS",
                    "id": "Zone: 0",
                    "color": {
                      "r": 0.625,
                      "g": 0.6875,
                      "b": 0.1875,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.0,
                      "y": 0.0
                    },
                    "tagsBundles": {
                      "$id": "13",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  },
                  {
                    "$id": "14",
                    "$type": "Zone, LBS",
                    "id": "Zone: 1",
                    "color": {
                      "r": 0.0625,
                      "g": 0.4375,
                      "b": 0.1875,
                      "a": 1.0
                    },
                    "pivot": {
                      "x": 0.0,
                      "y": 0.0
                    },
                    "tagsBundles": {
                      "$id": "15",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": []
                    }
                  }
                ]
              },
              "pairs": {
                "$id": "16",
                "$type": "System.Collections.Generic.List`1[[TileZonePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "17",
                    "$type": "TileZonePair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "zone": {
                      "$ref": "12"
                    }
                  }
                ]
              },
              "id": "SectorizedTileMapModule"
            },
            {
              "$id": "18",
              "$type": "ConnectedZonesModule, LBS",
              "edges": {
                "$id": "19",
                "$type": "System.Collections.Generic.List`1[[ZoneEdge, LBS]], mscorlib",
                "$values": []
              },
              "id": "ConnectedZonesModule"
            },
            {
              "$id": "20",
              "$type": "ConstrainsZonesModule, LBS",
              "pairs": {
                "$id": "21",
                "$type": "System.Collections.Generic.List`1[[ConstraintPair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "22",
                    "$type": "ConstraintPair, LBS",
                    "zone": {
                      "$ref": "12"
                    },
                    "constraint": {
                      "$id": "23",
                      "$type": "Constraint, LBS",
                      "minWidth": 3.0,
                      "minHeight": 3.0,
                      "maxWidth": 4.0,
                      "maxHeight": 4.0
                    }
                  },
                  {
                    "$id": "24",
                    "$type": "ConstraintPair, LBS",
                    "zone": {
                      "$ref": "14"
                    },
                    "constraint": {
                      "$id": "25",
                      "$type": "Constraint, LBS",
                      "minWidth": 3.0,
                      "minHeight": 3.0,
                      "maxWidth": 4.0,
                      "maxHeight": 4.0
                    }
                  }
                ]
              },
              "id": "ConstrainsZonesModule"
            }
          ]
        },
        "behaviours": {
          "$id": "26",
          "$type": "System.Collections.Generic.List`1[[LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": [
            {
              "$id": "27",
              "$type": "SchemaBehaviour, LBS",
              "name": "Schema behaviour",
              "Connections": {
                "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                "$values": [
                  "Wall",
                  "Door",
                  "Empty"
                ]
              }
            }
          ]
        },
        "assitants": {
          "$id": "28",
          "$type": "System.Collections.Generic.List`1[[LBS.Assisstants.LBSAssistant, LBS]], mscorlib",
          "$values": [
            {
              "$id": "29",
              "$type": "HillClimbingAssistant, LBS",
              "name": "HillClimbing",
              "TileMapMod": {
                "$ref": "5"
              },
              "AreasMod": {
                "$ref": "10"
              },
              "GraphMod": {
                "$ref": "18"
              },
              "ConstrainsZonesMod": {
                "$ref": "20"
              }
            }
          ]
        },
        "generatorRules": {
          "$id": "30",
          "$type": "System.Collections.Generic.List`1[[LBS.Generator.LBSGeneratorRule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "31",
              "$type": "SchemaRuleGenerator, LBS",
              "deltaWall": 1.0
            },
            {
              "$id": "32",
              "$type": "SchemaRuleGeneratorExteriror, LBS"
            }
          ]
        },
        "settings": {
          "$id": "33",
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
    "$id": "34",
    "$type": "System.Collections.Generic.List`1[[LBSQuest, LBS]], mscorlib",
    "$values": []
  }
}