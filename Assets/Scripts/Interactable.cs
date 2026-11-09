using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual bool CanInteract => isActiveAndEnabled;
    public abstract string Prompt { get; }
    public abstract void Interact(PlayerInteraction player);
}
