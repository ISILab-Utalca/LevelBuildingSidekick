using System;
using System.Collections.Generic;
using UnityEngine;

public class LBSInventory : MonoBehaviour
{
        [SerializeField]
        private Dictionary<Type, int> _inventory = new();

        /// <summary>
        /// Returns the amount of a given object type, user after HasType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetTypeAmount(Type type)
        { 
                return _inventory.GetValueOrDefault(type, 0);
        }
        
        /// <summary>
        /// Returns true if a type is in the inventory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasType(Type type)
        {
                return _inventory.ContainsKey(type);
        }

        /// <summary>
        /// Call to add a type of object into the invetory by n amount
        /// </summary>
        /// <param name="receiveType"></param>
        /// <param name="amount"></param>
        public void AddItems(Type receiveType, int amount)
        {
                _inventory.Add(receiveType, amount);
        }
}