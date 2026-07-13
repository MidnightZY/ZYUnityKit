using UnityEngine;
using System.Collections.Generic;

namespace ZYUnityKit
{
  #region Array
  public static partial class ZYUtils
  {
    /// <summary>
    /// 将二维数组序列化为一维数组。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static T[] Serialize2DArray<T>(T[,] arr)
    {

      int length1 = arr.GetLength(0);
      int length2 = arr.GetLength(1);
      T[] res = new T[length1 * length2];
      for (int i = 0; i < length1; i++)
      {
        for (int j = 0; j < length2; j++)
        {
          res[i * length1 + j] = arr[i, j];
        }
      }
      return res;
    }

    /// <summary>
    /// Deserialize a 1D array into a 2D array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <param name="len1"></param>
    /// <param name="len2"></param>
    /// <returns></returns>
    public static T[,] Deserialize2DArray<T>(T[] arr, int len1, int len2)
    {
      int length = arr.Length;

      T[,] res = new T[len1, len2];
      for (int i = 0; i < length; i++)
      {
        res[i / len1, i % len2] = arr[i];
      }
      return res;
    }

    /// <summary>
    /// Modify the length of an array. Existing data will be preserved.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static T[] ModifyArray<T>(T[] arr, int length)
    {
      T[] res = new T[length];
      for (int i = 0; i < arr.Length; i++)
      {
        if (i >= length)
          break;
        res[i] = arr[i];
      }
      return res;
    }

    /// <summary>
    /// Modify the length of an array. Existing data will be preserved.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static T[,] Modify2DArray<T>(T[,] arr, int width, int height)
    {
      T[,] res = new T[width, height];
      for (int i = 0; i < arr.GetLength(0); i++)
      {
        if (i >= width)
          break;
        for (int j = 0; j < arr.GetLength(1); j++)
        {
          if (j >= height)
            break;
          res[i, j] = arr[i, j];
        }
      }
      return res;
    }

    #endregion

    #region Vector&Rotation
    /// <summary>
    /// Convert an angle to a vector2.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>

    public static Vector3 Angle2Vector(float angle)
    {
      // angle = 0 -> 360
      float angleRad = angle * (Mathf.PI / 180f);
      return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    /// <summary>
    /// Convert an int angle to a vector2.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 Angle2VectorInt(int angle)
    {
      // angle = 0 -> 360
      float angleRad = angle * (Mathf.PI / 180f);
      return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    /// <summary>
    /// Convert a vector2 to an angle (0-360);
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float Vector2AngleFloat(Vector3 dir)
    {
      dir = dir.normalized;
      float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      if (n < 0) n += 360;

      return n;
    }

    /// <summary>
    /// Convert a vector2 (x,z) to an angle (0-360);
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float Vector2AngleFloatXZ(Vector3 dir)
    {
      dir = dir.normalized;
      float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
      if (n < 0) n += 360;

      return n;
    }

    /// <summary>
    /// Convert a vector2 to an angle (0-360);
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static int Vector2Angle(Vector3 dir)
    {
      dir = dir.normalized;
      float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      if (n < 0) n += 360;
      int angle = Mathf.RoundToInt(n);

      return angle;
    }

    /// <summary>
    /// Convert a vector2 to an angle (0-180);
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static int Vector2Angle180(Vector3 dir)
    {
      dir = dir.normalized;
      float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      int angle = Mathf.RoundToInt(n);

      return angle;
    }

    /// <summary>
    /// Rotate a vector.
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="vecRotation"></param>
    /// <returns></returns>
    public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
    {
      return ApplyRotationToVector(vec, Vector2AngleFloat(vecRotation));
    }

    /// <summary>
    /// Rotate a vector.
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
      return Quaternion.Euler(0, 0, angle) * vec;
    }

    /// <summary>
    /// Rotate a vector(x,z).
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
    {
      return Quaternion.Euler(0, angle, 0) * vec;
    }

    #endregion

    #region Dictionary

    /// <summary>
    /// 将一个键值对插入字典。如果键已存在，则更新其值。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static bool InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
      if (dict == null)
      {
        EditorLogError("InsertOrUpdateKeyInDictionary: <Dictionary dict> is null.");
        return false;
      }

      if (dict.ContainsKey(key))
        dict[key] = value;
      else
        dict.Add(key, value);

      return true;
    }

    /// <summary>
    /// 只有在字典中存在该键时才删除该键。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    public static bool RemoveKeyInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
    {
      if (dict == null)
      {
        EditorLogError("RemoveKeyInDictionary: <Dictionary dict> is null.");
        return false;
      }
      if (dict.ContainsKey(key))
      {
        dict.Remove(key);
        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// 只有在键存在时才获取字典中的值。
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue GetValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
    {
      if (dict == null)
      {
        EditorLogError("GetValueInDictionary: <Dictionary dict> is null.");
        return default(TValue);
      }

      if (dict.ContainsKey(key))
        return dict[key];
      else
      {
        EditorLogError("GetValueInDictionary: Key is not present.");
        return default(TValue);
      }
    }

    #endregion
  }


}