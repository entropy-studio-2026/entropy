using UnityEngine;

public class RunningAduio : MonoBehaviour
{
    [Header("音效")]
    [Tooltip("脚步声素材。留空不会播放。")]
    [SerializeField] AudioSource footstepAudio;

    [Header("触发")]
    [Tooltip("按住这个键才播放，默认 W。")]
    [SerializeField] KeyCode moveKey = KeyCode.W;

    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (footstepAudio == null) return;

        if (!Input.GetKey(moveKey))
        {
            if (footstepAudio.isPlaying) footstepAudio.Stop();
            return;
        }

        if (controller != null && !controller.isGrounded) return;

        if (footstepAudio.clip == null) return;
        if (footstepAudio.isPlaying) return;

        footstepAudio.Play();
    }

    void OnDisable()
    {
        if (footstepAudio != null && footstepAudio.isPlaying) footstepAudio.Stop();
    }
}
