using UnityEngine;
using System.IO;
using ZYUnityKit;

public class ArchiveManager : ZYSingleton<ArchiveManager>
{
  private const string DEFAULT_FILE_NAME = "Save.json";
  private static string GetSavePath(string fileName = DEFAULT_FILE_NAME)
  {
    return Path.Combine(Application.persistentDataPath, fileName);
  }

  public static void Save<T>(T targetSave, string fileName = DEFAULT_FILE_NAME) where T : class
  {
    string path = GetSavePath(fileName);
    string json = JsonUtility.ToJson(targetSave, true);
    File.WriteAllText(path, json);
  }

  public static T Load<T>(string fileName = DEFAULT_FILE_NAME)
  {
    string path = GetSavePath(fileName);

    if (!File.Exists(path))
    {
      Debug.LogWarning($"Save file not found: {path}");
      return default(T);
    }

    string json = File.ReadAllText(path);
    return JsonUtility.FromJson<T>(json);
  }

  public static bool Exists(string fileName = DEFAULT_FILE_NAME)
  {
    return File.Exists(GetSavePath(fileName));
  }

  public static void Delete(string fileName = DEFAULT_FILE_NAME)
  {
    string path = GetSavePath(fileName);

    if (File.Exists(path))
    {
      File.Delete(path);
    }
  }
}

