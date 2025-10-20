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
                    "iconGuid": "02a644759487ae249bc3a20d019c8745",
                    "id": "Exterior",
                    "name": "Layer Exterior",
                    "modules": [
                        {
                            "rid": 1001
                        },
                        {
                            "rid": 1002
                        }
                    ],
                    "behaviours": [
                        {
                            "rid": 1003
                        }
                    ],
                    "assistants": [
                        {
                            "rid": 1004
                        }
                    ],
                    "generatorRules": [
                        {
                            "rid": 1005
                        }
                    ],
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
                        "iconGuid": "02a644759487ae249bc3a20d019c8745",
                        "id": "Exterior",
                        "name": "Layer Exterior",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
                            }
                        ],
                        "behaviours": [
                            {
                                "rid": 1003
                            }
                        ],
                        "assistants": [
                            {
                                "rid": 1004
                            }
                        ],
                        "generatorRules": [
                            {
                                "rid": 1005
                            }
                        ],
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
                            "rid": 1006
                        }
                    ]
                }
            },
            {
                "rid": 1002,
                "type": {
                    "class": "ConnectedTileMapModule",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "ConnectedTileMapModule",
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "02a644759487ae249bc3a20d019c8745",
                        "id": "Exterior",
                        "name": "Layer Exterior",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
                            }
                        ],
                        "behaviours": [
                            {
                                "rid": 1003
                            }
                        ],
                        "assistants": [
                            {
                                "rid": 1004
                            }
                        ],
                        "generatorRules": [
                            {
                                "rid": 1005
                            }
                        ],
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
                    "connectedDirections": 4,
                    "gridType": 1,
                    "pairs": [
                        {
                            "rid": 1007
                        }
                    ]
                }
            },
            {
                "rid": 1003,
                "type": {
                    "class": "ExteriorBehaviour",
                    "ns": "ISILab.LBS.Behaviours",
                    "asm": "LBS"
                },
                "data": {
                    "visible": true,
                    "ownerLayerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "02a644759487ae249bc3a20d019c8745",
                        "id": "Exterior",
                        "name": "Layer Exterior",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
                            }
                        ],
                        "behaviours": [
                            {
                                "rid": 1003
                            }
                        ],
                        "assistants": [
                            {
                                "rid": 1004
                            }
                        ],
                        "generatorRules": [
                            {
                                "rid": 1005
                            }
                        ],
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
                    "icon": {
                        "instanceID": 26498
                    },
                    "colorTint": {
                        "r": 0.529411792755127,
                        "g": 0.8431373238563538,
                        "b": 0.9647059440612793,
                        "a": 1.0
                    },
                    "name": "Exterior Behaviour",
                    "targetBundleRef": {
                        "instanceID": 27562
                    },
                    "bundleRefGui": "14f8a6b99eef58e458faf05ec5836ea9",
                    "identifierToSet": {
                        "instanceID": 0
                    }
                }
            },
            {
                "rid": 1004,
                "type": {
                    "class": "AssistantWFC",
                    "ns": "ISILab.LBS.Assistants",
                    "asm": "LBS"
                },
                "data": {
                    "visible": true,
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "02a644759487ae249bc3a20d019c8745",
                        "id": "Exterior",
                        "name": "Layer Exterior",
                        "modules": [
                            {
                                "rid": 1001
                            },
                            {
                                "rid": 1002
                            }
                        ],
                        "behaviours": [
                            {
                                "rid": 1003
                            }
                        ],
                        "assistants": [
                            {
                                "rid": 1004
                            }
                        ],
                        "generatorRules": [
                            {
                                "rid": 1005
                            }
                        ],
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
                    "icon": {
                        "instanceID": 25740
                    },
                    "colorTint": {
                        "r": 0.760784387588501,
                        "g": 0.9647059440612793,
                        "b": 0.4431372880935669,
                        "a": 1.0
                    },
                    "name": "Assistant WFC",
                    "overrideValues": false,
                    "targetBundleRef": {
                        "instanceID": 27702
                    }
                }
            },
            {
                "rid": 1005,
                "type": {
                    "class": "ExteriorRuleGenerator",
                    "ns": "ISILab.LBS.Generators",
                    "asm": "LBS"
                },
                "data": {
                    "generator3D": {
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
                        "rules": []
                    }
                }
            },
            {
                "rid": 1006,
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
                "rid": 1007,
                "type": {
                    "class": "TileConnectionsPair",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "tile": {
                        "rid": 1006
                    },
                    "connections": [
                        "Grass",
                        "",
                        "",
                        ""
                    ],
                    "editedByIA": [
                        true,
                        false,
                        false,
                        false
                    ]
                }
            }
        ]
    }
}