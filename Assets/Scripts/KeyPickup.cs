using UnityEngine;

public class KeyPickup : Interactable
{
    [SerializeField] string keyId = "main_door";
    [SerializeField] string displayName = "钥匙";

    [Header("音效")]
    [Tooltip("拾取瞬间播放的音效。留空则不播。")]
    [SerializeField] AudioClip pickupClip;

    [Tooltip("拾取音效的音量。")]
    [SerializeField, Range(0f, 1f)] float pickupVolume = 0.8f;

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

        // 真正捡到的这一瞬间才播放
        PlayPickupSound();

        collected = true;
        player.ShowMessage($"已拾取{displayName}");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// 用一个独立的 2D 音源播放，播完自动销毁。
    /// 不能把 AudioSource 挂在钥匙自己身上：下面会 SetActive(false) + Destroy(gameObject)，
    /// 声音会被立刻掐断。
    /// </summary>
    void PlayPickupSound()
    {
        if (pickupClip == null)
            return;

        GameObject audioObject = new GameObject("KeyPickupSFX");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = pickupClip;
        source.volume = pickupVolume;
        source.spatialBlend = 0f;   // 2D，不受玩家位置影响
        source.playOnAwake = false;
        source.Play();

        Destroy(audioObject, pickupClip.length + 0.1f);
    }
}
