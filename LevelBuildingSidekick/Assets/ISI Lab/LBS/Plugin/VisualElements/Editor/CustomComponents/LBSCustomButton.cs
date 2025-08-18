using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    
    [UxmlElement]
    public partial class LBSCustomButton: Button
    {

        public readonly String LBSClassName = "lbs-button";
        
        public LBSCustomButton() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
        }
    }
}

