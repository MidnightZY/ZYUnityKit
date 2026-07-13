using UnityEngine;
using UnityEditor;
using TMPro.EditorUtilities;

namespace ZYUnityKit
{
  [CustomEditor(typeof(ZYText))]
  [CanEditMultipleObjects]
  public class ZYTextEditor : TMP_EditorPanelUI
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUILayout.Space();
      GUILayout.Label(
        new GUIContent("<b>Extensions</b>"),
        TMP_UIStyleManager.sectionHeader
        );
      EditorGUILayout.LabelField("Optional modules for this ZYText component.（可选模块）");

      // 文本动画模块
      if (HasTargetWithout<ZYTextAnimator>())
      {
        if (GUILayout.Button("Enable Text Animator", GUILayout.Height(30)))
        {
          AddComponentToTargets<ZYTextAnimator>();
        }
      }
      else
      {
        EditorGUILayout.HelpBox("Text Animator enabled.", MessageType.Info);
      }

      // 本地化模块
      EditorGUILayout.Space();
      if (HasTargetWithout<ZYLocalization>())
      {
        if (GUILayout.Button("Enable Localization", GUILayout.Height(30)))
        {
          AddComponentToTargets<ZYLocalization>();
        }
      }
      else
      {
        EditorGUILayout.HelpBox("Localization enabled.", MessageType.Info);
      }
    }

    private bool HasTargetWithout<T>() where T : Component
    {
      foreach (Object obj in targets)
      {
        ZYText text = obj as ZYText;
        if (text != null && !ZYUtils.GetComponentOrNull<T>(text.gameObject))
          return true;
      }

      return false;
    }

    private void AddComponentToTargets<T>() where T : Component
    {
      foreach (Object obj in targets)
      {
        ZYText text = obj as ZYText;
        if (text == null || ZYUtils.GetComponentOrNull<T>(text.gameObject))
          continue;

        Undo.AddComponent<T>(text.gameObject);
      }
    }

  }
}
