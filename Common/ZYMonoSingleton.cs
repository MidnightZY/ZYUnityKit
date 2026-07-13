using UnityEngine;

namespace ZYUnityKit
{
  /// <summary>
  /// MonoBehaviour 单例基类。
  /// 用于需要挂载在 GameObject 上、并在场景切换时保持存在的管理类。
  /// 当场景中出现重复实例时，会销毁新的重复对象，只保留第一个实例。
  /// </summary>
  /// <typeparam name="T">继承该基类的具体单例类型。</typeparam>
  public abstract class ZYMonoSingleton<T> : MonoBehaviour where T : ZYMonoSingleton<T>
  {
    private static T instance = null;

    public static T Instance
    {
      get => instance;
    }

    protected virtual void Awake()
    {
      if (instance != null && instance != this)
      {
        Destroy(gameObject);
        return;
      }

      instance = (T)this;
      DontDestroyOnLoad(gameObject);
    }
  }
}
