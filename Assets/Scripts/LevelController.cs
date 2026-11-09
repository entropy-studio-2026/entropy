using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] AnimFadeController fadeController;

    void Start()
    {
        if (fadeController == null)
        {
            Debug.LogError("LevelController: 请在 Inspector 中配置 Fade Controller。", this);
            return;
        }

        fadeController.FadeIn();
    }

}
