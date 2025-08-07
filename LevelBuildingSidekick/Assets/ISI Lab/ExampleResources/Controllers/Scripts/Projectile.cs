using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Examples
{
    public class Projectile : MonoBehaviour
    {
        public float lifetime = 20f;
        public int damageAmount = 10;

        private void Start()
        {
            Destroy(this.gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);

        }

        private void OnTriggerEnter(Collider other)
        {
            Character character = other.gameObject.GetComponent<Character>();

            if (character != null)
            {
                character.GetDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}