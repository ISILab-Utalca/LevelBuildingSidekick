using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace ISILab.Examples
{
    public class Character : MonoBehaviour
    {
        public float life = 100;
        //To give it animations
        public GameObject modelObject;

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
            var renderer = modelObject.GetComponent<Renderer>();
            List<Material> materials = new List<Material>(renderer.materials);

            foreach (Material material in materials)
            {
                material.SetFloat("_BlinkFactor", 1f);
            }
            /*if (characterRenderer == null) yield return null;
            Debug.Log(characterRenderer + " / " + characterRenderer.materials);
            characterRenderer.material.SetFloat("_BlinkFactor", 1f);
            */
            //The thing that changes the color should go here!
            yield return new WaitForSeconds(time);
        }
    }
}