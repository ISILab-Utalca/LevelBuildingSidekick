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
        "iconPath": "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/pine-tree.png",
        "id": "Exterior",
        "name": "Layer Exterior",
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
                "$values": [
                  {
                    "$id": "10",
                    "$type": "ISILab.LBS.Modules.TileConnectionsPair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "connections": {
                      "$id": "11",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": [
                        "Grass",
                        "",
                        "",
                        ""
                      ]
                    },
                    "editedByIA": {
                      "$id": "12",
                      "$type": "System.Collections.Generic.List`1[[System.Boolean, mscorlib]], mscorlib",
                      "$values": [
                        true,
                        false,
                        false,
                        false
                      ]
                    },
                    "EditedByIA": {
                      "$ref": "12"
                    }
                  }
                ]
              },
              "id": "ConnectedTileMapModule",
              "owner": {
                "$ref": "3"
              }
            }
          ]
        },
        "behaviours": {
          "$id": "13",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": [
            {
              "$id": "14",
              "$type": "ISILab.LBS.Behaviours.ExteriorBehaviour, LBS",
              "targetBundle": "Exterior_Plains_Draw",
              "visible": true,
              "name": "Exteriror behaviour"
            }
          ]
        },
        "assistants": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": [
            {
              "$id": "16",
              "$type": "ISILab.LBS.Assistants.AssistantWFC, LBS",
              "overrideValues": false,
              "targetBundle": "Exterior_Plains",
              "visible": true,
              "name": "Assistant WFC"
            }
          ]
        },
        "generatorRules": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": [
            {
              "$id": "18",
              "$type": "ISILab.LBS.Generators.ExteriorRuleGenerator, LBS"
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