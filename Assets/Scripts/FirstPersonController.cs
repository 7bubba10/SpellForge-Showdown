using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraTransform;   // drag the child Camera here
    public float mouseSensitivity = 120f;
    public float pitchClamp = 85f;

    CharacterController controller;
    float verticalVelocity;
    float pitch; // camera up/down

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();

        // quick unlock for editor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX); // yaw

        pitch -= mouseY;                      // pitch
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        if (cameraTransform) cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (grounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.Normalize(new Vector3(h, 0f, v));

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        Vector3 move = (transform.right * input.x + transform.forward * input.z) * speed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}
