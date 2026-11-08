using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float verticalVelocity;
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
        if (groundedPlayer && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            isjumping = false;
        }
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

        if(Input.GetButtonDown("Jump") && groundedPlayer )
        {
            isjumping = true;
            activeChar.GetComponent<Animator>().Play("Jump");
            verticalVelocity = 10f;
        }

        verticalVelocity += gravityValue * Time.deltaTime;
        Vector3 movement = transform.forward * (speed * Input.GetAxis("Vertical"));
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);

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
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.001f;
            if (isjumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
        }
    }
}
