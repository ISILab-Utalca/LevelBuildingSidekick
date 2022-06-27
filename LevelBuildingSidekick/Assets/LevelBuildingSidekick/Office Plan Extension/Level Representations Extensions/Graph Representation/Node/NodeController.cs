using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Blueprint;
using Utility;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    public class NodeController : Controller
    {
        public HashSet<NodeController> neighbors;
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
                if ((Data as NodeData == null))
                {
                    return Vector2Int.zero;
                }
                return (Data as NodeData).position;
            }
        }
        public int Radius
        {
            get
            {
                if ((Data as NodeData == null))
                {
                    return 0;
                }
                return (Data as NodeData).radius;
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
                if ((Data as NodeData == null))
                {
                    return null;
                }
                return (Data as NodeData).sprite;
            }
        } 

        public RoomCharacteristics Room
        {
            get
            {
                if ((Data as NodeData).room == null)
                {
                    return new RoomCharacteristics();
                }
                return (Data as NodeData).room;
            }
        }
        public string Label
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return "";
                }
                return Room.label;
            }
            set
            {
                Room.label = value;
            }
        }
        public Vector2Int Height
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.one;
                }
                return Room.height;
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.height = value;
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
                return Room.width;
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.width = value;
            }
        }
        public Vector2Int Ratio
        {
            get
            {
                if ((Data as NodeData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.one;
                }
                return Room.aspectRatio;
            }
            set
            {
                if (value.x < 1 || value.y < 1)
                {
                    return;
                }
                Room.aspectRatio = value;
            }
        }
        public ProportionType ProportionType
        {
            get
            {
                if(Room == null)
                {
                    Debug.LogWarning("Room does not Exit");
                    return ProportionType.RATIO;
                }
                return Room.proportionType;
            }
            set
            {
                if (value == ProportionType.NONE)
                {
                    return;
                }
                Room.proportionType = value;
            }
        }
        public List<GameObject> FloorTiles
        {
            get
            {
                if (Room.floorTiles == null)
                {
                    Room.floorTiles = new List<GameObject>();
                }
                return Room.floorTiles;
            }
        }
        public List<GameObject> WallTiles
        {
            get
            {
                if (Room.wallTiles == null)
                {
                    Room.wallTiles = new List<GameObject>();
                }
                return Room.wallTiles;
            }
        }
        public List<GameObject> DoorTiles
        {
            get
            {
                if (Room.doorTiles == null)
                {
                    Room.doorTiles = new List<GameObject>();
                }
                return Room.doorTiles;
            }
        }
        

        public NodeController(Data data) : base(data)
        {
            View = new NodeView(this);
            neighbors = new HashSet<NodeController>();
            Height = Vector2Int.one;
            Width = Vector2Int.one;
            Ratio = Vector2Int.one;
        }

        public override void LoadData()
        {
            //throw new System.NotImplementedException();
        }

        public override void Update()
        {
        }

        public Rect GetRect()
        {
            NodeData d = Data as NodeData;
            return new Rect(d.position, d.radius * 2 * Vector2.one);
        }

        public void Translate(Vector2 delta)
        {
            NodeData d = Data as NodeData;
            d.position += new Vector2Int((int)delta.x,(int)delta.y);
        }
        public void Translate(Vector2Int delta)
        {
            NodeData d = Data as NodeData;
            d.position += delta;
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
    }
}

