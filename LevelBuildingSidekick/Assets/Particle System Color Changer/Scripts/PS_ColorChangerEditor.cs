namespace TheDeveloper.ColorChanger
{
    using UnityEngine;
    using UnityEditor;

    /**
     * Editor class for PS_ColorChanger
     * 
     * Made by: The Developer
     * YouTube Channel: https://www.youtube.com/channel/UCwO0k5dccZrTW6-GmJsiFrg
     * Website: https://thedevelopers.tech
     */
    [CustomEditor(typeof(PS_ColorChanger))]
    public class PS_ColorChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Change Color"))
                ((PS_ColorChanger)target).ChangeColor();
            if (GUILayout.Button("Swap \"Current\" with \"New\" colors"))
                ((PS_ColorChanger)target).SwapCurrentWithNewColors();
        }
    }
}