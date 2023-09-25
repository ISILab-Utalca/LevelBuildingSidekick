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
                "$values": [
                  {
                    "$id": "10",
                    "$type": "TileConnectionsPair, LBS",
                    "tile": {
                      "$ref": "7"
                    },
                    "connections": {
                      "$id": "11",
                      "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                      "$values": [
                        "Grass",
                        "Path",
                        "Grass",
                        "Path"
                      ]
                    },
                    "editedByIA": {
                      "$id": "12",
                      "$type": "System.Collections.Generic.List`1[[System.Boolean, mscorlib]], mscorlib",
                      "$values": [
                        true,
                        true,
                        true,
                        true
                      ]
                    },
                    "EditedByIA": {
                      "$ref": "12"
                    }
                  }
                ]
              },
              "id": "ConnectedTileMapModule"
            }
          ]
        },
        "behaviours": {
          "$id": "13",
          "$type": "System.Collections.Generic.List`1[[LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assitants": {
          "$id": "14",
          "$type": "System.Collections.Generic.List`1[[LBS.Assisstants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[LBS.Generator.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "16",
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
    "$id": "17",
    "$type": "System.Collections.Generic.List`1[[LBSQuest, LBS]], mscorlib",
    "$values": []
  }
}