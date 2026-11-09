using System.Collections;
using UnityEngine;

public class DoorController : Interactable
{
    [SerializeField] string requiredKeyId = "main_door";
    [SerializeField] string requiredKeyName = "钥匙";
    [SerializeField] Transform hinge;
    [SerializeField] float openAngle = 90f;
    [SerializeField, Min(0.01f)] float openDuration = 0.8f;

    bool opened;
    bool opening;

    public override bool CanInteract => base.CanInteract && !opened && !opening;
    public override string Prompt => "开门";

    public override void Interact(PlayerInteraction player)
    {
        if (!CanInteract)
            return;

        if (hinge == null || string.IsNullOrWhiteSpace(requiredKeyId))
        {
            player.ShowMessage("门未配置 Hinge 或 Required Key Id");
            return;
        }

        if (!player.Inventory.TryConsumeKey(requiredKeyId))
        {
            player.ShowMessage($"需要{requiredKeyName}");
            return;
        }

        opening = true;
        player.ShowMessage($"已使用{requiredKeyName}，正在开门");
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        Quaternion start = hinge.localRotation;
        Quaternion end = start * Quaternion.Euler(0f, openAngle, 0f);
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, openDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hinge.localRotation = Quaternion.Slerp(start, end,
                Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration)));
            yield return null;
        }

        hinge.localRotation = end;
        opened = true;
        opening = false;
    }
}
