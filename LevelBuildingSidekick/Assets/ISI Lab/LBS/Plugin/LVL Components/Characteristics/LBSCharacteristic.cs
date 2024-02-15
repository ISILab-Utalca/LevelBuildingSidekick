using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using LBS.Bundles;

namespace ISILab.LBS.Characteristics
{
    /// <summary>
    /// Requiere que las cosas que hereden de el tengan un contructor por defecto sin parametros
    /// </summary>
    [System.Serializable]
    public abstract class LBSCharacteristic : ICloneable
    {
        #region FIELDS
        [HideInInspector, JsonIgnore]
        private Bundle owner;
        #endregion

        #region PROPERTIES
        [HideInInspector, JsonIgnore]
        public Bundle Owner => owner;
        #endregion

        #region CONSTRUCTORS
        public LBSCharacteristic() { }
        #endregion

        #region METHODS
        /// <summary>
        /// esta funcion es para que la characteristic tenga axeso a el bundle que lo posee
        /// asi podemos tener acciones o itenracciones dentro bharacteristics
        /// </summary>
        public void Init(Bundle owner)
        {
            this.owner = owner;
            OnEnable();
        }

        public virtual void OnEnable() { }

        public abstract object Clone();


        public abstract override bool Equals(object obj);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }
}