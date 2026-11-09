using UnityEngine;

public class KeyPickup : Interactable
{
    [SerializeField] string keyId = "main_door";
    [SerializeField] string displayName = "钥匙";

    [SerializeField] AudioSource pickupAudio;

    bool collected;

    public override bool CanInteract => base.CanInteract && !collected;
    public override string Prompt => $"拾取{displayName}";

    public override void Interact(PlayerInteraction player)
    {
        if (!CanInteract)
            return;

        if (!player.Inventory.AddKey(keyId))
        {
            player.ShowMessage("钥匙未配置 Key Id");
            return;
        }

        pickupAudio.Play();

        collected = true;
        player.ShowMessage($"已拾取{displayName}");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
