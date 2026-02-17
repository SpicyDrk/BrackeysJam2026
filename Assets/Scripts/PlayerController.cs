using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 4.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundedGravity = -2f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 0.5f;
    [SerializeField] private float lookXLimit = 85f;
    [SerializeField] private bool invertY = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask = 1;

    // Components
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;

    // Camera
    private Camera mainCamera;
    private float cameraPitch = 0f;

    // State
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        mainCamera = GetComponentInChildren<Camera>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Get input actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGround();
        Move();
        Look();
        ApplyGravity();
    }

    private void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            isGrounded = controller.isGrounded;
        }
    }

    private void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool isSprinting = sprintAction.ReadValue<float>() > 0.5f;

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void Look()
    {
        if (mainCamera == null) return;

        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();

        // Horizontal rotation (player body)
        float yaw = mouseDelta.x * lookSensitivity;
        transform.Rotate(Vector3.up * yaw);

        // Vertical rotation (camera only)
        float pitch = mouseDelta.y * lookSensitivity * (invertY ? 1f : -1f);
        cameraPitch -= pitch;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookXLimit, lookXLimit);

        mainCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = groundedGravity;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
