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
              "$type": "BundleTileMap, LBS",
              "tiles": {
                "$id": "6",
                "$type": "System.Collections.Generic.List`1[[TileBundlePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "7",
                    "$type": "TileBundlePair, LBS",
                    "tile": {
                      "$id": "8",
                      "$type": "LBS.Components.TileMap.LBSTile, LBS",
                      "x": 0,
                      "y": 0
                    },
                    "bData": {
                      "$id": "9",
                      "$type": "LBS.Components.TileMap.BundleData, LBS",
                      "characteristics": {
                        "$id": "10",
                        "$type": "System.Collections.Generic.List`1[[LBSCharacteristic, LBS]], mscorlib",
                        "$values": []
                      },
                      "bundleTag": "data",
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
                    "$ref": "7"
                  }
                ]
              }
            }
          ]
        },
        "behaviours": {
          "$id": "11",
          "$type": "System.Collections.Generic.List`1[[LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assitants": {
          "$id": "12",
          "$type": "System.Collections.Generic.List`1[[LBS.Assisstants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "13",
          "$type": "System.Collections.Generic.List`1[[LBS.Generator.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "14",
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
    "$id": "15",
    "$type": "System.Collections.Generic.List`1[[LBSQuest, LBS]], mscorlib",
    "$values": []
  }
}