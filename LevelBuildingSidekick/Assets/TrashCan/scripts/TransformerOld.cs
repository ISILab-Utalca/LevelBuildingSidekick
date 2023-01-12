using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Transformers
{
    public abstract class TransformerOld<T,U>
        where T : LBSRepresentationData
        where U : LBSRepresentationData
    {
        public abstract U Transform(T representation);
    }

}

