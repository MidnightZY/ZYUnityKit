using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace ZYUnityKit
{
  [RequireComponent(typeof(CanvasGroup))]
  [AddComponentMenu("ZYUnityKit/UI/ZYButton")]
  public class ZYButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IFadable
  {

    #region Fields
    [SerializeField] bool interactable = true;

    public bool Interactable
    {
      get => interactable;
      set
      {
        SetInteractability(value);
      }
    }

    [SerializeField] ZYText buttonText;
    [SerializeField] ZYImage buttonImage;

    [Header("Transition")]

    public ZYButtonTransitionMode transitionMode = ZYButtonTransitionMode.ColorImageAndText;
    public float transitionTime = 0.1f;
    [SerializeField] Color imageNormalColor = Color.white;
    [SerializeField] Color imageMouseOverColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] Color imageMousePressColor = Color.grey;
    [SerializeField] Color imageDisabledColor = Color.grey;

    [SerializeField] Color textNormalColor = Color.black;
    [SerializeField] Color textMouseOverColor = Color.black;
    [SerializeField] Color textMousePressColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] Color textDisabledColor = new Color(0.6f, 0.6f, 0.6f);

    public bool useScaleTransition = true;
    [Tooltip("开启后，按钮在鼠标悬停和按下时会有弹跳感的缩放过渡效果。")]
    public bool bouncyScaleTransition = false;
    [SerializeField] float normalScale = 1f, mouseOverScale = 0.95f, mousePressScale = 1.05f;

    [Header("Spam Control")]
    [Tooltip("开启后，该按钮不会处理时间间隔过近的事件。")]
    [SerializeField] bool spamPrevention = true;
    [Range(0.01f, 3f)] public float spamCooldown = 0.5f;

    [Header("Events")]
    [SerializeField] ZYButtonEvent onPress;

    [SerializeField] ZYButtonEvent onHoldStays;
    [Tooltip("Invoke OnHoldStays event every fixedUpdateInterval while holding the button.")]

    [SerializeField] ZYButtonEvent onHoldStaysForSeconds;
    [Tooltip("在持续按住的这段时间后触发 OnHoldStaysForSeconds 事件。")]
    [Range(0.01f, 10f)][SerializeField] float holdForSecondsTime = 0.5f;

    [SerializeField] ZYButtonEvent onHoldStaysForSeconds2;
    [Tooltip("在持续按住时，在这几秒后触发 OnHoldStaysForSeconds2 事件。")]
    [Range(0.01f, 10f)][SerializeField] float holdForSecondsTime2 = 0.8f;

    [SerializeField] ZYButtonEvent onHoldUp;
    [Tooltip("在按住按钮达到此秒数后触发 OnHoldUp 事件。")]
    [Range(0.01f, 10f)][SerializeField] float minHoldTime = 1f;

    [SerializeField] ZYButtonEvent onHoldUp2;
    [Tooltip("在按住按钮持续这些秒数后触发 OnHoldUp2 事件。")]
    [Range(0.01f, 10f)][SerializeField] float minHoldTime2 = 2f;

    [SerializeField] ZYButtonEvent onPointerEnter;

    [SerializeField] ZYButtonEvent onPointerExit;

    [SerializeField] ZYButtonEvent onPointerUp;

    [SerializeField] ZYButtonEvent onPointerDown;

    [SerializeField] ZYButtonEvent onStart;

    #endregion

    #region Private Fields
    [HideInInspector] public bool initialized = false;
    private float initialScale;
    private bool canClick = true;
    private bool isHolding = false, isHovering = false;
    private string hash;
    Animator animator;
    private CanvasGroup canvasGroup;
    [HideInInspector] public bool hasAnimator = false;
    #endregion

    #region Editor 
