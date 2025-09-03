using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public GameObject objectToPool;
    public int poolSize = 20;

    private List<GameObject> pool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform); // Optional: Set parent for organization
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Optional: Expand pool if needed
        return null;
    }
}
