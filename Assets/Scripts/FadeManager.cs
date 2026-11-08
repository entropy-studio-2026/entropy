using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换用的淡入淡出管理器。
/// 注意：脚本文件名必须和类名完全一致（FadeManager.cs ↔ class FadeManager），
/// 否则 Unity 会认为这个脚本没有可用的 MonoBehaviour，Inspector 里就只剩一个空壳，
/// 连 public 字段（Fade Image）都不会显示。
///
/// 推荐层级：
/// FadeManager   ← 挂本脚本，必须在场景根节点（不能是 Canvas 的子物体）
/// └ Canvas      ← Screen Space - Overlay
///   └ Image     ← 全屏黑色，Alpha = 0，Raycast Target 关掉
/// </summary>
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("UI 设置")]
    [Tooltip("全屏黑幕 Image。留空时脚本会自动在本物体子级里查找；实在找不到会自动创建一张。")]
    public Image fadeImage;

    [Tooltip("一次淡出（变黑）或淡入（变透明）需要的时间，单位秒。")]
    [Min(0.01f)]
    public float fadeDuration = 1.0f;

    [Header("启动设置")]
    [Tooltip("勾上：进入游戏时先从黑屏淡入。不需要就取消勾选。")]
    public bool fadeInOnStart = true;

    [Tooltip("正在切换场景时，重复调用 LoadSceneWithFade 是否直接忽略（推荐勾上，防连点）。")]
    public bool ignoreRepeatedLoad = true;

    Coroutine fadeRoutine;   // 只记录淡入淡出协程，避免 StopAllCoroutines 误杀其他协程
    bool isLoading;

    /// <summary>当前是否正在渐变。</summary>
    public bool IsFading { get { return fadeRoutine != null; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // DontDestroyOnLoad 只对根物体生效；挂在 Canvas 下面会报
        // "DontDestroyOnLoad only works for root GameObjects"，黑幕也就跟着场景一起被销毁
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning(
                "[FadeManager] 这个物体不是场景根节点，无法跨场景保留。请把 FadeManager 拖到场景根节点，" +
                "再把 Canvas 作为它的子物体。", this);
        }

        ResolveFadeImage();
    }

    void Start()
    {
        if (fadeImage == null) return;

        if (fadeInOnStart)
        {
            SetAlpha(1f);
            fadeRoutine = StartCoroutine(FadeRoutine(0f));
        }
        else
        {
            SetAlpha(0f);
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ------------------------------------------------------------------
    // 对外接口
    // ------------------------------------------------------------------

    /// <summary>
    /// 淡出（变黑）→ 加载场景 → 淡入（变透明）。
    /// </summary>
    /// <param name="sceneName">要加载的场景名（需要已经在 Build Settings 里）</param>
    public void LoadSceneWithFade(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[FadeManager] 场景名为空，无法加载。", this);
            return;
        }

        if (isLoading && ignoreRepeatedLoad) return;

        StopFadeRoutine();
        fadeRoutine = StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    /// <summary>只淡入（变透明），不加载场景。</summary>
    public void FadeIn()
    {
        StopFadeRoutine();
        fadeRoutine = StartCoroutine(FadeRoutine(0f));
    }

    /// <summary>只淡出（变黑），不加载场景。</summary>
    public void FadeOut()
    {
        StopFadeRoutine();
        fadeRoutine = StartCoroutine(FadeRoutine(1f));
    }

    /// <summary>
    /// 渐变到指定透明度（0 = 透明，1 = 全黑），可用于自定义流程。
    /// 用法：StartCoroutine(fm.FadeTo(1f)) 或者 yield return fm.FadeTo(1f);
    /// </summary>
    public IEnumerator FadeTo(float targetAlpha)
    {
        yield return FadeToInternal(targetAlpha);
    }

    // ------------------------------------------------------------------
    // 内部实现
    // ------------------------------------------------------------------

    IEnumerator FadeRoutine(float targetAlpha)
    {
        yield return FadeToInternal(targetAlpha);
        fadeRoutine = null;
    }

    IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        isLoading = true;

        // 1. 变黑
        yield return FadeToInternal(1f);

        // 2. 加载场景（在黑幕下完成，玩家看不到卡顿）
        SceneManager.LoadScene(sceneName);

        // 3. 等一帧，让新场景的相机 / Canvas 初始化完
        yield return null;

        // 4. 变透明
        yield return FadeToInternal(0f);

        isLoading = false;
        fadeRoutine = null;
    }

    IEnumerator FadeToInternal(float targetAlpha)
    {
        if (fadeImage == null)
        {
            // 原来的写法是 yield break 直接静默返回，结果就是"什么都没发生"，非常难查
            Debug.LogError("[FadeManager] fadeImage 没有指定，也没有找到可用的 Image，渐变已跳过。", this);
            yield break;
        }

        targetAlpha = Mathf.Clamp01(targetAlpha);
        float duration = Mathf.Max(0.01f, fadeDuration);   // 防止 fadeDuration = 0 时除零

        fadeImage.enabled = true;
        // 黑幕不透明时要挡住点击，全透明时不要挡住 UI
        fadeImage.raycastTarget = targetAlpha > 0.001f;

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 用 unscaledDeltaTime：以后加了暂停（Time.timeScale = 0），黑幕照样能渐变
            elapsed += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
            fadeImage.color = color;

            yield return null;
        }

        // 收尾精确赋值，避免浮点误差停在 0.999
        color.a = targetAlpha;
        fadeImage.color = color;
        fadeImage.raycastTarget = targetAlpha > 0.001f;
    }

    void StopFadeRoutine()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        // 协程被强制打断时不会走到结尾，这里必须把状态复位，
        // 否则 isLoading 永远为 true，之后所有 LoadSceneWithFade 都会被忽略
        isLoading = false;
    }

    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color color = fadeImage.color;
        color.a = Mathf.Clamp01(alpha);
        fadeImage.color = color;
        fadeImage.raycastTarget = color.a > 0.001f;
    }

    /// <summary>没手动拖 Image 时的兜底：先在子级里找，找不到就自动建一张全屏黑幕。</summary>
    void ResolveFadeImage()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponentInChildren<Image>(true);
        }

        if (fadeImage == null)
        {
            fadeImage = CreateFadeImage();
            Debug.LogWarning("[FadeManager] 没有指定 Fade Image，已自动创建一张全屏黑幕。", this);
        }

        // 场景里的 Image 如果开了 Raycast Target，Alpha = 0 时也会挡住所有 UI 点击
        fadeImage.raycastTarget = fadeImage.color.a > 0.001f;
    }

    Image CreateFadeImage()
    {
        Canvas canvas = GetComponentInChildren<Canvas>(true);

        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("FadeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform, false);

            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
        }

        GameObject imageGO = new GameObject("FadeImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageGO.transform.SetParent(canvas.transform, false);

        RectTransform rect = imageGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        Image image = imageGO.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;
        return image;
    }
}
