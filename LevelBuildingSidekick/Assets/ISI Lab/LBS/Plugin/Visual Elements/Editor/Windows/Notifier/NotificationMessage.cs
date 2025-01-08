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
            
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
        }

        /**
         * Currently only unique icons for LogTypes:
         * -Error
         * -Warning
         * -Log
         */
        public void SetData(string inMessage, LogType logType)
        {
            if (message == null || icon == null)
            {
                Debug.LogError("Missing VE");
                return;
            }
            VectorImage setIcon = logType switch
            {
                LogType.Error => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Error"),
                LogType.Assert => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Log"),
                LogType.Warning => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Warning"),
                LogType.Log => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Log"),
                LogType.Exception => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Error"),
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            
            Color setColor = logType switch
            {
                LogType.Error => new Color(1, 0.2f, 0.2f, 1),
                LogType.Assert => new Color(1, 0.2f, 0.2f, 1),
                LogType.Warning => new Color(1, 0.8f, 0.1f, 1),
                LogType.Log => new Color(1, 1, 1, 1),
                LogType.Exception => new Color(1, 1, 1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            
            icon.style.backgroundImage = new StyleBackground(setIcon);
            icon.style.unityBackgroundImageTintColor = setColor;
            message.text = inMessage;
        } 
    } 
}