#if UNITY_EDITOR
    /// <summary>
    /// 给一个空的/未初始化的 ZYButton 自动生成标准按钮结构
    /// </summary>
    public void GenerateStructure()
    {
      string pathSuffix = "/CommonButton.prefab"; //路径后缀
      GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ZYAssetLibrary.PREFAB_PATH + pathSuffix);
      if (prefab == null)
      {
        ZYUtils.EditorLogError("ZYButton Resource Error: Button prefab lost.");
        initialized = false;
        return;
      }
      // 新建button一个
      GameObject button = Instantiate(prefab);
      button.name = $"ZYButton-{GetHashCode()}";
      button.transform.SetParent(transform.parent);
      button.transform.CopyTransformFrom(transform);
      button.transform.SetSiblingIndex(transform.GetSiblingIndex());
      button.GetComponent<ZYButton>().initialized = true;
      Selection.activeGameObject = button;
      DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// 在编辑器 Inspector 里预览按钮颜色
    /// </summary>
    public void DrawEditorPreview()
    {
      if (buttonImage != null && buttonText != null)
      {
        if (transitionMode == ZYButtonTransitionMode.ColorImageOnly)
        {
          buttonImage.color = interactable ? imageNormalColor : imageDisabledColor;
          buttonText.color = textNormalColor;
        }
        else if (transitionMode == ZYButtonTransitionMode.ColorTextOnly)
        {
          buttonText.color = interactable ? textNormalColor : textDisabledColor;
          buttonImage.color = imageNormalColor;
        }
        else if (transitionMode == ZYButtonTransitionMode.ColorImageAndText)
        {
          buttonImage.color = interactable ? imageNormalColor : imageDisabledColor;
          buttonText.color = interactable ? textNormalColor : textDisabledColor;
        }
      }
    }

    /// <summary>
    /// 给按钮添加 Animator，并绑定自动生成的 Animator Controller
    /// </summary>
    /// <param name="controller"></param>
    public void AttachController(AnimatorController controller)
    {
      Animator anim = gameObject.AddComponent<Animator>();
      anim.runtimeAnimatorController = controller as RuntimeAnimatorController;
      this.animator = ZYUtils.GetComponentOrNull<Animator>(gameObject);
      hasAnimator = true;
    }

    /// <summary>
    /// 删除按钮上的 Animator 组件，并把内部 animator 引用清空
    /// </summary>
    public void DetachController()
    {
      if (GetComponent<Animator>() != null)
      {
        DestroyImmediate(GetComponent<Animator>());
        this.animator = null;
        hasAnimator = false;
      }
    }
