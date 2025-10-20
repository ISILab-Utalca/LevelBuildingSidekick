using System;
using System.Collections.Generic;
using UnityEngine;

namespace ISI_Lab.Commons.Utility
{
    public static class LBSHashUtilities
    {
        public static int CustomListHash<T>(List<T> list)
        {
            int hash = 0;
            foreach (var item in list)
            {
                int h = item.GetHashCode();
                hash = HashCode.Combine(hash, h);
            }
            return hash;
        } 
    }
}
