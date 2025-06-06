using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public abstract class NodeEditor : VisualElement
        { 
                public abstract void SetMyData(BaseQuestNodeData data);
        }
}