#endif
    #endregion

    #region Start
    void Start()
    {
      hash = GetHashCode().ToString();

      initialScale = transform.localScale.x;

      buttonImage.color = Interactable ? imageNormalColor : imageDisabledColor;
      buttonText.color = Interactable ? textNormalColor : textDisabledColor;

      if (transitionMode == ZYButtonTransitionMode.Animation)
      {
        animator = ZYUtils.GetComponentOrNull<Animator>(gameObject);
      }

      canvasGroup = ZYUtils.GetComponentOrNull<CanvasGroup>(gameObject);

      onStart.Invoke();
    }

    public void SetInteractability(bool value)
    {
      interactable = value;
      OnInteractabilityChange();
    }
    #endregion

    #region Transition
    public void TransitionToNormal()
    {
      TransitionTo(ZYButtonVisualState.Normal);
    }

    public void TransitionToOver()
    {
      TransitionTo(ZYButtonVisualState.Over);
    }

    public void TransitionToPress()
    {
      TransitionTo(ZYButtonVisualState.Pressed);
    }

    public void TransitionToDisabled()
    {
      TransitionTo(ZYButtonVisualState.Disabled);
    }

    public void TransitionTo(ZYButtonVisualState state)
    {
      if (transitionMode == ZYButtonTransitionMode.None) return;

      if (transitionMode == ZYButtonTransitionMode.Animation)
      {
        if (animator == null)
        {
          Debug.LogWarning("Animator component is missing on the button.");
          return;
        }

        string triggerName = GetAnimationTriggerName(state);
        animator.ResetTrigger(triggerName);
        animator.SetTrigger(triggerName);
        return;
      }

      StopAllCoroutines();

      if (transitionMode != ZYButtonTransitionMode.ColorTextOnly)
      {
        StartCoroutine(TransitionImageCR(GetTargetImageColor(state)));
      }

      if (transitionMode != ZYButtonTransitionMode.ColorImageOnly)
      {
        StartCoroutine(TransitionTextCR(GetTargetTextColor(state)));
      }

      if (useScaleTransition)
      {
        StartCoroutine(TransitionScaleCR(GetTargetScale(state), transitionTime));
      }
    }

    private IEnumerator TransitionTextCR(Color targetColor)
    {
      if (buttonText == null) yield break;

      Color startColor = buttonText.color;
      float elapsedTime = 0f;

      while (elapsedTime < transitionTime)
      {
        elapsedTime += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(elapsedTime / transitionTime);
        buttonText.color = Color.Lerp(startColor, targetColor, progress);
        yield return null;
      }

      buttonText.color = targetColor;
      yield return null;
    }

    private IEnumerator TransitionImageCR(Color targetColor)
    {
      if (buttonImage == null) yield break;

      Color startColor = buttonImage.color;
      float elapsedTime = 0f;

      while (elapsedTime < transitionTime)
      {
        elapsedTime += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(elapsedTime / transitionTime);
        buttonImage.color = Color.Lerp(startColor, targetColor, progress);
        yield return null;
      }

      buttonImage.color = targetColor;
      yield return null;
    }

    private IEnumerator TransitionScaleCR(float targetScale, float transitionTime)
    {
      float startScale = transform.localScale.x;
      float realTargetScale = initialScale * targetScale;
      float elapsedTime = 0f;

      if (transitionTime <= 0f)
      {
        transform.localScale = Vector3.one * realTargetScale;
        yield break;
      }

      while (elapsedTime < transitionTime)
      {
        elapsedTime += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(elapsedTime / transitionTime);
        float currentScale = Mathf.Lerp(startScale, realTargetScale, progress);
        transform.localScale = Vector3.one * currentScale;
        yield return null;
      }

      transform.localScale = Vector3.one * realTargetScale;
      yield return null;
    }

    private Color GetTargetImageColor(ZYButtonVisualState state)
    {
      return state switch
      {
        ZYButtonVisualState.Normal => imageNormalColor,
        ZYButtonVisualState.Over => imageMouseOverColor,
        ZYButtonVisualState.Pressed => imageMousePressColor,
        ZYButtonVisualState.Disabled => imageDisabledColor,
        _ => imageNormalColor
      };
    }

    private Color GetTargetTextColor(ZYButtonVisualState state)
    {
      return state switch
      {
        ZYButtonVisualState.Normal => textNormalColor,
        ZYButtonVisualState.Over => textMouseOverColor,
        ZYButtonVisualState.Pressed => textMousePressColor,
        ZYButtonVisualState.Disabled => textDisabledColor,
        _ => textNormalColor
      };
    }

    private float GetTargetScale(ZYButtonVisualState state)
    {
      return state switch
      {
        ZYButtonVisualState.Over => mouseOverScale,
        ZYButtonVisualState.Pressed => mousePressScale,
        _ => normalScale
      };
    }

    private string GetAnimationTriggerName(ZYButtonVisualState state)
    {
      return state switch
      {
        ZYButtonVisualState.Normal => "Normal",
        ZYButtonVisualState.Over => "Over",
        ZYButtonVisualState.Pressed => "Pressed",
        ZYButtonVisualState.Disabled => "Disabled",
        _ => "Normal"
      };
    }

    #endregion

    #region Event Handlers
    public void OnPointerClick(PointerEventData eventData)
    {
      if (!Interactable) return;

      if (spamPrevention)
      {
        if (canClick)
        {
          onPress.Invoke();
          // 终止残留CD
          ZYUtils.CancelInvoke("ResetClickCD");
          // 新加载CD
          ZYUtils.InvokeAction(spamCooldown, ResetClickCD, 0, 0, "ResetClickCD");
          canClick = false;
        }
      }
      else
      {
        onPress.Invoke();
      }
    }

    private void ResetClickCD()
    {
      canClick = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (!Interactable)
        return;
      TransitionToPress();

      //Start hold event
      if (onHoldStays.GetPersistentEventCount() > 0)
      {
        ZYCore.Tick000 += OnHoldStays;
      }

      ZYCommonTimer.Instance.CreateTimer(hash, 0, ZYCommonTimer.TimerMode.Unscaled);
      isHolding = true;

      //Invoke onPointerDown event
      if (onPointerDown.GetPersistentEventCount() > 0)
      {
        onPointerDown.Invoke();
      }

      Invoke("OnHoldStaysForSeconds1", holdForSecondsTime);
      Invoke("OnHoldStaysForSeconds2", holdForSecondsTime2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (!Interactable)
        return;
      //End hold event
      if (isHolding)
      {
        if (isHovering)
          TransitionToOver();
        else
          TransitionToNormal();
        float holdTime = GetCurrentHoldTime();
        if (holdTime >= minHoldTime)
        {
          onHoldUp.Invoke();
        }
        if (holdTime >= minHoldTime2)
        {
          onHoldUp2.Invoke();
        }
        if (onHoldStays.GetPersistentEventCount() > 0)
          ZYCore.Tick000 -= OnHoldStays;
        ZYCommonTimer.Instance.DisposeTimer(hash);
        isHolding = false;
      }
      //Invoke onPointerUp event
      if (onPointerUp.GetPersistentEventCount() > 0)
      {
        onPointerUp.Invoke();
      }

      CancelInvoke("OnHoldStaysForSeconds1");
      CancelInvoke("OnHoldStaysForSeconds2");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (!interactable)
        return;

      if (onPointerEnter.GetPersistentEventCount() > 0)
      {
        onPointerEnter.Invoke();
      }

      isHovering = true;
      TransitionToOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (!interactable)
        return;

      if (onPointerExit.GetPersistentEventCount() > 0)
      {
        onPointerExit.Invoke();
      }
      isHovering = false;

      if (!isHolding)
        TransitionToNormal();
    }

    private void OnHoldStays()
    {
      if (!interactable)
        return;
      onHoldStays.Invoke();
    }

    private void OnHoldStaysForSeconds1()
    {
      if (!interactable)
        return;
      if (onHoldStaysForSeconds.GetPersistentEventCount() > 0)
        onHoldStaysForSeconds.Invoke();
      if (transitionMode == ZYButtonTransitionMode.Animation)
        animator.SetTrigger("HoldForSeconds1");
    }

    private void OnHoldStaysForSeconds2()
    {
      if (!interactable)
        return;
      if (onHoldStaysForSeconds2.GetPersistentEventCount() > 0)
        onHoldStaysForSeconds2.Invoke();
      if (transitionMode == ZYButtonTransitionMode.Animation)
        animator.SetTrigger("HoldForSeconds2");
    }

    private void OnInteractabilityChange()
    {
      if (interactable)
        TransitionToNormal();
      else
        TransitionToDisabled();
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// 获取此ZYButton 当前的按住时间。如果未被按住，则返回 0。
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHoldTime()
    {
      return isHolding ? (float)ZYCommonTimer.Instance.GetTimerValue(hash) : 0;
    }

    /// <summary>
    /// Add an action to an ZYButton event.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void AddListener(ZYButtonEventType type, UnityAction action)
    {
      switch (type)
      {
        case ZYButtonEventType.OnPressed:
          onPress.AddListener(action);
          break;
        case ZYButtonEventType.OnHoldStays:
          onHoldStays.AddListener(action);
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds:
          onHoldStaysForSeconds.AddListener(action);
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds2:
          onHoldStaysForSeconds2.AddListener(action);
          break;
        case ZYButtonEventType.OnHoldUp:
          onHoldUp.AddListener(action);
          break;
        case ZYButtonEventType.OnHoldUp2:
          onHoldUp2.AddListener(action);
          break;
        case ZYButtonEventType.OnPointerEnter:
          onPointerEnter.AddListener(action);
          break;
        case ZYButtonEventType.OnpointerExit:
          onPointerExit.AddListener(action);
          break;
        case ZYButtonEventType.OnPointerUp:
          onPointerUp.AddListener(action);
          break;
        case ZYButtonEventType.OnPointerDown:
          onPointerDown.AddListener(action);
          break;
        default:
          break;
      }
    }
    /// <summary>
    /// Remove an action from an ZYButton event.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void RemoveListener(ZYButtonEventType type, UnityAction action)
    {
      switch (type)
      {
        case ZYButtonEventType.OnPressed:
          onPress.RemoveListener(action);
          break;
        case ZYButtonEventType.OnHoldStays:
          onHoldStays.RemoveListener(action);
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds:
          onHoldStaysForSeconds.RemoveListener(action);
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds2:
          onHoldStaysForSeconds2.RemoveListener(action);
          break;
        case ZYButtonEventType.OnHoldUp:
          onHoldUp.RemoveListener(action);
          break;
        case ZYButtonEventType.OnHoldUp2:
          onHoldUp2.RemoveListener(action);
          break;
        case ZYButtonEventType.OnPointerEnter:
          onPointerEnter.RemoveListener(action);
          break;
        case ZYButtonEventType.OnpointerExit:
          onPointerExit.RemoveListener(action);
          break;
        case ZYButtonEventType.OnPointerUp:
          onPointerUp.RemoveListener(action);
          break;
        case ZYButtonEventType.OnPointerDown:
          onPointerDown.RemoveListener(action);
          break;
        default:
          break;
      }
    }
    /// <summary>
    /// Remove all actions from an ZYButton event.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void RemoveAllListeners(ZYButtonEventType type)
    {
      switch (type)
      {
        case ZYButtonEventType.OnPressed:
          onPress.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnHoldStays:
          onHoldStays.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds:
          onHoldStaysForSeconds.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnHoldStaysForSeconds2:
          onHoldStaysForSeconds2.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnHoldUp:
          onHoldUp.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnHoldUp2:
          onHoldUp2.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnPointerEnter:
          onPointerEnter.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnpointerExit:
          onPointerExit.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnPointerUp:
          onPointerUp.RemoveAllListeners();
          break;
        case ZYButtonEventType.OnPointerDown:
          onPointerDown.RemoveAllListeners();
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Set the content of the button text.
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
      buttonText.text = text;
    }

    // public void UpdateText(int localID)
    // {
    //   buttonText.UpdateLocalID(localID);
    // }

    public void FadeIn()
    {
      ZYCore.FixedTick000 += FadeInCR;
      ZYCore.FixedTick000 -= FadeOutCR;
    }

    public void FadeOut()
    {
      ZYCore.FixedTick000 += FadeOutCR;
      ZYCore.FixedTick000 -= FadeInCR;
    }

    void FadeInCR()
    {
      float delta = 0.2f;
      canvasGroup.alpha += delta;
      if (Mathf.Abs(canvasGroup.alpha - 1) < 0.01f)
      {
        canvasGroup.alpha = 1;
        ZYCore.FixedTick000 -= FadeInCR;
      }
    }
    void FadeOutCR()
    {
      float delta = -0.2f;
      canvasGroup.alpha += delta;
      if (Mathf.Abs(canvasGroup.alpha) < 0.01f)
      {
        canvasGroup.alpha = 0;
        ZYCore.FixedTick000 -= FadeOutCR;
      }
    }

    #endregion

    public enum ZYButtonTransitionMode
    {
      ColorImageOnly,
      ColorTextOnly,
      ColorImageAndText,
      Animation,
      None
    }

    public enum ZYButtonVisualState
    {
      Normal,
      Over,
      Pressed,
      Disabled
    }
  }

  public enum ZYButtonEventType
  {
    OnPressed,
    OnHoldStays,
    OnHoldStaysForSeconds,
    OnHoldStaysForSeconds2,
    OnHoldUp,
    OnHoldUp2,
    OnPointerEnter,
    OnpointerExit,
    OnPointerUp,
    OnPointerDown
  }

  [Serializable] public class ZYButtonEvent : UnityEvent { }
}
