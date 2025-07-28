using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Macros;
using UnityEngine.UIElements.Experimental;


[UxmlElement]
public partial class LBSStartedSign: VisualElement
{
    private const string URL = "https://isilab-utalca.github.io/isilab-website/documentation/tutorials/lbs/overview/"; 
    
    private VisualTreeAsset signPopup;

    
    public LBSStartedSign()
    {
        signPopup = LBSAssetMacro.LoadAssetByGuid<VisualTreeAsset>("2019cc78f8952b649a6004d15c450b71");
        signPopup.CloneTree(this);
        
        Label linkLabel = this.Query<Label>("Link");
        linkLabel.RegisterCallback<PointerUpEvent>(onHyperlinkClicked);
    }

    private void onHyperlinkClicked(PointerUpEvent _evt)
    {
        Debug.Log("Hyperlink clicked");
        Application.OpenURL(URL);
    }
    
}
