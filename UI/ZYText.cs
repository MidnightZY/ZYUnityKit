using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace ZYUnityKit
{
  [DisallowMultipleComponent]
  [AddComponentMenu("ZYUnityKit/UI/ZYText")]
  public class ZYText : TextMeshProUGUI
  {
    public void UpdateTextDirectly(string newText)
    {
      text = newText;
    }

  }
}
