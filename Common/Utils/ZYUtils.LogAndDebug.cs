using System.Collections.Generic;
using UnityEngine;



namespace ZYUnityKit
{
  public static partial class ZYUtils
  {
    private static int[] lastFrameCounts = new int[3]; // 最后帧计数,用于辅助调试频率问题

    #region Editor Log
    /// <summary>
    /// 将普通消息记录到控制台。
    /// </summary>
    /// <param name="message"></param>
    /// <param name="detailed"></param>
    public static void EditorLogNormal(object message, bool detailed = false)
    {
      if (detailed)
        Debug.Log($"<{message} | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[0]}>");
      else
        Debug.Log($"<{message} | Frame: {Time.frameCount}>");
      lastFrameCounts[0] = Time.frameCount;
    }

    /// <summary>
    /// 将警告消息记录到控制台。
    /// </summary>
    /// <param name="message"></param>
    /// <param name="detailed"></param>
    public static void EditorLogWarning(object message, bool detailed = false)
    {
      if (detailed)
        Debug.LogWarning($"<<color=#B9902A>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[1]}>");
      else
        Debug.LogWarning($"<<color=#B9902A>{message}</color> | Frame: {Time.frameCount}>");
      lastFrameCounts[1] = Time.frameCount;
    }

    /// <summary>
    /// 将错误信息记录到控制台。
    /// </summary>
    /// <param name="message"></param>
    /// <param name="detailed"></param>
    public static void EditorLogError(object message, bool detailed = false)
    {
      if (detailed)
        Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[2]}>");
      else
        Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
      lastFrameCounts[2] = Time.frameCount;
    }

    #endregion

    #region PrintData
    public static void PrintArray<T>(T[] array)
    {
      if (array == null)
      {
        EditorLogWarning("PrintArray: array is null.");
        return;
      }

      foreach (T item in array)
      {
        EditorLogNormal(item);
      }
    }

    public static void PrintList<T>(List<T> list)
    {
      if (list == null)
      {
        EditorLogWarning("PrintList: list is null.");
        return;
      }

      foreach (T item in list)
      {
        EditorLogNormal(item);
      }

    }
    public static void PrintDict<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
      if (dict == null)
      {
        EditorLogWarning("PrintDict: dict is null.");
        return;
      }

