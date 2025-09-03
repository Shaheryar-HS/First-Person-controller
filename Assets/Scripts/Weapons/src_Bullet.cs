using UnityEngine;

public class src_Bullet : MonoBehaviour
{
    [Header("Setting")]
    public float lifeTime;
    private GameObject temp;
    void Start()
    {

    }
    void OnEnable()
    {
        Invoke("DisableObject", lifeTime);
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }
    void OnCollisionEnter(Collision collision)
    {
        // enemy
        // temp = collision.gameObject;
        // temp.SetActive(false); // Example: deactivate the object hit

        // ✅ Check if object has EnemyHealth script
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.Die(); // Apply damage 💥
        }

        // Instead of Destroy(gameObject);
        gameObject.SetActive(false);
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Optional: reset physics
    }

}
