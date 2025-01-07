using System;
using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class NotificationMessage : VisualElement
    {
        private Label message;
        private VisualElement icon;

        public class NotificationMessageFactory : UxmlFactory<NotificationMessage, UxmlTraits> { }
        
        public NotificationMessage()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NotificationMessage");
            visualTree.CloneTree(this);
            message = this.Q<Label>("MessageVe");
            icon = this.Q<VisualElement>("IconVe");
            pickingMode = PickingMode.Ignore; 
        }

        public void SetData(string inMessage, LogType logType)
        {
            if (message == null || icon == null)
            {
                Debug.LogError("Missing VE");
                return;
            }
            var setIcon = logType switch
            {
                LogType.Error => Resources.Load<Texture2D>("Icons/Error"),
                LogType.Assert => Resources.Load<Texture2D>("Icons/Assert"),
                LogType.Warning => Resources.Load<Texture2D>("Icons/Warning"),
                LogType.Log => Resources.Load<Texture2D>("Icons/Log"),
                LogType.Exception => Resources.Load<Texture2D>("Icons/Exception"),
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            
            icon.style.backgroundImage = setIcon;
            message.text = inMessage;
            Debug.Log("Data Set");
            
        } 
    } 
}
