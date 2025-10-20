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
                    "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                    "id": "Population",
                    "name": "Layer Population",
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
                        "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                        "id": "Population",
                        "name": "Layer Population",
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
                        },
                        {
                            "rid": 1007
                        },
                        {
                            "rid": 1008
                        },
                        {
                            "rid": 1009
                        }
                    ]
                }
            },
            {
                "rid": 1002,
                "type": {
                    "class": "BundleTileMap",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "BundleTileMap",
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                        "id": "Population",
                        "name": "Layer Population",
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
                    "groups": [
                        {
                            "rid": 1010
                        }
                    ]
                }
            },
            {
                "rid": 1003,
                "type": {
                    "class": "PopulationBehaviour",
                    "ns": "ISILab.LBS.Behaviours",
                    "asm": "LBS"
                },
                "data": {
                    "visible": true,
                    "ownerLayerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                        "id": "Population",
                        "name": "Layer Population",
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
                    "name": "Population Behavior",
                    "tileMap": {
                        "id": "TileMapModule",
                        "ownerLayer": {
                            "visible": true,
                            "blocked": false,
                            "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                            "id": "Population",
                            "name": "Layer Population",
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
                            },
                            {
                                "rid": 1007
                            },
                            {
                                "rid": 1008
                            },
                            {
                                "rid": 1009
                            }
                        ]
                    },
                    "bundleRefGui": "3e607c0f80297b849a6ea0d7f98c73a3",
                    "selectedToSet": {
                        "instanceID": 0
                    },
                    "bundleCollection": {
                        "instanceID": 0
                    },
                    "allFilter": "All",
                    "selectedTypeFilter": ""
                }
            },
            {
                "rid": 1004,
                "type": {
                    "class": "AssistantMapElite",
                    "ns": "ISILab.LBS.Assistants",
                    "asm": "LBS"
                },
                "data": {
                    "visible": true,
                    "ownerLayer": {
                        "visible": true,
                        "blocked": false,
                        "iconGuid": "48f2011efc0f7b2449db9f824c895d9d",
                        "id": "Population",
                        "name": "Layer Population",
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
                        "instanceID": 25970
                    },
                    "colorTint": {
                        "r": 0.760784387588501,
                        "g": 0.9647059440612793,
                        "b": 0.4431372880935669,
                        "a": 1.0
                    },
                    "name": "Map Elite - Genetic Algorithm",
                    "toUpdate": []
                }
            },
            {
                "rid": 1005,
                "type": {
                    "class": "PopulationRuleGenerator",
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
                    "class": "LBSTile",
                    "ns": "LBS.Components.TileMap",
                    "asm": "LBS"
                },
                "data": {
                    "x": 0,
                    "y": -1
                }
            },
            {
                "rid": 1008,
                "type": {
                    "class": "LBSTile",
                    "ns": "LBS.Components.TileMap",
                    "asm": "LBS"
                },
                "data": {
                    "x": 1,
                    "y": 0
                }
            },
            {
                "rid": 1009,
                "type": {
                    "class": "LBSTile",
                    "ns": "LBS.Components.TileMap",
                    "asm": "LBS"
                },
                "data": {
                    "x": 1,
                    "y": -1
                }
            },
            {
                "rid": 1010,
                "type": {
                    "class": "TileBundleGroup",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "tileGroup": [
                        {
                            "x": 0,
                            "y": 0
                        },
                        {
                            "x": 0,
                            "y": -1
                        },
                        {
                            "x": 1,
                            "y": 0
                        },
                        {
                            "x": 1,
                            "y": -1
                        }
                    ],
                    "bData": {
                        "characteristics": [
                            {
                                "rid": 1011
                            },
                            {
                                "rid": 1012
                            }
                        ],
                        "guid": "",
                        "bundleName": "Goblin",
                        "bundle": {
                            "instanceID": 27630
                        }
                    },
                    "rotation": {
                        "x": 1.0,
                        "y": 0.0
                    }
                }
            },
            {
                "rid": 1011,
                "type": {
                    "class": "LBSTagsCharacteristic",
                    "ns": "ISILab.LBS.Characteristics",
                    "asm": "LBS"
                },
                "data": {
                    "owner": {
                        "instanceID": 27630
                    },
                    "tagName": "Goblin",
                    "value": {
                        "instanceID": 27894
                    },
                    "tagGUID": "560e713e6c53d0b4f9ae9a9a5d8d18bf"
                }
            },
            {
                "rid": 1012,
                "type": {
                    "class": "LBSTagsCharacteristic",
                    "ns": "ISILab.LBS.Characteristics",
                    "asm": "LBS"
                },
                "data": {
                    "owner": {
                        "instanceID": 27630
                    },
                    "tagName": "NoBake",
                    "value": {
                        "instanceID": 29044
                    },
                    "tagGUID": "04212421bd21c0040972c027fe29563c"
                }
            }
        ]
    }
}