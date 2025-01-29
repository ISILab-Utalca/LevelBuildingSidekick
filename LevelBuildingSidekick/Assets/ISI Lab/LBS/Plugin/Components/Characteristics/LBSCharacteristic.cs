using System;
using System.Collections;
using System.Collections.Generic;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    /// <summary>
    /// Requiere que las cosas que hereden de el tengan un contructor por defecto sin parametros
    /// </summary>
    [System.Serializable]
    public abstract class LBSCharacteristic : ICloneable
    {
        #region FIELDS
        [SerializeReference, SerializeField]
        private Bundle owner;
        #endregion

        #region PROPERTIES
        [JsonIgnore, HideInInspector]
        public Bundle Owner
        {
            get => owner; 
            set => owner = value;
        }
        #endregion

        #region CONSTRUCTORS
        [SerializeField]
        public LBSCharacteristic() {   }
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

        public virtual void OnEnable() {  }

        public abstract object Clone();


        public abstract override bool Equals(object obj);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }
}