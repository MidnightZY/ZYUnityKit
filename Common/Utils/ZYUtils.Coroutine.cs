using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace ZYUnityKit
{
  public static partial class ZYUtils
  {
    private static Dictionary<string, Coroutine> procedureDict = new Dictionary<string, Coroutine>();

    private static Dictionary<string, IEnumerator> crDict = new Dictionary<string, IEnumerator>();

    /// <summary>
    /// 在指定时间秒后调用一个动作，然后每隔 repeatInterval 秒重复一次，重复 repeatCount 次后停止。使用未缩放时间。
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <param name="repeatCount"></param>
    /// <param name="repeatInterval"></param>
    public static void InvokeAction(float seconds, Action callback, int repeatCount = 0, float repeatInterval = 1, string id = "", Action onFinish = null)
    {
      if (callback == null)
      {
        EditorLogError("InvokeAction: <Action callback> is null.");
        return;
      }
      if (!ZYCommonTimer.Instance) return;
      ZYCommonTimer.Instance.InvokeAction(seconds, callback, repeatCount, repeatInterval, id, onFinish);
    }

    // #if UNITY_EDITOR
    //     /// <summary>
    //     /// Invoke an action after time seconds in editor mode.
    //     /// </summary>
    //     /// <param name="seconds"></param>
    //     /// <param name="callback"></param>
    //     public static void InvokeActionEditor(float seconds, Action callback)
    //     {
    //       ZYEditorCoroutineManager.StartEditorCoroutine(EditorActionCoroutine(seconds, callback));
    //     }
    //     private static IEnumerator EditorActionCoroutine(float seconds, Action callback)
    //     {
    //       yield return new WaitForSeconds(seconds);
    //       callback.Invoke();
    //     }
    // #endif
    /// <summary>
    /// 在经过 time 秒后触发一次动作，然后每隔 repeatInterval 秒重复执行。
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <param name="repeatInterval"></param>
    public static void InvokeActionUnlimited(float seconds, Action callback, float repeatInterval = 1, string id = "")
    {
      if (callback == null)
      {
        EditorLogError("InvokeAction: <Action callback> is null.");
        return;
      }
      if (!ZYCommonTimer.Instance) return;
      ZYCommonTimer.Instance.InvokeActionUnlimited(seconds, callback, repeatInterval, id);
    }

    /// <summary>
    /// 在调用 InvokeAction 时取消由 id 指定的操作。
    /// </summary>
    /// <param name="id">ID of action.</param>
    public static void CancelInvoke(string id)
    {
      if (!ZYCommonTimer.Instance) return;
      ZYCommonTimer.Instance.CancelInvokeAction(id);
    }

    /// <summary>
    /// 开始一个变量随时间变化的连续过程。补间动画。
    /// </summary>
    /// <param name="curve">Curve of the procedure.</param>
    /// <param name="startValue">Initial value of the variable.</param>
    /// <param name="endValue">Target value of the variable.</param>
    /// <param name="timeParam">ProcedureType.Linear: Seconds to finish the procedure;  ProcedureType.Lerp: Lerp amount per frame.</param>
    /// <param name="action">Action called per frame.</param>
    /// <param name="onFinish">Action called at the end of the procedure.</param>
    /// <param name="allowMultipleInstances">If false, previous procedures with the same name will be terminated.</param>
    public static Coroutine StartProcedure(ZYCurve curve, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish = null, bool allowMultipleInstances = false, string id = "")
    {
      Coroutine cr = StartCoroutine(ProcedureCR(curve, startValue, endValue, timeParam, action, onFinish), allowMultipleInstances);
      if (id.Length > 0)
      {
        InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
      }
      return cr;
    }

    /// <summary>
    /// 开始一个连续的过程，其中变量随时间从0变化到1。插值动画。
    /// </summary>
    /// <param name="curve">Curve of the procedure.</param>
    /// <param name="action">Action called per frame.</param>
    /// <param name="onFinish">Action called at the end of the procedure.</param>
    /// <param name="id"></param>
    public static Coroutine StartProcedure(ZYCurve curve, float time, Action<float> action, Action<float> onFinish = null, string id = "")
    {
      Coroutine cr = StartCoroutine(ProcedureCR(curve, 0, 1, time, action, onFinish), true);
      if (id.Length > 0)
      {
        InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
      }
      return cr;
    }
    /// <summary>
    /// Starts a continuous procedure where a variable changes from 0 to 1 over time. Tweening.
    /// </summary>
    /// <param name="curve">Curve of the procedure.</param>
    /// <param name="action">Action called per frame.</param>
    /// <param name="onFinish">Action called at the end of the procedure.</param>
    /// <param name="id"></param>
    public static Coroutine StartProcedureUnscaled(ZYCurve curve, float time, Action<float> action, Action<float> onFinish = null, string id = "")
    {
      Coroutine cr = StartCoroutine(ProcedureCRUnscaled(curve, 0, 1, time, action, onFinish), true);
      if (id.Length > 0)
      {
        InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
      }
      return cr;
    }
    /// <summary>
    /// [DEPRECATED] Starts a continuous procedure where a variable changes over time. Tweening.
    /// </summary>
    /// <param name="type">Type of the procedure.</param>
    /// <param name="startValue">Initial value of the variable.</param>
    /// <param name="endValue">Target value of the variable.</param>
    /// <param name="timeParam">ProcedureType.Linear: Seconds to finish the procedure;  ProcedureType.Lerp: Lerp amount per frame.</param>
    /// <param name="action">Action called per frame.</param>
    /// <param name="onFinish">Action called at the end of the procedure.</param>
    /// <param name="lerpThreshold">Only in Lerp mode: the threshold of lerp operation.</param>
    /// <param name="allowMultipleInstances">If false, previous procedures with the same name will be terminated.</param>
    public static Coroutine StartProcedure(ProcedureType type, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish = null, float lerpThreshold = 0.05f, bool allowMultipleInstances = false, string id = "")
    {
      Coroutine cr = StartCoroutine(ProcedureCR(type, startValue, endValue, timeParam, action, onFinish, lerpThreshold), allowMultipleInstances);
      if (id.Length > 0)
      {
        InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
      }
      return cr;
    }

    /// <summary>
    /// Stops the procedure specified by id when calling StartProcedure.
    /// </summary>
    /// <param name="id"></param>
    public static void StopProcedure(string id)
    {
      if (procedureDict.ContainsKey(id))
      {
        StopCoroutine(procedureDict[id]);
        procedureDict.Remove(id);
      }
    }

    private static IEnumerator ProcedureCR(ProcedureType type, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish, float lerpThreshold)
    {
      float variable = startValue;
      if (type == ProcedureType.Linear)
      {
        float stepValue = (endValue - startValue) / timeParam * Time.fixedDeltaTime;
        int step = Mathf.CeilToInt(timeParam / Time.fixedDeltaTime);

        for (int i = 0; i < step; i++)
        {
          variable += stepValue;

          if (action != null)
            action.Invoke(variable);

          yield return new WaitForFixedUpdate();
        }
      }
      if (type == ProcedureType.Lerp)
      {
        while (Mathf.Abs(endValue - variable) > lerpThreshold)
        {
          variable = Mathf.Lerp(variable, endValue, timeParam);
          action.Invoke(variable);
          yield return new WaitForEndOfFrame();
        }
      }
      variable = endValue;
      if (onFinish != null)
        onFinish.Invoke(variable);
    }

    static WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();

    private static IEnumerator ProcedureCR(ZYCurve curve, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish)
    {
      float step = timeParam / Time.fixedDeltaTime;
      float diff = endValue - startValue;
      float variable;
      for (int i = 0; i < step; i++)
      {
        variable = startValue + (ZYCurveSampler.SampleCurve(curve, i / step)) * diff;
        action?.Invoke(variable);

        yield return waitFixedUpdate;
      }
      variable = curve.curveDir == CurveDir.In ? endValue : startValue;
      action?.Invoke(variable);
      onFinish?.Invoke(variable);
    }
    static WaitForSecondsRealtime waitTick = new WaitForSecondsRealtime(0.015f);

    private static IEnumerator ProcedureCRUnscaled(ZYCurve curve, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish)
    {
      float step = timeParam / 0.015f;
      float diff = endValue - startValue;
      float variable;
      for (int i = 0; i < step; i++)
      {
        variable = startValue + (ZYCurveSampler.SampleCurve(curve, i / step)) * diff;
        action?.Invoke(variable);

        yield return new WaitForSecondsRealtime(0.015f); ;
      }
      variable = curve.curveDir == CurveDir.In ? endValue : startValue;
      action?.Invoke(variable);
      onFinish?.Invoke(variable);
    }
    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    /// <param name="cr"></param>
    /// <param name="allowMultipleInstances">If not, previous instances of the coroutine will be stopped.</param>
    /// <returns></returns>

    public static Coroutine StartCoroutine(IEnumerator cr, bool allowMultipleInstances = false, string id = "")
    {
      if (!ZYCommonTimer.Instance) return null;
      if (!allowMultipleInstances)
      {
        if (crDict.ContainsKey(nameof(cr)))
        {
          ZYCommonTimer.Instance.StopCoroutine(crDict[nameof(cr)]);
        }
        InsertOrUpdateKeyValueInDictionary(crDict, nameof(cr), cr);
      }
      if (id.Length > 0)
        InsertOrUpdateKeyValueInDictionary(crDict, id, cr);
      return ZYCommonTimer.Instance.StartCoroutine(cr);
    }

    /// <summary>
    /// Stops a coroutine.
    /// </summary>
    /// <param name="cr"></param>
    public static void StopCoroutine(IEnumerator cr)
    {
      if (!ZYCommonTimer.Instance) return;
      ZYCommonTimer.Instance.StopCoroutine(cr);
    }
    /// <summary>
    /// Stops a coroutine.
    /// </summary>
    /// <param name="cr"></param>
    public static void StopCoroutine(string id)
    {
      if (!ZYCommonTimer.Instance) return;
      if (id.Length > 0)
        if (crDict.ContainsKey(id))
          ZYCommonTimer.Instance.StopCoroutine(crDict[id]);
    }
    /// <summary>
    /// Stops a coroutine.
    /// </summary>
    /// <param name="cr"></param>
    public static void StopCoroutine(Coroutine cr)
    {
      if (ZYCommonTimer.Instance) //Batch v0.15.3: fixed issue with ZYTextAnimator errors when quitting game
        ZYCommonTimer.Instance.StopCoroutine(cr);
    }

    /// <summary>
    /// Release an object into an object pool after time seconds
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time"></param>
    public static void ReleaseObject(GameObject obj, float time)
    {
      InvokeAction(time, () =>
      {
        ReleaseObject(obj);
      });
    }
  }

  public enum ProcedureType
  {
    Linear,
    Lerp
  }

}