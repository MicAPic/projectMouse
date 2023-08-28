using System.Collections.Generic;
using Bullets;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    public Queue<Bullet>[] ObjectPools = new[] { new Queue<Bullet>(), new Queue<Bullet>() };
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
        if (!ObjectPools[poolIndex].TryDequeue(out var result))
        {
            result = Instantiate(objectsToPool[poolIndex], transform).GetComponent<Bullet>();
        }

        return result;
    } 
}
