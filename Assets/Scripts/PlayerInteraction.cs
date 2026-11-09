using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Transform interactionOrigin;
    [SerializeField, Min(0.1f)] float interactionDistance = 2f;
    [SerializeField] LayerMask interactionLayers = ~0;
    [SerializeField] KeyCode interactionKey = KeyCode.E;
    [SerializeField] bool showScreenPrompt = true;

    Interactable currentTarget;
    string message;
    float messageUntil;
    GUIStyle promptStyle;

    public PlayerInventory Inventory { get; private set; }
    public string CurrentPrompt => currentTarget != null
        ? $"[{interactionKey}] {currentTarget.Prompt}" : string.Empty;

    Vector3 Origin => interactionOrigin != null
        ? interactionOrigin.position : transform.position + Vector3.up;

    void Awake()
    {
        Inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        currentTarget = FindNearestTarget();
        if (currentTarget != null && Input.GetKeyDown(interactionKey))
        {
            currentTarget.Interact(this);
            currentTarget = FindNearestTarget();
        }
    }

    Interactable FindNearestTarget()
    {
        Vector3 origin = Origin;
        Collider[] colliders = Physics.OverlapSphere(origin, interactionDistance,
            interactionLayers, QueryTriggerInteraction.Collide);
        Interactable nearest = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (Collider candidate in colliders)
        {
            if (candidate.transform.IsChildOf(transform))
                continue;

            Interactable target = candidate.GetComponentInParent<Interactable>();
            if (target == null || !target.CanInteract)
                continue;

            float distance = (candidate.ClosestPoint(origin) - origin).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = target;
            }
        }

        return nearest;
    }

    public void ShowMessage(string text)
    {
        message = text;
        messageUntil = Time.unscaledTime + 2f;
        Debug.Log(text, this);
    }

    void OnGUI()
    {
        if (!showScreenPrompt)
            return;

        string text = Time.unscaledTime < messageUntil ? message : CurrentPrompt;
        if (string.IsNullOrEmpty(text))
            return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 22,
                wordWrap = true
            };
        }

        float width = Mathf.Min(600f, Screen.width - 24f);
        GUI.Box(new Rect((Screen.width - width) / 2f, Screen.height - 100f, width, 64f),
            text, promptStyle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Origin, interactionDistance);
    }
}
