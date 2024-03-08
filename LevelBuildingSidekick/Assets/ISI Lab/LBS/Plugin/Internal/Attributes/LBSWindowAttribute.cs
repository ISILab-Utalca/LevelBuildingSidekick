using ISILab.LBS;

namespace ISILab.LBS.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LBSWindowAttribute : LBSAttribute
    {
        private string name;
        public string Name => name;

        public LBSWindowAttribute(string name)
        {
            this.name = name;
        }
    }
}