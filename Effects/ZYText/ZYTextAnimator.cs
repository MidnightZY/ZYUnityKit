using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ZYUnityKit
{
  /// <summary>
  /// Plays custom effects on text components.
  /// </summary>
  [RequireComponent(typeof(ZYTextAnimation))]
  [DisallowMultipleComponent]
  [AddComponentMenu("ZYUnityKit/TextAnimator/ZYTextAnimator")]
  public class ZYTextAnimator : MonoBehaviour
  {
    [Header("Typewriter")]
    public bool useTypeWriter = false;
    public ZYTextTypewriterType typewriterType;
    public float typeSpeed = 1.0f;

    [HideInInspector]
    public bool startOnEnable = true; //deprecated

    [Header("Inline Effects")]
    public bool useInlineEffects = false;

    private string oStr; //original string
    private string pStr; //parsed string

    private List<ZYTextAnimSemantics> anims;

    [HideInInspector]
    public ZYTextAnimation zyAnim;
    [HideInInspector]
    public TMP_Text text;

    public UnityAction onTypeWriterFinished;

    [HideInInspector] public int curTypewriterChar = -1;
    [HideInInspector] public string curTypewriterCRID;
    private int maxTypewriterChar = -1;
    private int curTypewriterEffect = 0;
    private bool typewriterPlaying = false;
    private void Awake()
    {
      zyAnim = GetComponent<ZYTextAnimation>();
      text = GetComponent<TMP_Text>();
      oStr = text.text;
    }
    private void Start()
    {
      ZYUtils.InvokeAction(0.1f, () =>
      {
        if (useInlineEffects)
        {
          ParseText();
          ApplyParsedText();
        }

        ZYUtils.InvokeAction(0.1f, () =>
              {
                ZYTextUtils.StopAllRoutines(text);
                if (startOnEnable)
                {
                  PlayTypeWriterInternal();
                }
                if (!startOnEnable || !useTypeWriter)
                {
                  ApplyEffects();
                }
              });
      });
    }
    private void OnDestroy()
    {
      ZYTextUtils.StopAllRoutines(text);
    }
    /// <summary>
    /// Replay the typewriter effect associated to this object.
    /// </summary>
    public void PlayTypeWriter()
    {
      UpdateText(text.text);
    }
    private void PlayTypeWriterInternal()
    {
      if (!useTypeWriter)
        return;

      zyAnim.UpdateTextInfo();
      typewriterPlaying = true;
      curTypewriterEffect = 0;
      curTypewriterChar = -1;
      maxTypewriterChar = -1;
      switch (typewriterType)
      {
        case ZYTextTypewriterType.Standard:
          zyAnim.TypeWriterScaling(typeSpeed);
          break;
        case ZYTextTypewriterType.AlphaFade:
          zyAnim.TypeWriter(typeSpeed);
          break;
        case ZYTextTypewriterType.Rotate:
          zyAnim.TypeWriterRotating(typeSpeed);
          break;
        case ZYTextTypewriterType.Wave:
          zyAnim.TypeWriterWave(typeSpeed);
          break;
        case ZYTextTypewriterType.Translate:
          zyAnim.TypeWriterTranslating(typeSpeed);
          break;
        case ZYTextTypewriterType.Shake:
          zyAnim.TypeWriterShaking(typeSpeed);
          break;
        default:
          break;
      }
    }
    private void Update()
    {
      UpdateTypeWriterEffect();
    }
    /// <summary>
    /// Set a new string for text, then play effects.
    /// </summary>
    /// <param name="s"></param>
    public void UpdateText(string s)
    {
      if (!text)
        return;
      if (!useTypeWriter && !useInlineEffects)
      {
        text.text = s;
        return;
      }

      Color32 oColor = text.color.ToColor32();
      float oa = text.alpha;

      oStr = s;
      text.alpha = 0;

      ParseText();
      ZYTextUtils.StopAllRoutines(text);
      ZYUtils.InvokeAction(0.05f, () =>
      {
        ApplyParsedText();
        zyAnim.UpdateTextDataColor(oColor);
        ZYUtils.InvokeAction(0f, () =>
              {
                text.alpha = oa;
                if (useTypeWriter)
                {
                  PlayTypeWriterInternal();
                }
                else
                {
                  ApplyEffects();
                }
              });
      });
    }

    public void TypewriterFastForward()
    {
      ZYUtils.CancelInvoke(curTypewriterCRID);
      zyAnim.Alpha(10000000, 1, 0, 1000);

      typewriterPlaying = false;
      while (curTypewriterEffect < anims.Count)
      {
        ApplyEffect(anims[curTypewriterEffect++]);
      }
    }

    /// <summary>
    /// Stop the typewriter and display whole text.
    /// </summary>
    public void TypewriteFastForwardComplete(string content)
    {
      ZYUtils.CancelInvoke(curTypewriterCRID);
      int len = text.textInfo.meshInfo[0].colors32.Length;
      for (int i = 0; i < len; i++)
      {
        Color32 c = text.textInfo.meshInfo[0].colors32[i];
        text.textInfo.meshInfo[0].colors32[i] = new Color32(c.r, c.g, c.b, 255);
      }

      text.text = content;
    }

    private void UpdateTypeWriterEffect()
    {
      if (anims == null || anims.Count == 0)
        return;

      if (typewriterPlaying)
      {
        if (curTypewriterChar == -1) return;
        if (curTypewriterChar > maxTypewriterChar)
        {
          maxTypewriterChar = curTypewriterChar;
          if (anims[curTypewriterEffect].endIndex == curTypewriterChar)
          {
            ApplyEffect(anims[curTypewriterEffect]);
            curTypewriterEffect++;
            if (curTypewriterEffect == anims.Count)
            {
              typewriterPlaying = false;
            }
          }
        }
      }
    }
    private void ParseText()
    {
      if (oStr == null)
        return;

      oStr = oStr.Replace(@"\r", "\r");
      oStr = oStr.Replace(@"\n", "\n");

      if (!useInlineEffects)
      {
        pStr = oStr;
        return;
      }


      anims = new List<ZYTextAnimSemantics>();
      ZYTextAnimSemantics curAnim = new ZYTextAnimSemantics();
      if (!text) return;
      TMP_LineInfo[] lineInfos = text.GetTextInfo(oStr).lineInfo;
      if (lineInfos == null || lineInfos.Length == 0)
        return;
      //First pass: construct anims
      string s = oStr;
      int nullCount = 0;
      int curLine = 0;
      for (int i = 0; i < s.Length; i++)
      {
        if (curLine < lineInfos.Length && i > lineInfos[curLine].lastCharacterIndex)
        {
          nullCount++;
          curLine++;
        }
        if (s[i] == ' ')
        {
          nullCount++;
        }
        if (s[i] == '<')
        {
          if (s[i + 1] == '/')
          {
            curAnim.endIndex = i - 1 - nullCount;
            //submit anim
            anims.Add(curAnim);
            curAnim = null;
            while (s[i] != '>') { nullCount++; i++; }
            nullCount++;
          }
          else
          {
            curAnim = new ZYTextAnimSemantics();
            int l = 1;
            while (i + l < s.Length && (s[i + l] != '>' && s[i + l] != '(')) l++;
            curAnim.typedef = s.Substring(i + 1, l - 1); //parse type

            if (s[i + l] == '(') //parse args
            {
              int m = 1, n = 1;
              int curStart = i + l + m;
              while (s[i + l + m] != ')')
              {
                if (s[i + l + m] == ',')
                {
                  curAnim.args.Add(float.Parse(s.Substring(curStart, n - 1)));
                  curStart = i + l + m + 1;
                  n = 0;
                }
                m++;
                n++;
              }
              curAnim.args.Add(float.Parse(s.Substring(curStart, n - 1)));
              nullCount += l + m + 2;
              i = i + l + m + 2;
              while (s[i] == ' ') { nullCount++; i++; }
              curAnim.startIndex = i - nullCount;
            }
            else
            {
              nullCount += l + 1;
              i = i + l + 1;
              while (i < s.Length && s[i] == ' ') { nullCount++; i++; }
              curAnim.startIndex = i - nullCount;
            }
          }

        }
      }

      //Second pass: remove markers
      StringBuilder sb = new StringBuilder();
      bool excl = false;
      for (int i = 0; i < s.Length; i++)
      {
        if (excl)
        {
          if (s[i] == '>')
            excl = false;
        }
        else
        {
          if (s[i] == '<')
          {
            excl = true;
          }
          else
          {
            sb.Append(s[i]);
          }
        }
      }

      pStr = sb.ToString();
    }
    private void ApplyEffect(ZYTextAnimSemantics anim)
    {
      if (!useInlineEffects)
      {
        return;
      }
      if (anim.typedef == ZYTextUtils.CM_SHAKE)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Shake(anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.ShakeTimed(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_BANNER)
      {
        int argCount = anim.args.Count;
        if (argCount < 4)
        {
          zyAnim.Banner(1, new Color32(250, 100, 40, 255), anim.startIndex, anim.endIndex);
        }
        else if (argCount == 4)
        {
          zyAnim.Banner(anim.args[0], new Color32((byte)anim.args[1], (byte)anim.args[2], (byte)anim.args[3], 255), anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_FADE)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Fade(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.Fade(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_TWINKLE)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Twinkle(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.Twinkle(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_DANGLE)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Dangle(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.Dangle(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_EXCLAIM)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Exclaim(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 4)
        {
          zyAnim.ExclaimColor(anim.args[0], new Color32((byte)anim.args[1], (byte)anim.args[2], (byte)anim.args[3], 255), anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.Exclaim(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_EXCLAIM_TIMED)
      {
        int argCount = anim.args.Count;
        if (argCount == 5)
        {
          zyAnim.ExclaimColorTimed(anim.args[0], anim.args[1], new Color32((byte)anim.args[2], (byte)anim.args[3], (byte)anim.args[4], 255), anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_WAVE_LOOP)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.WaveLoop(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.WaveLoop(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_SCALE_UP)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.ScaleUp(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.ScaleUp(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_SCALE_DOWN)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.ScaleDown(1, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.ScaleDown(anim.args[0], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_SCALE_ROTATE)
      {
        int argCount = anim.args.Count;
        if (argCount == 0)
        {
          zyAnim.Rotate(1, 15, anim.startIndex, anim.endIndex);
        }
        else if (argCount == 1)
        {
          zyAnim.Rotate(anim.args[0], 15, anim.startIndex, anim.endIndex);
        }
        else
        {
          zyAnim.Rotate(anim.args[0], anim.args[1], anim.startIndex, anim.endIndex);
        }
      }
      else if (anim.typedef == ZYTextUtils.CM_SCALE_COLOR)
      {
        int argCount = anim.args.Count;
        if (argCount == 4)
        {
          zyAnim.Color(anim.args[0], new Color32((byte)anim.args[1], (byte)anim.args[2], (byte)anim.args[3], 255), anim.startIndex, anim.endIndex);
        }
        else if (argCount == 3)
        {
          zyAnim.Color(1, new Color32((byte)anim.args[0], (byte)anim.args[1], (byte)anim.args[2], 255), anim.startIndex, anim.endIndex);
        }
      }

    }
    private void ApplyEffects()
    {
      if (!useInlineEffects)
      {
        return;
      }
      foreach (ZYTextAnimSemantics anim in anims)
      {
        ApplyEffect(anim);
      }
    }

    private void ApplyParsedText()
    {
      if (!text)
        return;
      text.SetText(pStr);
      text.ForceMeshUpdate(true, true);
      zyAnim.UpdateTextInfo(pStr);
    }

  }

  public class ZYTextAnimSemantics
  {
    public string typedef;
    public List<float> args = new List<float>();
    public int startIndex, endIndex;
  }
  public enum ZYTextTypewriterType
  {
    Standard,
    AlphaFade,
    Rotate,
    Wave,
    Translate,
    Shake
  }
}
