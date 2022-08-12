using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Schema;
using Utility;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    public class LBSNodeController : Controller
    {
        public HashSet<LBSNodeController> neighbors;
        public HashSet<string> NeighborsIDs
        {
            get
            {
                if((Data as LBSNodeData).room.neighbors == null)
                {
                    (Data as LBSNodeData).room.neighbors = new List<string>();
                }
                return (Data as LBSNodeData).room.neighbors.ToHashSet();
            }
            set
            {
                (Data as LBSNodeData).room.neighbors = value.ToList();
            }
        }

        public int ID
        {
            get
            {
                return GetHashCode();
            }
        }
        public Vector2Int Position
        {
            get
            {
                if ((Data as LBSNodeData == null))
                {
                    return Vector2Int.zero;
                }
                var d = (Data as LBSNodeData);
                return new Vector2Int(d.x,d.y);
                //return (Data as NodeData).position;
            }
            set
            {
                if (value.x < 0) value.x = 0;
                if (value.y < 0) value.y = 0;
                (Data as LBSNodeData).x = value.x;
                (Data as LBSNodeData).y = value.y;
            }
        }
        public int Radius
        {
            get
            {
                if ((Data as LBSNodeData == null))
                {
                    return 0;
                }
                return (Data as LBSNodeData).radius;
            }
            set
            {
                (Data as LBSNodeData).radius = value;
            }
        }
        public Vector2Int Centroid
        {
            get
            {
                return (Position + (Vector2Int.one * Radius));
            }
        } 
        public Texture2D Sprite
        {
            get
            {
                if ((Data as LBSNodeData == null))
                {
                    return null;
                }
                return (Data as LBSNodeData).sprite;
            }
        } 

        public RoomCharacteristics Room
        {
            get
            {
                if ((Data as LBSNodeData).room == null)
                {
                    return new RoomCharacteristics();
                }
                return (Data as LBSNodeData).room;
            }
        }
        public Dictionary<string, HashSet<GameObject>> Prefabs
        {
            get
            {
                return Room.prefabs.ToDictionary(p => p.category, p => p.items.ToHashSet());
            }
            set
            {
                Room.prefabs = value.Select((k) => new ItemCategory(k.Key, k.Value.ToList())).ToList();
            }
        }
        public Func<string, bool> Exist;
        public string Label
        {
            get
            {
                return (Data as LBSNodeData).label;
            }
            set
            {  
                if(Exist?.Invoke(value) == true)
                {
                    //Debug.Log("Does not exist");
                    (Data as LBSNodeData).label = value;

                }
            }
        }
        public Vector2Int HeightRange
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.one;
                }
                return new Vector2Int(Room.minHeight, Room.maxHeight);
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.minHeight = value.x;
                Room.maxHeight = value.y;
            }
        }
        public Vector2Int Width
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.one;
                }
                return new Vector2Int(Room.minWidth, Room.maxWidth);
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.minWidth = value.x;
                Room.maxWidth = value.y;
            }
        }
        public Vector2Int Ratio
        {
            get
            {
                if ((Data as LBSNodeData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.one;
                }
                return new Vector2Int(Room.xAspectRatio, Room.yAspectRatio);
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.xAspectRatio = value.x;
                Room.yAspectRatio = value.y;
            }
        }
        public int MinArea
        {
            get
            {
                if (ProportionType == ProportionType.RATIO)
                {
                    return Ratio.x * Ratio.y;
                }
                else
                {
                    return Width.x * HeightRange.x;
                }
            }
        }
        public ProportionType ProportionType
        {
            get
            {
                if(Room == null)
                {
                    //Debug.LogWarning("Room does not Exit");
                    return ProportionType.RATIO;
                }
                return Room.proportionType;
            }
            set
            {
                Room.proportionType = value;
            }
        }
        public HashSet<string> Tags
        {
            get
            {
                if (Room.tags == null)
                {
                    Room.tags = new HashSet<string>();
                }
                return Room.tags;
            }
            set
            {
                Room.tags = value;
            }
        }
        public string[] ItemCategories
        {
            get
            {
                return Room.prefabCategories;
            }
        }

        public LBSNodeController(Data data) : base(data)
        {
            //View = new LBSNodeView(this);
            neighbors = new HashSet<LBSNodeController>();
        }

        public override void LoadData()
        {
            //throw new System.NotImplementedException();
            if(Room != null && Room.prefabs != null)
            {
                foreach (var cat in Room.prefabs)
                {
                    foreach (var name in cat.ItemNames)
                    {
                        var obj = Utility.DirectoryTools.SearchAssetByName<GameObject>(name);
                        if (obj != null)
                        {
                            cat.items.Add(obj);
                        }
                    }
                }
            }
            
        }

        public override void Update()
        {
        }

        public Rect GetRect()
        {
            return new Rect(Position, Radius * 2 * Vector2.one);
        }

        public void Translate(Vector2 delta)
        {
            Position += new Vector2Int((int)delta.x,(int)delta.y);
        }
        public void Translate(Vector2Int delta)
        {
            Position += delta;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public Vector2Int GetAnchor(Vector2 pos)
        {
            Vector2Int v = Centroid;
            int angle = MathTools.GetAngleD90(Position, pos);
            //Axes are reversed :C
            switch(angle)
            { 
                case 0: v += Vector2Int.left * Radius; break;
                case 90: v += Vector2Int.down * Radius; break;
                case 180: v += Vector2Int.right * Radius; break;
                case 270: v += Vector2Int.up * Radius; break;
                default: Debug.LogWarning("Something wrong with MathTools.GetAngleD90"); break;
            }

            return v;
        }

        public HashSet<GameObject> GetPrefabs(string category)
        {
            if(Room.prefabCategories.Contains(category))
            {
                if(Room.prefabs == null)
                {
                    Room.prefabs = new List<ItemCategory>();
                }
                if(Room.prefabs.Find((r) => r.category == category) == null)
                {
                    Room.prefabs.Add(new ItemCategory(category));
                }
                return (Room.prefabs.Find((r) => r.category == category).items.ToHashSet());
            }
            Debug.LogWarning("No such category of items in Node");
            return null;
        }

        public bool SetPrefabs(string category, HashSet<GameObject> prefabs)
        {
            if(!Room.prefabCategories.Contains(category))
            {
                return false;
            }
            var cat = Room.prefabs.Find(c => c.category == category);
            cat.items = prefabs.ToList();
            return true;
        }

        public bool AddNeighbor(LBSNodeController n)
        {
            if(NeighborsIDs.Contains(n.Label))
            {
                return false;
            }

            (Data as LBSNodeData).room.neighbors.Add(n.Label);
            neighbors.Add(n);
            return true;
        }

    }
}

