
namespace ZYUnityKit
{
  /// <summary>
  /// 不依赖 Unity 场景和组件的普通 C# 单例基类。
  /// 适用于配置、数据、存档、事件分发等纯逻辑管理类。
  /// </summary>
  /// <typeparam name="T">继承该基类的具体单例类型。</typeparam>
  public class ZYSingleton<T> where T : class, new() // 必须为引用类型，且有无参构造函数
  {
    protected ZYSingleton() { }

    private static T _instance = null;

    public static T Instance => _instance ?? (_instance = new T());

    public static void Clear()
    {
      _instance = null;
    }

  }
}

