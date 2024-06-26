using ObjectPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<PoolingType, Pool<PoolableMono>> _poolDictionary
        = new Dictionary<PoolingType, Pool<PoolableMono>>();

    public PoolList poolListSO;

    private void Awake()
    {
        foreach (PoolingItemSO item in poolListSO.GetList())
        {
            CreatePool(item);
        }

        PoolManager[] poolManagers = FindObjectsOfType<PoolManager>();
        if (poolManagers.Length > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void CreatePool(PoolingItemSO item)
    {
        Pool<PoolableMono> pool = new Pool<PoolableMono>(
            item.prefab, item.prefab.type, transform, item.poolCount);
        _poolDictionary.Add(item.prefab.type, pool);
    }

    public PoolableMono Pop(PoolingType type)
    {
        if (_poolDictionary.ContainsKey(type) == false)
        {
            Debug.LogError($"Prefab does not exist on pool : {type.ToString()}");
            return null;
        }

        PoolableMono item = _poolDictionary[type].Pop();
        item.ResetItem();
        return item;
    }

    public void Push(PoolableMono obj, bool resetParent = false)
    {
        if (resetParent)
            obj.transform.parent = transform;
        _poolDictionary[obj.type].Push(obj);
    }
}