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
                    "class": "BundleTileMap",
                    "ns": "ISILab.LBS.Modules",
                    "asm": "LBS"
                },
                "data": {
                    "id": "BundleTileMap",
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
                    "groups": [
                        {
                            "rid": 1002
                        }
                    ]
                }
            },
            {
                "rid": 1002,
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
                        }
                    ],
                    "bData": {
                        "characteristics": [
                            {
                                "rid": 1003
                            }
                        ],
                        "guid": "",
                        "bundleName": "data",
                        "bundle": {
                            "instanceID": 0
                        }
                    },
                    "rotation": {
                        "x": 1.0,
                        "y": 0.0
                    }
                }
            },
            {
                "rid": 1003,
                "type": {
                    "class": "LBSTagsCharacteristic",
                    "ns": "ISILab.LBS.Characteristics",
                    "asm": "LBS"
                },
                "data": {
                    "owner": {
                        "instanceID": 27548
                    },
                    "tagName": "Furniture",
                    "value": {
                        "instanceID": 27910
                    },
                    "tagGUID": "f34ea2bda3e03b44fad1047a0050c860"
                }
            }
        ]
    }
}