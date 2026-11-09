using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimFadeController : MonoBehaviour
{
    public GameObject fadeOutImage; // 拖入变黑的Image
    public GameObject fadeInImage;  // 拖入变亮的Image
    public float animDuration = 1.0f; // 必须和你做动画的时长一模一样

    void Start()
    {
        // 开局亮屏
        if (fadeInImage != null) StartCoroutine(PlayFadeIn());
    }

    // 供外部调用：准备死亡或通关跳转
    public void TriggerFadeAndLoad(string sceneName)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    private IEnumerator PlayFadeIn()
    {
        fadeInImage.SetActive(true);
        yield return new WaitForSeconds(animDuration); // 等亮屏动画播完
        fadeInImage.SetActive(false); // 关掉它，省性能
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        // 1. 开启黑屏动画
        fadeOutImage.SetActive(true);

        // 2. 死死等 1 秒钟（等屏幕完全变黑）
        yield return new WaitForSeconds(animDuration);

        // 3. 屏幕黑了，再切场景
        SceneManager.LoadScene(sceneName);
    }
}