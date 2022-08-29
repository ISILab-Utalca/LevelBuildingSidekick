using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Transformers
{
    public abstract class Transformer<T,U>
        where T : LBSRepesentationData
        where U : LBSRepesentationData
    {
        public abstract U Transform(T representation);
    }

}

