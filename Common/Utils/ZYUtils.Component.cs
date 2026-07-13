using UnityEngine;

namespace ZYUnityKit
{
  /// <summary>
  /// 工具类，提供了一些实用方法。
  /// </summary>
  public static partial class ZYUtils
  {
    /// <summary>
    /// 尝试从指定的 GameObject 上获取指定类型的组件。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetComponentOrNull<T>(GameObject go) where T : Component
    {
      return go.TryGetComponent(out T component) ? component : null;
    }


    /// <summary>
    /// Release object into an object pool.
    /// </summary>
    /// <param name="go"></param>
    public static void ReleaseObject(GameObject go)
    {
      ZYPoolManager.ReleaseObject(go);
    }



  }
}