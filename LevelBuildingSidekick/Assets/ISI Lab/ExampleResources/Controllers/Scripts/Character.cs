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
        public List<GameObject> modelObjects;

        public void GetDamage(float damage)
        {
            life -= damage;
            if (life <= 0)
            {
                Death();
            }
            
            StartCoroutine(DamageEffect(0.1f));
        }

        public void Death()
        {
            Destroy(this.gameObject);
        }

        IEnumerator DamageEffect(float time)
        {
            var materialList = new List<Material>();
            foreach (GameObject modelObject in modelObjects)
            {
                var renderer = modelObject.GetComponent<Renderer>();
                List<Material> materials = new List<Material>(renderer.materials);

                foreach (Material material in materials)
                {
                    materialList.Add(material);
                    material.SetFloat("_BlinkFactor", 1f);
                    material.SetFloat("_GrowFactor", 0.025f);
                }
            }
            yield return new WaitForSeconds(time);
            foreach(Material material in materialList)
            {
                material.SetFloat("_BlinkFactor", 0f);
                material.SetFloat("_GrowFactor", 0.01f);
            }

            /*if (characterRenderer == null) yield return null;
            Debug.Log(characterRenderer + " / " + characterRenderer.materials);
            characterRenderer.material.SetFloat("_BlinkFactor", 1f);
            */
            //The thing that changes the color should go here!
        }
    }
}