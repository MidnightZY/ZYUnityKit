using System.Collections.Generic;
using UnityEngine;
using System;


namespace ZYUnityKit
{
  [AddComponentMenu("ZYTools/ObjectPoolManager")]
  public class ZYPoolManager : ZYMonoSingleton<ZYPoolManager>
  {
    // public Transform root;

    // 控制是否打印对象池使用状态
    public bool logStatus;
    private bool dirty = false;

    // prefab及其对应的对象池
    private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;

    // clone实例来自哪个对象池.用来回收时判断这个对象应该还给哪个池子
    private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

    protected override void Awake()
    {
      base.Awake();

      prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
      instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
    }

    private void Update()
    {
      if (logStatus && dirty)
      {
        dirty = false;
        PrintStatus();
      }
    }

    private void warmPool(GameObject prefab, int size, Transform parent = null)
    {
      if (prefabLookup.ContainsKey(prefab))
      {
        throw new Exception("Pool for prefab " + prefab.name + " has already been created");
      }

      var pool = new ObjectPool<GameObject>(() =>
       { return InstantiatePrefab(prefab, parent); }, size);
      prefabLookup[prefab] = pool;

      dirty = true;
    }

    private GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
      if (prefab == null) return null;
      if (!prefabLookup.ContainsKey(prefab)) warmPool(prefab, 1);

      var pool = prefabLookup[prefab];
      var clone = pool.GetItem();
      if (clone == null)
        return null;
      clone.transform.position = position;
      clone.transform.rotation = rotation;
      clone.SetActive(true);

      // 记录下clone的属于哪个池子
      instanceLookup[clone] = pool;
      dirty = true;
      return clone;
    }

    private void releaseObject(GameObject clone)
    {
      if (clone == null) return;

      if (instanceLookup.ContainsKey(clone))
      {
        clone.SetActive(false);
        instanceLookup[clone].ReleaseItem(clone);
        instanceLookup.Remove(clone);
        dirty = true;
      }
      else
      {
        Debug.LogWarning("No pool contains the object: " + clone.name);
      }
    }

    private GameObject InstantiatePrefab(GameObject prefab, Transform parent)
    {
      var go = GameObject.Instantiate(prefab, parent) as GameObject;
      // if (root != null) go.transform.SetParent(root);
      go.SetActive(false);
      return go;
    }

    public void PrintStatus()
    {
      foreach (var keyVal in prefabLookup)
      {
        Debug.LogWarning(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
            keyVal.Value.UsedCount, keyVal.Value.Count));
      }
    }

    // #region Static API

    /// <summary>
    /// 预先创建一个池子
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="size"></param>
    /// <param name="parent"></param>
    public static void WarmPool(GameObject prefab, int size, Transform parent = null)
    {
      Instance.warmPool(prefab, size, parent);
    }

    /// <summary>
    /// 从池子中找一个并进行实例化 Instantiate a prefab from the pool
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject SpawnPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
      return Instance.spawnObject(prefab, position, rotation);
    }
    public static GameObject SpawnPool(GameObject prefab)
    {
      return Instance.spawnObject(prefab, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 通过实例向池子中释放
    /// </summary>
    /// <param name="clone"></param>
    public static void ReleaseObject(GameObject clone)
    {
      Instance.releaseObject(clone);
    }
  }
}

