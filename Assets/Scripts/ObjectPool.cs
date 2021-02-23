using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
    [SerializeField]
    private string name;
    [SerializeField]
    private List<GameObject> pooledObjects = new List<GameObject>();
    [Header("Prefab")]
    [SerializeField]
    private GameObject objectToPool;
    #region Properties
    public string Name { get => name; }
    public List<GameObject> PooledObjects { get => pooledObjects; }
    public GameObject ObjectToPool { get => objectToPool; }
    #endregion
    #region Constructor
    public ObjectPool()
    {

    }
    public ObjectPool(string name)
    {
        this.name = name;
    }
    public ObjectPool(string name, GameObject objectToPool)
    {
        this.name = name;
        this.objectToPool = objectToPool;
        pooledObjects = null;
    }
    public ObjectPool(string name, List<GameObject> pooledObjects, GameObject objectToPool)
    {
        this.name = name;
        this.objectToPool = objectToPool;
        this.pooledObjects = pooledObjects;
    }
    #endregion
    public GameObject CreatePooledObject()
    {
        GameObject pooledObject = GameManager.Instance.CreateObject(objectToPool);
        pooledObjects.Add(pooledObject);
        return pooledObject;
    }
    public GameObject GetPooledObject()
    {
        foreach(GameObject pooledObject in pooledObjects)
        {
            if(pooledObject != null && !pooledObject.activeInHierarchy)
            {
                return pooledObject;
            }
        }
        return null;
    }
}
