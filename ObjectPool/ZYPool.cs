using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZYUnityKit
{
  public class ObjectPool<T>
  {
    // 存放全部对象，并用来查找可用对象。
    private List<ZYPoolContainer<T>> list;

    // 存正在使用的对象。目的：根据归还的对象，快速找到它对应的容器
    private Dictionary<T, ZYPoolContainer<T>> lookup;

    // Func<T>表示一个没有参数、返回值类型是 T 的委托,用作对象池中对象不足时的创建行为函数
    private Func<T> factoryFunc;

    // 用于记录上一次检索终止的位置，下一次直接向后找
    int lastIndex = -1;

    public ObjectPool(Func<T> factoryFunc, int initialSize)
    {
      list = new List<ZYPoolContainer<T>>();
      lookup = new Dictionary<T, ZYPoolContainer<T>>();

      this.factoryFunc = factoryFunc;

      Warm(initialSize);
    }

    private void Warm(int initialSize)
    {
      for (int i = 0; i < initialSize; i++)
      {
        CreateContainer();
      }
    }

    private ZYPoolContainer<T> CreateContainer()
    {
      ZYPoolContainer<T> container = new ZYPoolContainer<T>();

      // 抛出一个“参数为空”的异常
      if (factoryFunc == null)
      {
        throw new ArgumentNullException(nameof(factoryFunc));
      }

      container.Item = factoryFunc(); // 通过创建函数返回新建的对象，并存入对象的Item属性中
      list.Add(container);
      return container;
    }

    public T GetItem()
    {
      ZYPoolContainer<T> container = null;
      for (int i = 0; i < Count; i++)
      {
        // 直接从上一次向后找
        lastIndex = (lastIndex + 1) % Count;
        if (list[lastIndex].Used) continue;
        else
        {
          container = list[lastIndex];
          break;
        }
      }

      // 新建新的加入对象池
      if (container == null)
      {
        container = CreateContainer();
      }

      container.Consume();
      lookup.Add(container.Item, container);
      return container.Item;
    }

    // public void ReleaseItem(object item)
    // {
    //   ReleaseItem((T)item);
    // }

    public void ReleaseItem(T item)
    {
      if (lookup.ContainsKey(item))
      {
        var container = lookup[item];
        container.Release();
        lookup.Remove(item);
      }
      else
      {
        Debug.LogWarning("This object pool does not contain the item provided: " + item);
      }
    }


    public int Count
    {
      get => list.Count;
    }

    public int UsedCount
    {
      get => lookup.Count;
    }

  }
}
