using ISI_Lab.LBS.DevTools;
using ISILab.LBS.VisualElements;
using UnityEngine.UIElements;

namespace ISI_Lab.DevTools.Gizmos.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    [CustomEditor(typeof(Custom3dMeshGizmo))]
    public class Custom3dMeshGizmoEditor : Editor
    {
        private VisualElement rootVisualElement;
        private bool isVisible = false;
        private Rect popupRect;


        private const float buttonSize = 18;
        private const float yOffset = 64;
        
        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            RemoveUI();
        }
        
        private void RemoveUI()
        {
            if (rootVisualElement != null)
            {
                rootVisualElement.RemoveFromHierarchy();
                rootVisualElement = null;
            }
        }
        
        void OnSceneGUI(SceneView sceneView)
        {
            Custom3dMeshGizmo targetComponent = (Custom3dMeshGizmo)target;
            Vector3 center = targetComponent.worldPosition;
            Vector2 screenPoint = HandleUtility.WorldToGUIPoint(center);

            if (rootVisualElement == null)
            {
                rootVisualElement = new WorldEditBarView(targetComponent.gameObject);
                sceneView.rootVisualElement.Add(rootVisualElement);
            }

            rootVisualElement.style.position = Position.Absolute;
            // Update position
            rootVisualElement.style.left = screenPoint.x - rootVisualElement.resolvedStyle.width/2;
            rootVisualElement.style.top = screenPoint.y - yOffset;
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
