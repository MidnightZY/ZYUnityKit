using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多UI暂停管理器
/// 通过栈记录当前打开的暂停 UI 层级，当第一层 UI 打开时暂停游戏，最后一层 UI 关闭时恢复游戏。
/// </summary>
public class ZYMultiMenuPause : MonoBehaviour
{
  private readonly Stack<GameObject> uiStack = new Stack<GameObject>();

  public void OpenUI(GameObject uiPanel)
  {
    if (uiPanel == null) return;

    if (uiStack.Count == 0)
    {
      ZYPauseManager.Instance.Pause();
    }

    uiPanel.SetActive(true);
    uiStack.Push(uiPanel);
  }

  public void CloseUI(GameObject uiPanel)
  {
    if (uiPanel == null) return;
    if (uiStack.Count == 0) return;

    if (uiPanel != uiStack.Peek())
    {
      Debug.LogWarning("关闭了非最上层的 UI。");
      return;
    }

    uiStack.Pop();
    uiPanel.SetActive(false);

    if (uiStack.Count == 0)
    {
      ZYPauseManager.Instance.Resume();
    }
  }
}
