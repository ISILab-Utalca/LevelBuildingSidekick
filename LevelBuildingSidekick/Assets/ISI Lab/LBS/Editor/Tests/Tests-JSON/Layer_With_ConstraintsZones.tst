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
              "$type": "ISILab.LBS.Modules.ConstrainsZonesModule, LBS",
              "pairs": {
                "$id": "6",
                "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Modules.ConstraintPair, LBS]], mscorlib",
                "$values": [
                  {
                    "$id": "7",
                    "$type": "ISILab.LBS.Modules.ConstraintPair, LBS",
                    "zone": {
                      "$id": "8",
                      "$type": "ISILab.LBS.Components.Zone, LBS",
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
                      "insideStyles": {
                        "$id": "9",
                        "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                        "$values": []
                      },
                      "outsideStyles": {
                        "$id": "10",
                        "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                        "$values": []
                      }
                    },
                    "constraint": {
                      "$id": "11",
                      "$type": "ISILab.LBS.Modules.Constraint, LBS",
                      "minWidth": 3.0,
                      "minHeight": 3.0,
                      "maxWidth": 4.0,
                      "maxHeight": 4.0
                    }
                  },
                  {
                    "$id": "12",
                    "$type": "ISILab.LBS.Modules.ConstraintPair, LBS",
                    "zone": {
                      "$id": "13",
                      "$type": "ISILab.LBS.Components.Zone, LBS",
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
                      "insideStyles": {
                        "$id": "14",
                        "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                        "$values": []
                      },
                      "outsideStyles": {
                        "$id": "15",
                        "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
                        "$values": []
                      }
                    },
                    "constraint": {
                      "$id": "16",
                      "$type": "ISILab.LBS.Modules.Constraint, LBS",
                      "minWidth": 5.0,
                      "minHeight": 5.0,
                      "maxWidth": 6.0,
                      "maxHeight": 6.0
                    }
                  }
                ]
              },
              "id": "ConstrainsZonesModule",
              "owner": {
                "$ref": "3"
              }
            }
          ]
        },
        "behaviours": {
          "$id": "17",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Behaviours.LBSBehaviour, LBS]], mscorlib",
          "$values": []
        },
        "assistants": {
          "$id": "18",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Assistants.LBSAssistant, LBS]], mscorlib",
          "$values": []
        },
        "generatorRules": {
          "$id": "19",
          "$type": "System.Collections.Generic.List`1[[ISILab.LBS.Generators.LBSGeneratorRule, LBS]], mscorlib",
          "$values": []
        },
        "settings": {
          "$id": "20",
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
    "$id": "21",
    "$type": "System.Collections.Generic.List`1[[LBS.Components.LBSLayer, LBS]], mscorlib",
    "$values": []
  }
}