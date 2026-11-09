using UnityEngine;

public class RunningAduio : MonoBehaviour
{
    [Header("音效")]
    [Tooltip("脚步声素材。留空不会播放。")]
    [SerializeField] AudioSource footstepAudio;

    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (footstepAudio == null) return;

        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (!moving || (controller != null && !controller.isGrounded))
        {
            if (footstepAudio.isPlaying) footstepAudio.Stop();
            return;
        }

        if (footstepAudio.clip == null) return;
        if (footstepAudio.isPlaying) return;

        footstepAudio.Play();
    }

    void OnDisable()
    {
        if (footstepAudio != null && footstepAudio.isPlaying) footstepAudio.Stop();
    }
}
