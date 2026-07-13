using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;


namespace ZYUnityKit
{
  [AddComponentMenu("ZYUnityKit/Misc/ZYCommonTimer")]
  // 公共计时器&延迟执行中心
  public sealed class ZYCommonTimer : ZYMonoSingleton<ZYCommonTimer>
  {
    private double scaledTimer = 0f; // 受Time.timeScale影响的时间
    private double fixedTimer = 0f; //基于FixedUpdate的时间
    private double unScaledTimer = 0; // 不受Time.timeScale影响的时间

    // 保存延迟执行的协程，方便用 id 取消。
    private Dictionary<string, Coroutine> crDict = new Dictionary<string, Coroutine>();

    // 保存命名计时器。
    private Dictionary<string, (double, TimerMode)> timerDict = new Dictionary<string, (double, TimerMode)>();

    protected override void Awake()
    {
      base.Awake();
      // ZYUtils.mainCam = Camera.main;
    }

    private void Start()
    {
      // 将Tick() 注册给 SKCore.Tick000
      ZYCore.Tick000 += Tick;
      ZYCore.FixedTick000 += FixedTick;
      ZYCore.UnscaledTick000 += UnScaledTick;
    }

    private void FixedTick()
    {
      fixedTimer += Time.fixedDeltaTime;
    }

    private void Tick()
    {
      scaledTimer += Time.deltaTime;
    }

    private void UnScaledTick()
    {
      unScaledTimer += Time.unscaledDeltaTime;
    }

    /// <summary>
    /// 新建一个计时器
    /// </summary>
    /// <param name="name">计时器的名字</param>
    /// <param name="startTime">计时器的初始时间</param>
    /// <param name="isScaled">如果计时器受 Time.timescale 影响，则为真</param> 
    public void CreateTimer(string name, float startTime = 0, TimerMode timerMode = TimerMode.Scaled)
    {
      ZYUtils.InsertOrUpdateKeyValueInDictionary(timerDict, name, (scaledTimer - startTime, timerMode));
    }

    /// <summary>
    /// 移除计时器
    /// </summary>
    /// <param name="name"></param>
    public void DisposeTimer(string name)
    {
      ZYUtils.RemoveKeyInDictionary(timerDict, name);
    }

    public double GetTimerValue(string name)
    {
      (double, TimerMode) info = ZYUtils.GetValueInDictionary(timerDict, name);
      switch (info.Item2)
      {
        case TimerMode.Scaled:
          return scaledTimer - info.Item1;
        case TimerMode.Fixed:
          return fixedTimer - info.Item1;

        case TimerMode.Unscaled:
          return unScaledTimer - info.Item1;

        default:
          return scaledTimer - info.Item1;
      }
    }

    /// <summary>
    /// 在指定的时间秒后调用一个动作，然后每隔重复间隔秒重复一次，在达到重复次数后停止。
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <param name="repeatCount"></param>
    /// <param name="repeatInterval"></param>
    public void InvokeAction(float seconds, Action callback, int repeatCount = 0, float repeatInterval = 1, string id = "", Action onFinish = null)
    {
      Coroutine cr = StartCoroutine(SimpleActionCoroutine(seconds, callback, repeatCount, repeatInterval, false, onFinish));

      if (id.Length > 0)
      {
        ZYUtils.InsertOrUpdateKeyValueInDictionary(crDict, id, cr);
      }
    }


    /// <summary>
    /// 终止指定协程
    /// </summary>
    /// <param name="id"></param>
    public void CancelInvokeAction(string id)
    {
      if (!Instance) return;

      if (crDict.ContainsKey(id))
      {
        StopCoroutine(crDict[id]);
        crDict.Remove(id);
      }
    }

    /// <summary>
    /// 延迟 seconds 秒后开始执行 callback，然后每隔 repeatInterval 秒重复执行
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <param name="repeatInterval"></param>
    /// <param name="id"></param>
    public void InvokeActionUnlimited(float seconds, Action callback, float repeatInterval = 1, string id = "")
    {
      Coroutine cr = StartCoroutine(SimpleActionCoroutine(seconds, callback, 10, repeatInterval, true));

      if (id.Length > 0)
      {
        ZYUtils.InsertOrUpdateKeyValueInDictionary(crDict, id, cr);
      }
    }

    private IEnumerator SimpleActionCoroutine(float seconds, Action callback, int repeatCount = 0, float repeatInterval = 1, bool unlimited = false, Action onFinish = null)
    {
      yield return new WaitForSecondsRealtime(seconds);
      callback.Invoke();

      for (int i = 0; i < repeatCount; i++)
      {
        if (unlimited)
          i = 0;
        yield return new WaitForSecondsRealtime(repeatInterval);
        callback.Invoke();
      }

      if (onFinish != null)
        onFinish.Invoke();
    }

    public enum TimerMode
    {
      Scaled,
      Fixed,
      Unscaled
    }
  }
}