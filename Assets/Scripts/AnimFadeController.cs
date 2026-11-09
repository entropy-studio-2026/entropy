using System.Collections;
using UnityEngine;

public class AnimFadeController : MonoBehaviour
{
    [SerializeField] GameObject fadeOutImage;
    [SerializeField] GameObject fadeInImage;
    [SerializeField] float animDuration = 1.0f;

    public void FadeIn()
    {
        StartCoroutine(PlayFadeIn());
    }

    public void FadeOut()
    {
        fadeOutImage.SetActive(true);
    }

    private IEnumerator PlayFadeIn()
    {
        fadeInImage.SetActive(true);
        yield return new WaitForSeconds(animDuration);
        fadeInImage.SetActive(false);
    }
}
