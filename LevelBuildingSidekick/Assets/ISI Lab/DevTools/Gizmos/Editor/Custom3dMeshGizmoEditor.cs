using ISI_Lab.LBS.DevTools;
using ISILab.LBS.VisualElements;
using UnityEngine.UIElements;

namespace ISI_Lab.DevTools.Gizmos.Editor
{
    using UnityEngine;
    using UnityEditor;
    using ISI_Lab.LBS.Plugin.MapTools.Generators3D;

    [CustomEditor(typeof(Custom3dMeshGizmo))]
    public class Custom3dMeshGizmoEditor : Editor
    {
        private WorldEditBarView rootVisualElement;
        private bool isVisible = false;
        private Rect popupRect;

        private const float buttonSize = 18;
        private const float yOffset = 64;

        private LBSGenerated lbsComponent;
        
        void OnEnable()
        {
            Custom3dMeshGizmo targetComponent = (Custom3dMeshGizmo)target;
            lbsComponent = targetComponent.GetComponent<LBSGenerated>();
            
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
            if (sceneView.drawGizmos)
            {
                Custom3dMeshGizmo targetComponent = (Custom3dMeshGizmo)target;
                targetComponent.UpdatePosition();
                Vector3 center = targetComponent.worldPosition;
                Vector2 screenPoint = HandleUtility.WorldToGUIPoint(center);

                if (rootVisualElement == null)
                {
                    rootVisualElement = new WorldEditBarView(lbsComponent);
                    
                    UpdatePopupPosition(screenPoint);
                    
                    sceneView.rootVisualElement.Add(rootVisualElement);
                    rootVisualElement.SetFields(lbsComponent.BundleTemp);
                }
                else
                {
                    // Update position
                    UpdatePopupPosition(screenPoint);
                }

            }
            else
            {
                RemoveUI();
            }
        }

        private void UpdatePopupPosition(Vector2 screenPoint)
        {
            rootVisualElement.style.position = Position.Absolute;
            rootVisualElement.style.left = screenPoint.x - rootVisualElement.style.width.value.value/2;
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
