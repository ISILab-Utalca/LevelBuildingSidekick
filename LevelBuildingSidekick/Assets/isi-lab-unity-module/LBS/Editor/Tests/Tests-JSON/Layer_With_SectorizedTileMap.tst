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
                        },
                        {
                            "rid": 1002
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
                    "class": "TileMapModule",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "TileMapModule",
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "Icon/Default",
                        "id": "LBSLayer",
                        "name": "Layer name",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
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
                    "tiles": [
                        {
                            "rid": 1003
                        }
                    ]
                }
            },
            {
                "rid": 1002,
                "type": {
                    "class": "SectorizedTileMapModule",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "SectorizedTileMapModule",
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "Icon/Default",
                        "id": "LBSLayer",
                        "name": "Layer name",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
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
                    "zones": [
                        {
                            "rid": 1004
                        }
                    ],
                    "pairs": [
                        {
                            "rid": 1005
                        }
                    ]
                }
            },
            {
                "rid": 1003,
                "type": {
                    "class": "LBSTile",
                    "ns": "LBS.Components.TileMap",
                    "asm": "LBS"
                },
                "data": {
                    "x": 0,
                    "y": 0
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
                        "x": 0.5,
                        "y": 0.5
                    },
                    "insideStyles": [],
                    "outsideStyles": []
                }
            },
            {
                "rid": 1005,
                "type": {
                    "class": "TileZonePair",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "tile": {
                        "rid": 1003
                    },
                    "zone": {
                        "rid": 1004
                    }
                }
            }
        ]
    }
}