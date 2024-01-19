using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Class to manage object pooling: Instead of instantiating and destroying objects, we can just disable and enable them from a list
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    public List<PooledObject> ObjectPools = new List<PooledObject>();

    public GameObject _objectPoolHierarchyHolder;
    public GameObject _enemyPoolHolder;
    public GameObject _enemyBulletPoolHolder;
    public GameObject _bulletPoolHolder;

    public enum PoolType
    {
        Enemy,
        EnemyBullet,
        Bullet,
        None
    }

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        setupHierarchyHolders();
    }

    private void setupHierarchyHolders()
    {
        _objectPoolHierarchyHolder = new GameObject("Pooled Objects");

        _enemyPoolHolder = new GameObject("Enemy Pool");
        _enemyPoolHolder.transform.SetParent(_objectPoolHierarchyHolder.transform);

        _enemyBulletPoolHolder = new GameObject("Enemy Bullet Pool");
        _enemyBulletPoolHolder.transform.SetParent(_objectPoolHierarchyHolder.transform);

        _bulletPoolHolder = new GameObject("Bullet Pool");
        _bulletPoolHolder.transform.SetParent(_objectPoolHierarchyHolder.transform);
    }

    // Spawn an object from the pool, or instantiate a new one if none are available
    public GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        PooledObject currentPool = ObjectPools.Find(pool => pool.name == objectToSpawn.name);
        
        // instantiate pool if it doesn't exist
        if(currentPool == null)
        {
            currentPool = new PooledObject() { name = objectToSpawn.name };
            ObjectPools.Add(currentPool);
        }

        GameObject currentObject = currentPool.InactiveObjects.FirstOrDefault();

        // if there are no inactive objects, instantiate a new one
        if(currentObject == null)
        {
            currentObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            GameObject parentObject = getParentObject(poolType);
            if(parentObject != null)
            {
                currentObject.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            currentObject.transform.position = spawnPosition;
            currentObject.transform.rotation = spawnRotation;
            currentPool.InactiveObjects.Remove(currentObject);
            currentObject.SetActive(true);
        }

        return currentObject;
    }

    // Spawn an object from the pool, or instantiate a new one if none are available. Spawn on parent object
    public GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
    {
        PooledObject currentPool = ObjectPools.Find(pool => pool.name == objectToSpawn.name);

        // instantiate pool if it doesn't exist
        if (currentPool == null)
        {
            currentPool = new PooledObject() { name = objectToSpawn.name };
            ObjectPools.Add(currentPool);
        }

        GameObject currentObject = currentPool.InactiveObjects.FirstOrDefault();

        // if there are no inactive objects, instantiate a new one
        if (currentObject == null)
        {
            currentObject = Instantiate(objectToSpawn, parentTransform);
        }
        else
        {
            currentPool.InactiveObjects.Remove(currentObject);
            currentObject.SetActive(true);
        }

        return currentObject;
    }

    public void DespawnObject(GameObject objectToDespawn)
    {
        string goName = objectToDespawn.name.Substring(0, objectToDespawn.name.Length-7); // remove "(Clone)" from the name
        PooledObject currentPool = ObjectPools.Find(pool => pool.name == goName);

        if(currentPool == null)
        {
            Debug.LogError("Object pool not found for " + objectToDespawn.name);
            return;
        }

        currentPool.InactiveObjects.Add(objectToDespawn);
        objectToDespawn.SetActive(false);
    }

    private GameObject getParentObject(PoolType poolType)
    {
        switch(poolType)
        {
            case PoolType.Enemy:
                return _enemyPoolHolder;
            case PoolType.EnemyBullet:
                return _enemyBulletPoolHolder;
            case PoolType.Bullet:
                return _bulletPoolHolder;
            default:
                return null;
        }
    }

}

public class PooledObject
{
    public string name;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}