using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // to overlap other visual elements
            pickingMode = PickingMode.Ignore; 
            style.position = Position.Absolute;
            style.top = 0;
            style.left = 0;
            style.right = 0;
            style.bottom = 0;
        }
        
        private void SetContainer()
        {
            if (messageContainer != null) return;
            
            messageContainer = this.Q<ListView>("MessageContainer");
            if (messageContainer == null)
            {
                Debug.LogError("MessageContainer not found in the UXML.");
                return;
            }
            var scrollView = messageContainer.hierarchy.Children().OfType<ScrollView>().Single();
            
            scrollView.contentViewport.style.justifyContent = Justify.FlexEnd;

            // Make sure the content does not grow; we want the list to have a fixed size in height
            scrollView.contentViewport.style.flexGrow = 0f;

            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            messageContainer.style.flexDirection = FlexDirection.ColumnReverse;
            scrollView.contentViewport.style.flexDirection = FlexDirection.ColumnReverse;
            scrollView.contentContainer.style.flexDirection = FlexDirection.ColumnReverse;
            
            // flip because listview only adds content from top to bottom. Design calls for the opposite
            scrollView.style.scale = new Vector2(1, -1); 
            
        }
        
        private NotificationMessage[] GetChildren()
        {
            List<NotificationMessage> messages = new List<NotificationMessage>();
            var veChildren = messageContainer.hierarchy.Children().OfType<ScrollView>().Single().Children().ToArray();
            foreach (var veChild in veChildren)
            {
                if(veChild is NotificationMessage message) messages.Add(message);
            }
            
            return messages.ToArray();
        }
        
        private NotificationMessage[] GetNonWhiteChildren()
        {
            List<NotificationMessage> messages = new List<NotificationMessage>();
            var veChildren = messageContainer.hierarchy.Children().OfType<ScrollView>().Single().Children().ToArray();
            foreach (var veChild in veChildren)
            {
                if(veChild is NotificationMessage message && !message.WhiteSpace) messages.Add(message);
            }
            
            return messages.ToArray();
        }
        
        public void SendNotification(string message, LogType logType, int duration)
        {
            SetContainer();
            var newMessage = new NotificationMessage();
            message = GetNonWhiteChildren().Length.ToString();
            newMessage.SetData(message, logType);
            messageContainer.hierarchy.Children().OfType<ScrollView>().Single().Add(newMessage);
            OnNotificationVisualUpdate();
            
            Lifetime(newMessage, duration);

        }
        
        public void OnNotificationVisualUpdate()
        {
            var _children = GetNonWhiteChildren();
            if (_children == null || !_children.Any()) return;

            NotificationMessage[] reOrdered = GetNonWhiteChildren();
            var _scrollView = messageContainer.hierarchy.Children().OfType<ScrollView>().Single();
            _scrollView.Clear();

            
            // Add white space for empty spaces
            var whites = MaxCount - reOrdered.Length;
            for (int i = 0; i < whites; i++)
            {
                var whiteMessage = new NotificationMessage();
                whiteMessage.WhiteSpace = true;
                whiteMessage.style.opacity = 0f;
                _scrollView.Add(whiteMessage);
            }
            
            // Re-add visual elements
            foreach (var child in reOrdered) _scrollView.Add(child);
            
            // Remove overboard
            if (GetChildren().Count() > MaxCount)
            {
                for (int i = MaxCount; i < GetChildren().Count(); i++)
                {
                    _scrollView.RemoveAt(i);
                }
            }
        }

        private async void Lifetime(NotificationMessage element, float duration)
        {
            // Ensure the duration is valid
            if (duration > 0)
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    await Task.Yield(); 
                    elapsed += Time.deltaTime;
                }
            }
            FadeOut(element, duration);
        }

        private async void FadeOut(NotificationMessage element, float duration)
        {
            // Ensure the duration is valid
            if (duration > 0)
            {
                float startOpacity = element.resolvedStyle.opacity;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    await Task.Yield(); 
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration; 
                    element.style.opacity = Mathf.Lerp(startOpacity, 0f, t);
                }
            }
            
            element.style.opacity = 0f;
            element.WhiteSpace = true;
            OnNotificationVisualUpdate();
        }
    }
}
  

