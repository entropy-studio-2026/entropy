using System.Collections;
using UnityEngine;

public class AnimFadeController : MonoBehaviour
{
    [SerializeField] GameObject fadeOutImage;
    [SerializeField]  GameObject fadeInImage;
    public float animDuration = 1.0f;

    public void FadeIn()
    {
        StartCoroutine(PlayFadeIn());
    }
    public void FadeOut(string sceneName)
    {
        StartCoroutine(PlayFadeOut(sceneName));
    }

    private IEnumerator PlayFadeIn()
    {
        fadeInImage.SetActive(true);
        yield return new WaitForSeconds(animDuration);
        fadeInImage.SetActive(false);
    }
    private IEnumerator PlayFadeOut(string sceneName)
    {
        fadeOutImage.SetActive(true);
        yield return new WaitForSeconds(animDuration);
    }
}