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
        private bool whiteSpace;
        public class NotificationMessageFactory : UxmlFactory<NotificationMessage, UxmlTraits> { }
        
        public NotificationMessage()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NotificationMessage");
            visualTree.CloneTree(this);
            message = this.Q<Label>("MessageVe");
            icon = this.Q<VisualElement>("IconVe");
            pickingMode = PickingMode.Ignore;
            style.scale = new Vector2(1f, -1f); // flip because notification viewer is flipped on Y
        }

        public bool WhiteSpace
        {
            get => whiteSpace;
            set => whiteSpace = value;
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
            var setIcon = logType switch
            {
                LogType.Error => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Error"),
                LogType.Assert => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Log"),
                LogType.Warning => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Warning"),
                LogType.Log => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Log"),
                LogType.Exception => Resources.Load<VectorImage>("Icons/Vectorial/Icon=Error"),
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
            
            icon.style.backgroundImage = new StyleBackground(setIcon);
            message.text = inMessage;
        } 
    } 
}
