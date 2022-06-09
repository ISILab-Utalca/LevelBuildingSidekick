using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Blueprint;

namespace LevelBuildingSidekick.Graph
{
    public class NodeController : Controller
    {
        public List<NodeController> neighbors;
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
        }

        public Vector2Int Height
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return Room.height;
            }
        }

        public Vector2Int Width
        {
            get
            {
                if (Room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return Room.width;
            }
        }

        public Vector2Int Ratio
        {
            get
            {
                if ((Data as NodeData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return Room.aspectRatio;
            }
        }

        public ProportionType proportionType
        {
            get
            {
                if(Room == null)
                {
                    Debug.LogWarning("Room does not Exit");
                    return ProportionType.NONE;
                }
                return Room.proportionType;
            }
        }

        public NodeController(Data data) : base(data)
        {
            View = new NodeView(this);
            neighbors = new List<NodeController>();
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

