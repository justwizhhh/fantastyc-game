using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // ------------------------------
    //
    //   An addon class for objects that need to access/create a lot of additional objects with them (e.g projectiles)
    //
    //   Created: 06/07/2024
    //
    // ------------------------------

    public GameObject ObjectPrefab;
    public int ObjectCount;

    public List<GameObject> PooledObjects = new List<GameObject>();
    public int SpawnedObjectCount;

    private int spawnOrder;

    private void Awake()
    {
        CreateAllObjects();
    }

    public void CreateAllObjects()
    {
        for (int i = 0; i < ObjectCount; i++)
        {
            GameObject newObj = Instantiate(ObjectPrefab);

            newObj.SetActive(false);
            newObj.transform.SetParent(transform);
            PooledObjects.Add(newObj);
        }
    }

    public GameObject SpawnObject(Vector2 position, Quaternion rotation)
    {
        // Creates objects one-by-one based on their order in the PooledObjects list
        // If we run out of objects in the pool, we re-use the first object in the list (the one most likely to be far away)

        GameObject newObj = PooledObjects[spawnOrder];

        newObj.SetActive(true);
        newObj.transform.SetParent(null);
        newObj.transform.position = position;
        newObj.transform.rotation = rotation;

        if (spawnOrder >= PooledObjects.Count - 1)
        {
            spawnOrder = 0;
        }
        else
        {
            spawnOrder++;
        }

        return newObj;
    }

    public void ClearAllObjects()
    {
        foreach (GameObject obj in PooledObjects)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            spawnOrder = 0;
        }
    }

    private void Update()
    {
        // We keep track of the number of objects that have been spawned in and are active here
        SpawnedObjectCount = PooledObjects.FindAll(x => x.activeInHierarchy == true).Count;
    }
}
