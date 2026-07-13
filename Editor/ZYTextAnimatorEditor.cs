using UnityEngine;
using UnityEditor;

namespace ZYUnityKit
{
  [CustomEditor(typeof(ZYTextAnimator))]
  [CanEditMultipleObjects]
  public class ZYTextAnimatorEditor : Editor
  {
    public ZYTextAnimator anim;

    bool expanded = false;
    GUIStyle bold = new GUIStyle(EditorStyles.boldLabel);

    private void OnEnable()
    {
      anim = (ZYTextAnimator)target;
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      if (!expanded)
      {
        GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
        if (GUILayout.Button("< View effect commands >"))
        {
          expanded = true;
        }
        GUI.contentColor = Color.white;
      }
      else
      {
        GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
        if (GUILayout.Button("< Hide effect commands >"))
        {
          expanded = false;
        }
        GUI.contentColor = Color.white;
        GUILayout.Label("To use command: <effectName(arg0,arg1...)>Text</>", bold);
        GUILayout.Label("1. 抖动: <shake(time)>");
        GUILayout.Label("2. 彩虹横幅: <banner(time,r,g,b)>");
        GUILayout.Label("3. 淡入淡出: <fade(speed)>");
        GUILayout.Label("4. 闪烁: <twinkle(speed)>");
        GUILayout.Label("5. 摇摆: <dangle(speed)>");
        GUILayout.Label("6. 强调: <excl(speed,r,g,b)>");
        GUILayout.Label("7. 定时强调: <exclt(speed,time,r,g,b)>");
        GUILayout.Label("8. 波浪: <wave(speed)>");
        GUILayout.Label("9. 放大: <scaleup(speed)>");
        GUILayout.Label("10. 缩小: <scaledn(speed)>");
        GUILayout.Label("11. 旋转: <rot(speed,angle)>");
        GUILayout.Label("12. 变色: <col(speed,time,r,g,b)>");
      }

    }
  }
}
