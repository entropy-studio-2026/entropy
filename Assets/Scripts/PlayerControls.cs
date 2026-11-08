using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 playerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float playerSpeed;
    [SerializeField] float gravityValue;
    [SerializeField] GameObject activeChar;
    [SerializeField] float moveHorizontal;
    [SerializeField] float moveVertical;
    [SerializeField] float speed = 4;
    [SerializeField] float rotateSpeed = 3;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] bool isjumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpeed = 1;
        gravityValue = -20;
    }

    // Update is called once per frame
    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        controller.SimpleMove(forward * curSpeed);

        if(Input.GetButtonDown("Jump")&& groundedPlayer )
        {
            isjumping = true;
            activeChar.GetComponent<Animator>().Play("Jump");
            playerVelocity.y += 10;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.001f;
            if(isjumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Standard Run");
            }
        }
        else
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.901f;
            if (isjumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
        }
    }
}
