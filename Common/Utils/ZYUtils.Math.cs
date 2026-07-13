using UnityEngine;

namespace ZYUnityKit
{
  public static partial class ZYUtils
  {
    /// <summary>
    /// 将输入值 x 映射到区间 [a, b] 上的平滑 0~1 权重。
    /// </summary>
    /// <remarks>
    /// 这个函数更接近 Shader 中的 smoothstep(edge0, edge1, x)，
    /// 和 Unity 的 Mathf.SmoothStep(from, to, t) 参数语义不同：
    /// 这里的 a、b 是输入边界，x 是输入值，返回值始终是 0~1 的平滑比例。
    /// 当 a 和 b 几乎相等时，区间退化为一个点，此时按 step 行为处理，避免除以 0。
    /// </remarks>
    /// <param name="a">输入区间的起始边界。</param>
    /// <param name="b">输入区间的结束边界。</param>
    /// <param name="x">需要被映射的输入值。</param>
    /// <returns>x 在 [a, b] 区间中的平滑 0~1 权重。</returns>
    public static float SmoothStep01(float a, float b, float x)
    {
      float range = b - a;

      if (Mathf.Abs(range) < 0.000001f)
      {
        return x < a ? 0f : 1f;
      }

      float t = Mathf.Clamp01((x - a) / range);
      return t * t * (3f - 2f * t);
    }

    public static Vector3 RandomVector2()
    {
      return new Vector2(Random(-1f, 1f), Random(-1f, 1f));
    }

    public static float Random(float min, float max)
    {
      return UnityEngine.Random.Range(min, max);
    }
  }


}