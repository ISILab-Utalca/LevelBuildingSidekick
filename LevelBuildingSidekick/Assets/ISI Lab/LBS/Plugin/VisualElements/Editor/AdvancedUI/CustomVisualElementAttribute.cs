using ISILab.LBS.Internal;
using System;

namespace ISILab.LBS.Internal
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomVisualElementAttribute : LBSAttribute
    {
        public Type type;

        public CustomVisualElementAttribute(Type t)
        {
            type = t;
        }
    }
}