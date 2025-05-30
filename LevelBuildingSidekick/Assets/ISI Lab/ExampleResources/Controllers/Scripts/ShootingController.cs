using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Examples
{
    public class ShootingController : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletSpeed = 10f;

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}