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
              "$type": "ISILab.LBS.Modules.BundleTileMap, LBS",
              "tiles": {
                "$id": "6",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.TileBundlePair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "7",
                    "$type": "ISILab.LBS.Modules.TileBundlePair, LBS",
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
                        "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Characteristics.LBSCharacteristic, LBS]], mscorlib",
                        "$values": [
                          {
                            "$id": "11",
                            "$type": "ISILab.LBS.Characteristics.LBSDirection, LBS",
                            "connections": {
                              "$id": "12",
                              "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                              "$values": [
                                "Road",
                                "",
                                "Road",
                                "Road"
                              ]
                            }
                          }
                        ]
                      },
                      "bundleName": "data"
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
          "$id": "13",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assistants": {
          "$id": "14",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "15",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "16",
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
    "$id": "17",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": []
  }
}