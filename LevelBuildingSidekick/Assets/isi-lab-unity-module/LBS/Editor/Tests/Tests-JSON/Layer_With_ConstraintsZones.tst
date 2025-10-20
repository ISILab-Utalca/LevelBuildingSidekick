{
    "layers": [
        {
            "rid": 1000
        }
    ],
    "quests": [],
    "savedLayerMaps": [],
    "contextLayers": [],
    "references": {
        "version": 2,
        "RefIds": [
            {
                "rid": 1000,
                "type": {
                    "class": "LBSLayer",
                    "ns": "LBS.Components",
                    "asm": "LBS"
                },
                "data": {
                    "visible": true,
                    "blocked": false,
                    "iconGuid": "Icon/Default",
                    "id": "LBSLayer",
                    "name": "Layer name",
                    "modules": [
                        {
                            "rid": 1001
                        }
                    ],
                    "behaviours": [],
                    "assistants": [],
                    "generatorRules": [],
                    "settings": {
                        "useBundleSize": false,
                        "lightVolume": false,
                        "reflectionProbe": false,
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
                        "replacePrevious": true,
                        "name": "DEFAULT",
                        "rootParentName": "Root_Parent"
                    },
                    "index": 0
                }
            },
            {
                "rid": 1001,
                "type": {
                    "class": "ConstrainsZonesModule",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "ConstrainsZonesModule",
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "Icon/Default",
                        "id": "LBSLayer",
                        "name": "Layer name",
                        "modules": [
                            {
                                "rid": 1001
                            }
                        ],
                        "behaviours": [],
                        "assistants": [],
                        "generatorRules": [],
                        "settings": {
                            "useBundleSize": false,
                            "lightVolume": false,
                            "reflectionProbe": false,
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
                            "replacePrevious": true,
                            "name": "DEFAULT",
                            "rootParentName": "Root_Parent"
                        },
                        "index": 0
                    },
                    "pairs": [
                        {
                            "rid": 1002
                        },
                        {
                            "rid": 1003
                        }
                    ]
                }
            },
            {
                "rid": 1002,
                "type": {
                    "class": "ConstraintPair",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "zone": {
                        "rid": 1004
                    },
                    "constraint": {
                        "rid": 1005
                    }
                }
            },
            {
                "rid": 1003,
                "type": {
                    "class": "ConstraintPair",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "zone": {
                        "rid": 1006
                    },
                    "constraint": {
                        "rid": 1007
                    }
                }
            },
            {
                "rid": 1004,
                "type": {
                    "class": "Zone",
                    "ns": "ISILab.LBS.Components",
                    "asm": "LBS"
                },
                "data": {
                    "id": "Zone-1",
                    "color": {
                        "r": 1.0,
                        "g": 0.0,
                        "b": 0.0,
                        "a": 1.0
                    },
                    "borderThickness": 0.0,
                    "pivot": {
                        "x": 0.0,
                        "y": 0.0
                    },
                    "insideStyles": [],
                    "outsideStyles": []
                }
            },
            {
                "rid": 1005,
                "type": {
                    "class": "Constraint",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "minWidth": 3.0,
                    "minHeight": 3.0,
                    "maxWidth": 4.0,
                    "maxHeight": 4.0
                }
            },
            {
                "rid": 1006,
                "type": {
                    "class": "Zone",
                    "ns": "ISILab.LBS.Components",
                    "asm": "LBS"
                },
                "data": {
                    "id": "Zone-2",
                    "color": {
                        "r": 0.0,
                        "g": 0.0,
                        "b": 1.0,
                        "a": 1.0
                    },
                    "borderThickness": 0.0,
                    "pivot": {
                        "x": 0.0,
                        "y": 0.0
                    },
                    "insideStyles": [],
                    "outsideStyles": []
                }
            },
            {
                "rid": 1007,
                "type": {
                    "class": "Constraint",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "minWidth": 5.0,
                    "minHeight": 5.0,
                    "maxWidth": 6.0,
                    "maxHeight": 6.0
                }
            }
        ]
    }
}