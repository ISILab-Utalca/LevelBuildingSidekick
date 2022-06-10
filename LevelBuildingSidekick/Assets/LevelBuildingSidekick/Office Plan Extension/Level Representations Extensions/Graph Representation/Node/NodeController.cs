using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Blueprint;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    public class NodeController : Controller
    {
        public List<NodeController> neighbors;
        public int ID
        {
            get
            {
                return Label.GetHashCode();
            }
        }
        public Vector2Int Position => (Data as NodeData).position;
        public int Radius => (Data as NodeData).Radius;
        public Texture2D Sprite => (Data as NodeData).Sprite;

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

        public NodeController(Data data) : base(data)
        {
            View = new NodeView(this);
            neighbors = new List<NodeController>();
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
            return new Rect(d.position, d.Radius * 2 * Vector2.one);
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
    }
}

