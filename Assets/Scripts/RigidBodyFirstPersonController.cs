using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyFirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float sprintSpeed = 3f;
    public float jumpForce = 5f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 400f;
    public float pitchClamp = 85f;

    [Header("Physics")]
    public float groundDrag = 5f;
    public float airDrag = 0.5f;

    Rigidbody rb;
    float pitch;
    bool grounded;
    Vector3 externalForces;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        CheckGrounded();
        HandleJump();
    }

    void FixedUpdate()
    {
        Move();
        ApplyExternalForces();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = (transform.forward * v + transform.right * h).normalized;

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        rb.AddForce(inputDir * targetSpeed * 50f * (grounded ? 1f : 0.5f), ForceMode.Force);

        rb.linearDamping = grounded ? groundDrag : airDrag;
    }

    void HandleJump()
    {
        if (grounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset vertical
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        float radius = col.radius * 0.9f;

        grounded = Physics.CheckSphere(
            col.bounds.center - new Vector3(0, col.height / 2f, 0),
            radius,
            LayerMask.GetMask("Default", "Ground"),   // adjust if needed
            QueryTriggerInteraction.Ignore
        );
    }


    void ApplyExternalForces()
    {
        if (externalForces != Vector3.zero)
        {
            rb.AddForce(externalForces, ForceMode.Acceleration);
            externalForces = Vector3.zero;
        }
    }

    public void AddExternalForce(Vector3 force)
    {
        externalForces += force;
    }
}
