using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RunningAduio : MonoBehaviour
{
    [Header("音效")]
    [Tooltip("脚步声素材。留空不会播放。")]
    [SerializeField] AudioClip footstepSound;

    [Tooltip("音量。")]
    [SerializeField, Range(0f, 1f)] float volume = 1f;

    [Header("触发")]
    [Tooltip("按住这个键才播放，默认 W。")]
    [SerializeField] KeyCode moveKey = KeyCode.W;

    AudioSource audioSource;
    CharacterController controller;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;   
        audioSource.loop = false;          

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!Input.GetKey(moveKey))
        {
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        if (controller != null && !controller.isGrounded) return;

        if (footstepSound == null) return;
        if (audioSource.isPlaying) return;

        audioSource.clip = footstepSound;
        audioSource.volume = volume;
        audioSource.Play();
    }

    void OnDisable()
    {
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }
}
