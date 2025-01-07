using System.Linq;
using ISILab.Commons.Utility.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class NotifierViewer : VisualElement
    {
        private ListView messageContainer;
        
        // max size of messages on the container before force remove
        private int MaxCount = 5; 
        
        public class NotifierViewerFactory : UxmlFactory<NotifierViewer, UxmlTraits> { }
        
        public NotifierViewer()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NotifierViewer");
            if (visualTree == null)
            {
                Debug.LogError("Could not load NotifierViewer.uxml. Check the file path.");
                return;
            }
            //visualTree.CloneTree(this);

            messageContainer = this.Q<ListView>("MessageContainer");
            if (messageContainer == null)
            {
                Debug.LogError("MessageContainer not found in the UXML.");
                return;
            }

            messageContainer.style.flexDirection = FlexDirection.ColumnReverse;

            this.pickingMode = PickingMode.Ignore; // Ensure you want this behavior
            this.style.position = Position.Absolute;
            this.style.top = 0;
            this.style.left = 0;
            this.style.right = 0;
            this.style.bottom = 0;
        }

        
        public void SendNotification(string message, LogType logType, int duration)
        {
            messageContainer = this.Q<ListView>("MessageContainer");
            var newMessage = new NotificationMessage();
            newMessage.SetData(message, logType);
            messageContainer.hierarchy.Add(newMessage);
            
            StartFadeOut(newMessage, duration);
            OnNotificationVisualUpdate();
        }
        
        public void OnNotificationVisualUpdate()
        {
            if (messageContainer.hierarchy.childCount == 0) return;
            if (messageContainer.hierarchy.childCount > MaxCount) messageContainer.hierarchy.RemoveAt(0);
            
            
            var evenOpacityDifference = 1.0f / messageContainer.Children().Count(); 
            int index = 0;
            // Lower opacity of older messages
            foreach (var child in messageContainer.Children())
            {
                if(index == messageContainer.Children().Count()) break; // not on the new one
                var remainingOpacity = 1.0f - (evenOpacityDifference * index);
                remainingOpacity = Mathf.Clamp01(remainingOpacity);
                child.style.opacity = remainingOpacity;
                index++;
            }
            
        }

        private void StartFadeOut(NotificationMessage message, int duration)
        {
            var initialOpacity = message.style.opacity.value;
        }
        
    }
}
  

