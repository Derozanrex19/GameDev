using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private bool lockCursorOnStart = true;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float topClamp = 85f;
    [SerializeField] private float bottomClamp = -85f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float groundedOffset = -0.14f;
    [SerializeField] private float groundedRadius = 0.35f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraPivot == null)
        {
            Transform pivotChild = transform.Find("CameraPivot");
            if (pivotChild != null)
            {
                cameraPivot = pivotChild;
            }
            else if (Camera.main != null)
            {
                cameraPivot = Camera.main.transform;
            }
        }

        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        if (lockCursorOnStart && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        ReadInput();
        GroundCheck();
        HandleLook();
        HandleMovement();
    }

    private void ReadInput()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 move = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) move.y += 1f;
            if (Keyboard.current.sKey.isPressed) move.y -= 1f;
            if (Keyboard.current.dKey.isPressed) move.x += 1f;
            if (Keyboard.current.aKey.isPressed) move.x -= 1f;
        }
        MoveInput = move.normalized;
        IsSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
#else
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        IsSprinting = Input.GetKey(KeyCode.LeftShift);
#endif
    }

    private void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y - groundedOffset,
            transform.position.z
        );

        IsGrounded = Physics.CheckSphere(
            spherePosition,
            groundedRadius,
            groundLayers,
            QueryTriggerInteraction.Ignore
        );

        if (IsGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
    }

    private void HandleLook()
    {
        if (cameraPivot == null)
        {
            return;
        }

        Vector2 lookDelta = ReadLookInput();

        float mouseX = lookDelta.x * mouseSensitivity;
        float mouseY = lookDelta.y * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);

        cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float currentSpeed = IsSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = (transform.right * MoveInput.x + transform.forward * MoveInput.y) * currentSpeed;

#if ENABLE_INPUT_SYSTEM
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
#endif

        if (jumpPressed && IsGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private Vector2 ReadLookInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.delta.ReadValue();
        }

        if (Pointer.current != null)
        {
            return Pointer.current.delta.ReadValue();
        }

        return Vector2.zero;
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? new Color(0f, 1f, 0f, 0.35f) : new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius
        );
    }
}
