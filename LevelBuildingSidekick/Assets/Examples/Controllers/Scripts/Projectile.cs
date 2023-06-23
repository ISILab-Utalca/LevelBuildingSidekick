using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Character character = collision.gameObject.GetComponent<Character>();

        if (character != null)
        {
            character.GetDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}
