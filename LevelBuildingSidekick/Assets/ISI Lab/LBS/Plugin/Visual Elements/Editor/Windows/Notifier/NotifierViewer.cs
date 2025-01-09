using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISILab.Commons.Utility.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class NotifierViewer : VisualElement
    {
        private ListView messageContainer;
        
        // max size of messages on the container before force remove
        private int maxCount = 20;
        private bool notificationOn = true;
        private ScrollView scrollView;
        private VectorImage iconNotificationsOn; 
        private VectorImage iconNotificationsOff;
        private static int fadeTime = 5;
        public class NotifierViewerFactory : UxmlFactory<NotifierViewer, UxmlTraits> { }
        
        public NotifierViewer()
        {
            // to overlap other visual elements
            // Set up the container styles
            style.position = Position.Absolute;
            style.bottom = 0; // Anchor to the bottom
            style.left = 0;   // Anchor to the left
            style.alignContent = Align.FlexStart; // Align content at the start
            style.justifyContent = Justify.FlexStart; // Ensure content alignment starts at the bottom
            style.overflow = Overflow.Visible; // Allow it
            
            iconNotificationsOn = Resources.Load<VectorImage>("Icons/Vectorial/Icon=Notification");
            iconNotificationsOff = Resources.Load<VectorImage>("Icons/Vectorial/Icon=MuteNotification");
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
            scrollView = messageContainer.hierarchy.Children().OfType<ScrollView>().Single();
            
            // disable as a clickable 
            messageContainer.pickingMode = PickingMode.Ignore;
            scrollView.contentViewport.pickingMode = PickingMode.Ignore;
            scrollView.pickingMode = PickingMode.Ignore;
            
            scrollView.contentViewport.style.justifyContent = Justify.FlexStart;

            // Make sure the content does not grow; list with fixed height size
            scrollView.contentViewport.style.flexGrow = 1f;

            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            scrollView.contentViewport.style.flexDirection = FlexDirection.Row;

            scrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            
        }
        
        private NotificationMessage[] GetChildren()
        {
            List<NotificationMessage> messages = new List<NotificationMessage>();
            var veChildren = scrollView.Children().ToArray();
            foreach (var veChild in veChildren)
            {
                if(veChild is NotificationMessage message) messages.Add(message);
            }
            
            return messages.ToArray();
        }
        
        public void SendNotification(string message, LogType logType, int duration)
        {
            
            SetContainer();
            var newMessage = new NotificationMessage();
            newMessage.SetData(message, logType);
            scrollView.Add(newMessage);
            
            Lifetime(newMessage, duration);

        }
        
        private async void Lifetime(NotificationMessage element, float duration)
        {
            // Ensure the duration is valid
            if (duration > 0)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew(); 
                while (stopwatch.Elapsed.TotalSeconds < duration)
                {
                    await Task.Yield();
                }
                stopwatch.Stop();
            }
            FadeOut(element);
        }

        private async void FadeOut(NotificationMessage element)
        {
            // Ensure the duration is valid
            if (fadeTime > 0)
            {
                float startOpacity = element.resolvedStyle.opacity;
                float elapsed = 0f;

                while (elapsed < fadeTime)
                {
                    await Task.Yield(); 
                    elapsed += Time.deltaTime;
                    float t = elapsed / fadeTime; 
                    element.style.opacity = Mathf.Lerp(startOpacity, 0f, t);
                }
            }
            
            element.style.opacity = 0f;
            scrollView.Remove(element);
        }

        public void ClearNotifications()
        {
            scrollView.Clear();
        }
        
        public void NotificationFlipFlop(VisualElement button)
        {
            notificationOn = !notificationOn;
            if (button == null) return;
            var tButton = button as ToolbarButton;
            if (tButton == null) return;
            tButton.contentContainer.style.backgroundImage = notificationOn ? new StyleBackground(iconNotificationsOn) : new StyleBackground(iconNotificationsOff);
        }
        
        public void SetButtons(VisualElement cleanButton, VisualElement disableNotificationButton)
        {
            cleanButton.RegisterCallback<ClickEvent>(vt => ClearNotifications());
            disableNotificationButton.RegisterCallback<ClickEvent>(vt => NotificationFlipFlop(disableNotificationButton));
        }

    }
}
  

