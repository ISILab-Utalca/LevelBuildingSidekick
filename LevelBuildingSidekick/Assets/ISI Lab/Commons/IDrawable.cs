using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Commons
{
    public interface IDrawable
    {
        public Texture2D ToTexture();
    }
}