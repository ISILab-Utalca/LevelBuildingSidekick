using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{

    [UxmlElement]
    public partial class LBSCustomImage : Image
    {
        private VectorImage lbsImage;


        #region PARAMETERS
        
        [UxmlAttribute]
        public VectorImage LBSImage
        {
            get => lbsImage;
            set
            {
                lbsImage = value;
                if (lbsImage != null)
                {
                    this.vectorImage = lbsImage;
                    this.style.display = DisplayStyle.Flex;
                }
                else
                {
                    this.style.display = DisplayStyle.None;
                }
            }
        }
        
        #endregion

        public LBSCustomImage(): base()
        {
            this.AddToClassList("lbs-icon");
        }
    }

}
