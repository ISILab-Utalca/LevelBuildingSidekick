
using ISILab.LBS.Modules;

namespace ISI_Lab.DevTools.Gizmos.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    [CustomEditor(typeof(Custom3dMeshGizmo))]
    public class Custom3dMeshGizmoEditor : Editor
    {
        private bool isVisible = false;
        private Rect popupRect;

        void OnSceneGUI()
        {
            Custom3dMeshGizmo targetComponent = (Custom3dMeshGizmo)target;
            popupRect = new Rect(10, 10, 150, 100);
            
            
            Vector3 center = targetComponent.transform.position;
            Vector2 screenPoint =  RectTransformUtility.WorldToScreenPoint(Camera.current, center);
            
            Handles.BeginGUI();
            if (GUI.Button(new Rect(screenPoint.x, screenPoint.y, 100, 20), "Toggle Popup"))
            {
                isVisible = !isVisible;
            }

            if (isVisible)
            {
                GUILayout.Window(0, popupRect, DrawPopupWindow, "Popup Window");
            }
            Handles.EndGUI();
        }
        
        void DrawPopupWindow(int windowID)
        {
            GUILayout.Label("This is a popup in the Scene view.");
            if (GUILayout.Button("Close"))
            {
                isVisible = false;
            }

            // Make the window draggable
            GUI.DragWindow();
        }
        
        
    }
}
