using ISILab.LBS.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class PickerBase : VisualElement
    {
        private static Button _activeButton;
        private readonly Color _color = LBSSettings.Instance.view.toolkitNormal;
        private readonly Color _selected = LBSSettings.Instance.view.newToolkitSelected;

        /// <summary>
        /// Call when a picker button is clicked in order to set the active button visually
        /// to correspond to a Picker in the editor toolbar
        /// </summary>
        /// <param name="button"></param>
        protected void ActivateButton(Button button)
        {
            if(_activeButton is not null) 
            {
                _activeButton.style.backgroundColor = _color; // deactivate previous
            }
            _activeButton = button;
            _activeButton.style.backgroundColor = _selected; // activate newest
        }
    }
}
