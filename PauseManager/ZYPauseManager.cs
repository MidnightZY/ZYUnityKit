using UnityEngine;
using ZYUnityKit;

/// <summary>
/// 游戏暂停管理器。
/// 负责统一维护游戏暂停状态，并通过 Time.timeScale 控制游戏时间的暂停与恢复。
/// UI 显示、音频处理等表现层逻辑可以在暂停状态变化时由外部系统响应。
/// </summary>
public class ZYPauseManager : ZYSingleton<ZYPauseManager>
{
  public bool IsPaused { get; private set; }

  public void Pause()
  {
    if (IsPaused) return;

    IsPaused = true;
    Time.timeScale = 0f;
  }

  public void Resume()
  {
    if (!IsPaused) return;

    IsPaused = false;
    Time.timeScale = 1f;
  }
}