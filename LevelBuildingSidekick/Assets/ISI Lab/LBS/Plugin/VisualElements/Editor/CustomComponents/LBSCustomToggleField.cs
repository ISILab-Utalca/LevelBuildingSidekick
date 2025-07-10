using System.Linq;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomToggleField: Toggle
    {
        public LBSCustomToggleField(): this("CustomToggleField"){}

        public LBSCustomToggleField(string _label) : base(_label)
        {
            
        }
    }
}
