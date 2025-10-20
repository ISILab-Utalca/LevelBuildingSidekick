using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Examples
{
    public class ShootingController : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletSpeed = 18f;

        private void Update()
        {
        }

        public void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Projectile>().damageAmount = 15;
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}