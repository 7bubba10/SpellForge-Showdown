using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 400f;
    public float pitchClamp = 85f;

    CharacterController controller;
    float verticalVelocity;
    float pitch;

    // >>> NEW: external forces applied by spells / wind / knockback
    [Header("externals")] public Vector3 externalForce = Vector3.zero;
    public float externalForceDamping = 5f; // how fast it fades

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
        DampExternalForce();

        // Exit lock in editor
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

        pitch -= mouseY; // pitch
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (grounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Player input (WASD)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.Normalize(new Vector3(h, 0f, v));

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 move = (transform.right * input.x + transform.forward * input.z) * speed;

        // FINAL MOVE VECTOR
        Vector3 movement = move + externalForce;
        // Add external vertical force into verticalVelocity
        verticalVelocity += externalForce.y;

        // Apply to final movement
        movement.y = verticalVelocity;


        controller.Move(movement * Time.deltaTime);
    }

    // >>> NEW: slowly fade external forces so they don't last forever
    void DampExternalForce()
    {
        if (externalForce.magnitude > 0.1f)
            externalForce = Vector3.Lerp(externalForce, Vector3.zero, externalForceDamping * Time.deltaTime);
        else
            externalForce = Vector3.zero;
    }

    // >>> NEW: Call this from spells / knockback / wind zones
    public void AddExternalForce(Vector3 force)
    {
        externalForce += force;
    }
}
