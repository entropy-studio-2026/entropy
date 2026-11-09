using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ExitFadeTrigger : MonoBehaviour
{
    [SerializeField] AnimFadeController fadeController;

    bool triggered;

    void Reset()
    {
        ConfigureTrigger();
    }

    void Awake()
    {
        ConfigureTrigger();
    }

    void ConfigureTrigger()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered || other.GetComponentInParent<PlayerInventory>() == null)
            return;

        if (fadeController == null)
        {
            Debug.LogError("ExitFadeTrigger: 请在 Inspector 中配置 Fade Controller。", this);
            enabled = false;
            return;
        }

        triggered = true;
        fadeController.FadeOut();
    }
}
