using UnityEngine;

public class GunFire : MonoBehaviour
{
    public GameObject bulletPrefab; // Assign your bullet prefab
    public Transform firePoint; // Assign the fire point (muzzle)
    public float bulletForce = 20f;

    void Update()
    {
        // if (Input.GetButtonDown("Fire1")) // Default is left mouse click
        // {
        //     Fire();
        // }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
    }
}
