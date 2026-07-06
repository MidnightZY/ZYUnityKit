using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
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