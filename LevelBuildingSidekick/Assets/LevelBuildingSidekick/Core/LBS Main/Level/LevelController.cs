using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System.Linq;
using System;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public class LevelController : Controller
    {

        public HashSet<string> Tags
        {
            get
            {
                if ((Data as LevelData).tags == null)
                {
                    (Data as LevelData).tags = new List<string>();
                }
                return (Data as LevelData).tags.ToHashSet();
            }
            set 
            {
                (Data as LevelData).tags = value.ToList();
            }
        }

        public List<ItemCategory> LevelObjects
        {
            get
            {
                if ((Data as LevelData).levelObjects == null)
                {
                    (Data as LevelData).levelObjects = new List<ItemCategory>();
                }
                return (Data as LevelData).levelObjects;
            }
            set
            {
                (Data as LevelData).levelObjects = value;
            }

        }
        public List<string> ItemCategories
        {
            get
            {
                var categories = LevelObjects.Select((c) => c.category).ToList();
                if (categories == null || categories.Count == 0)
                {
                    categories = new List<string>();
                }
                return categories;
            }
        }
        public Vector2Int LevelSize
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return Vector2Int.zero;
                }
                return (Data as LevelData).size;
            }
            set
            {
                (Data as LevelData).size = value;
            }
        }

        private List<LevelRepresentationController> _Steps;
        public List<LevelRepresentationController> Steps
        {
            get
            {
                if(_Steps == null)
                {
                    _Steps = new List<LevelRepresentationController>();
                }
                return _Steps;
            }
        }

        public string Name
        {
            get
            {
                return (Data as LevelData).levelName;
            }
        }

        public LevelController(Data data) : base(data)
        {
            View = new LevelView(this);
        }

        public override void LoadData()
        {
            LevelData data = Data as LevelData;
            if(data == null)
            {
                return;
            }

            foreach(var cat in LevelObjects)
            {
                foreach(var name in cat.ItemNames)
                {
                    var obj = Utility.DirectoryTools.SearchObjectByName<GameObject>(name);
                    if(obj != null)
                    {
                        cat.items.Add(obj);
                    }
                }
            }

            //Debug.Log("Steps: " + data.steps.Count);
            foreach(Data d in data.representations)
            {
                //Debug.Log("Type: " + d.ControllerType);
                var levelRepresentation = Activator.CreateInstance(d.ControllerType, new object[] { d });
                if(levelRepresentation is LevelRepresentationController)
                {
                    Steps.Add(levelRepresentation as LevelRepresentationController);
                }
            }
            //Debug.Log("StepsC: " + Steps.Count);
            //throw new System.NotImplementedException();
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }

        public HashSet<GameObject> RequestLevelObjects(string category)
        {
            if (LevelObjects.Find((i) => i.category == category) == null)
            {
                //Debug.Log("category created: " + category);
                (Data as LevelData).levelObjects.Add(new ItemCategory(category));
                //LevelObjects.Add(category, new HashSet<GameObject>());
            }
            return LevelObjects.Find((i) => i.category == category).items.ToHashSet();
        }
    }
}


