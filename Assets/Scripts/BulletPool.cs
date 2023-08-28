using System.Collections.Generic;
using Bullets;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    private List<Bullet>[] objectPools = { new(), new() };
    public GameObject[] objectsToPool;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Bullet GetBulletFromPool(int poolIndex)
    {
        for (var i = 0; i < objectPools[poolIndex].Count; i++)
        {
            if (!objectPools[poolIndex][i].gameObject.activeInHierarchy)
            {
                return objectPools[poolIndex][i];
            }
        }

        var result = Instantiate(objectsToPool[poolIndex], transform).GetComponent<Bullet>();
        objectPools[poolIndex].Add(result);
        return result;
    } 
}