      foreach (TKey key in dict.Keys)
      {
        EditorLogNormal($"key:{key}->value:{dict[key]}");
      }

    }

    #endregion

    #region Debug Draw

    /// <summary>
    /// 封装Debug.DrawLine()方法
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <param name="duration"></param>
    /// <param name="color"></param>
    public static void DebugDrawLine(Vector3 origin, Vector3 direction, float maxDistance, float duration, Color color)
    {
      Debug.DrawLine(origin, origin + direction * maxDistance, color, duration);
    }

    /// <summary>
    /// 封装Debug.DrawLine()方法，绘制红线
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="maxDistance"></param>
    /// <param name="duration"></param>
    public static void DebugDrawLine(Vector3 origin, Vector3 direction, float maxDistance, float duration)
    {
      Debug.DrawLine(origin, origin + direction * maxDistance, Color.red, duration);
    }

    /// <summary>
    /// 使用 Debug.DrawLine() 绘制一个圆。
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <param name="divisions">The more the divisions, the more accurate the circle.</param>
    public static void DebugDrawCircle(Vector3 center, float radius, Color color, float duration, int divisions)
    {
      if (divisions <= 0) return;
      divisions = Mathf.Max(3, divisions);

      for (int i = 0; i <= divisions; i++)
      {
        Vector3 vec1 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * i);
        Vector3 vec2 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * (i + 1));
        Debug.DrawLine(vec1, vec2, color, duration);
      }
    }

    /// <summary>
    /// 使用 Debug.DrawLine() 绘制 CircleCast 的扫掠范围。
    /// </summary>
    /// <param name="origin">圆形检测的起点。</param>
    /// <param name="radius">圆形检测的半径。</param>
    /// <param name="direction">检测方向。</param>
    /// <param name="distance">检测距离。</param>
    /// <param name="divisions">圆形分段数，越大越平滑。</param>
    public static void DebugDrawCircle2D(Vector2 origin, float radius, Vector2 direction, float distance, int divisions = 32)
    {
      if (radius <= 0)
      {
        EditorLogWarning("DebugDrawCircle: radius must be greater than 0.");
        return;
      }

      Vector2 castDirection = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.zero;
      Vector2 end = origin + castDirection * distance;

      // 每帧重画一次
      DebugDrawCircle(origin, radius, Color.red, 0f, divisions);
      DebugDrawCircle(end, radius, Color.red, 0f, divisions);

      if (castDirection == Vector2.zero || distance <= 0) return;

      // 法线
      Vector2 normal = new Vector2(-castDirection.y, castDirection.x) * radius;
      Debug.DrawLine(origin + normal, end + normal, Color.yellow, 0f);
      Debug.DrawLine(origin - normal, end - normal, Color.yellow, 0f);
    }

    /// <summary>
    /// Draw a rectangle using Debug.DrawLine().
    /// </summary>
    /// <param name="minXY"></param>
    /// <param name="maxXY"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public static void DebugDrawRectangle(Vector3 minXY, Vector3 maxXY, Color color, float duration)
    {
      Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(maxXY.x, minXY.y), color, duration);
      Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(minXY.x, maxXY.y), color, duration);
      Debug.DrawLine(new Vector3(minXY.x, maxXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
      Debug.DrawLine(new Vector3(maxXY.x, minXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
    }

    /// <summary>
    /// 使用 Debug.DrawLine() 绘制 BoxCast 的扫掠范围。
    /// </summary>
    /// <param name="origin">盒形检测的中心点。</param>
    /// <param name="size">盒形检测的尺寸。</param>
    /// <param name="angle">盒形检测的旋转角度。</param>
    /// <param name="direction">检测方向。</param>
    /// <param name="distance">检测距离。</param>
    public static void DebugDrawRectangle2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
    {
      if (size.x <= 0 || size.y <= 0)
      {
        EditorLogWarning("DebugDrawRectangle: size must be greater than 0.");
        return;
      }

      Vector2 castDirection = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.zero;
      Vector2 end = origin + castDirection * distance;
      Vector3[] startCorners = GetDebugRectangleCorners(origin, size, angle);
      Vector3[] endCorners = GetDebugRectangleCorners(end, size, angle);

      DebugDrawLines(Vector3.zero, Color.yellow, 1, 0f, new Vector3[]
      {
        startCorners[0], startCorners[1], startCorners[2], startCorners[3], startCorners[0]
      });

      DebugDrawLines(Vector3.zero, Color.yellow, 1, 0f, new Vector3[]
      {
        endCorners[0], endCorners[1], endCorners[2], endCorners[3], endCorners[0]
      });

      if (castDirection == Vector2.zero || distance <= 0) return;

      for (int i = 0; i < startCorners.Length; i++)
      {
        Debug.DrawLine(startCorners[i], endCorners[i], Color.yellow, 0f);
      }
    }

    private static Vector3[] GetDebugRectangleCorners(Vector2 origin, Vector2 size, float angle)
    {
      Vector2 halfSize = size * 0.5f;
      // 左下顺时针
      Vector3[] corners = new Vector3[]
      {
        new Vector3(-halfSize.x, -halfSize.y),
        new Vector3(-halfSize.x, halfSize.y),
        new Vector3(halfSize.x, halfSize.y),
        new Vector3(halfSize.x, -halfSize.y)
      };

      for (int i = 0; i < corners.Length; i++)
      {
        corners[i] = (Vector3)origin + ApplyRotationToVector(corners[i], angle);
      }

      return corners;
    }

    /// <summary>
    /// 使用 Debug.DrawLine() 绘制多条线。
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    /// <param name="duration"></param>
    /// <param name="points">Vertices of the lines.</param>
    public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, Vector3[] points)
    {
      if (points == null)
      {
        EditorLogWarning("DebugDrawLines: points is null.");
        return;
      }

      if (points.Length < 2)
      {
        EditorLogWarning("DebugDrawLines: at least two points are required.");
        return;
      }

      for (int i = 0; i < points.Length - 1; i++)
      {
        Debug.DrawLine(position + points[i] * size, position + points[i + 1] * size, color, duration);
      }
    }

    /// <summary>
    /// 使用 Debug.DrawLine() 绘制多条线。
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    /// <param name="duration"></param>
    /// <param name="points">Points are pairs of 2.</param>
    public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, float[] points)
    {
      if (points == null)
      {
        EditorLogWarning("DebugDrawLines: points is null.");
        return;
      }

      int validLength = points.Length - points.Length % 2;
      if (validLength < 4)
      {
        EditorLogWarning("DebugDrawLines: at least two point pairs are required.");
        return;
      }

      if (validLength != points.Length)
      {
        EditorLogWarning("DebugDrawLines: points length is odd, the last value will be ignored.");
      }

      List<Vector3> vecList = new List<Vector3>();
      for (int i = 0; i < validLength; i += 2)
      {
        Vector3 vec = new Vector3(points[i + 0], points[i + 1]);
        vecList.Add(vec);
      }
      DebugDrawLines(position, color, size, duration, vecList.ToArray());
    }
    #endregion

  }
}
