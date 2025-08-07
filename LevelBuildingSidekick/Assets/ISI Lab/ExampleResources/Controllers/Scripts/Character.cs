using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Examples
{
    public class Character : MonoBehaviour
    {
        public float life = 100;

        public void GetDamage(float damage)
        {
            life -= damage;
            if (life <= 0)
            {
                Death();
            }
            
            StartCoroutine(DamageEffect(0.25f));
        }

        public void Death()
        {
            Destroy(this.gameObject);
        }

        IEnumerator DamageEffect(float time)
        {
            //The thing that changes the color should go here!
            yield return new WaitForSeconds(time);
        }
    }
}