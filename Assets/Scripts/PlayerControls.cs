using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float verticalVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float playerSpeed;
    [SerializeField] float gravityValue;
    [SerializeField] GameObject activeChar;
    [SerializeField] float speed = 4;
    [SerializeField] float rotateSpeed = 3;

    [SerializeField] Transform viewCamera;
    [SerializeField, Range(-89f, 0f)] float minPitch = -75f;
    [SerializeField, Range(0f, 89f)] float maxPitch = 75f;

    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] bool isjumping;

    float cameraPitch;
    float initialCameraPitch;
    Quaternion initialCameraRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpeed = 1;
        gravityValue = -20;
        initialCameraRotation = viewCamera.localRotation;
        initialCameraPitch = Mathf.DeltaAngle(0f, viewCamera.localEulerAngles.x);
        cameraPitch = initialCameraPitch;

        controller.minMoveDistance = 0.001f;
        SetCursorLocked(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            isjumping = false;
        }

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            isjumping = true;
            activeChar.GetComponent<Animator>().Play("Jump");
            verticalVelocity = 10f;
        }

        verticalVelocity += gravityValue * Time.deltaTime;
        Vector3 direction = Vector3.ClampMagnitude(
            transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"), 1f);
        Vector3 movement = direction * speed;
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            if (isjumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Standard Run");
            }
        }
        else
        {
            if (isjumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
        }
    }

    void UpdateMouseLook()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLocked(false);
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
                SetCursorLocked(true);
            return;
        }

        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f, Space.World);

        cameraPitch = Mathf.Clamp(
            cameraPitch - Input.GetAxis("Mouse Y") * rotateSpeed,
            minPitch,
            maxPitch
        );
        viewCamera.localRotation = initialCameraRotation
            * Quaternion.Euler(cameraPitch - initialCameraPitch, 0f, 0f);
    }

    void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SetCursorLocked(false);
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }
}
