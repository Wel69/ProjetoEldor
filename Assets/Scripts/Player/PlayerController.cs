using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public LayerMask groundMask;
    public AimCursor cursorController;
    public ExplorationMovement explorationAnim; // NOVO

    [Header("Ledge Check")]
    public Transform groundCheckPoint;
    public float ledgeCheckDistance = 0.5f;
    public float raycastLength = 1.2f;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Ledge Control")]
    public bool disableLedgeCheck = false;


    // Input
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction aimAction;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 mousePosition;
    private bool isAiming;


    private bool isJumping;
    private bool isSliding;
    private bool isCrouching = false;
    private bool canSlide = true;
    private float verticalVelocity = 0f;

    public float jumpForce = 5f;
    private float jumpCooldown = 0f;
    public float gravity = -9.81f;

    [SerializeField] private VaultSystem vaultSystem;
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Bind actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        aimAction = playerInput.actions["AimMode"];

        var jumpAction = playerInput.actions["Jump"];
        jumpAction.performed += ctx => TryJump();

        var slideAction = playerInput.actions["Slide"];
        slideAction.performed += ctx => TrySlideOrCrouch();
    }

    void OnEnable()
    {
        aimAction.started += OnAimStarted;
        aimAction.canceled += OnAimCanceled;
    }

    void OnDisable()
    {
        aimAction.started -= OnAimStarted;
        aimAction.canceled -= OnAimCanceled;
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        mousePosition = Mouse.current.position.ReadValue();

        HandleMovement();
        HandleRotation();
        HandleAnimation(); // NOVO
        ApplyGravity();
        UpdateAnimatorStates();
    }

    public void ForceJumpCooldown(float duration)
    {
        jumpCooldown = duration;
    }

    private void HandleMovement()
    {
        if (!controller.enabled) return;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDir = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f) * moveDir;

        // 🔥 Check ledge na direção que tá andando
        Vector3 checkDir = new Vector3(moveDir.x, 0, moveDir.z).normalized;

        if (checkDir != Vector3.zero)
        {
            Vector3 checkOrigin = groundCheckPoint.position + checkDir * ledgeCheckDistance;

            if (!Physics.Raycast(checkOrigin, Vector3.down, raycastLength, groundMask))
            {
                // 🔥 Está na borda → bloqueia avanço nessa direção
                if (Vector3.Dot(checkDir, moveDir) > 0.9f)
                {
                    moveDir = Vector3.ProjectOnPlane(moveDir, checkDir);
                }

                Debug.DrawRay(checkOrigin, Vector3.down * raycastLength, Color.red);
            }
            else
            {
                Debug.DrawRay(checkOrigin, Vector3.down * raycastLength, Color.green);
            }
        }

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (isAiming)
        {
            if (lookInput.sqrMagnitude > 0.1f)
            {
                Vector3 aimDirection = new Vector3(lookInput.x, 0, lookInput.y);
                float targetAngle = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
                {
                    Vector3 dir = hit.point - transform.position;
                    dir.y = 0;
                    if (dir.sqrMagnitude > 0.1f)
                    {
                        Quaternion rot = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            if (moveInput.sqrMagnitude > 0.1f)
            {
                Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

                Vector3 cameraForward = mainCamera.transform.forward;
                Vector3 cameraRight = mainCamera.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;

                Vector3 moveWorld = cameraForward.normalized * inputDir.z + cameraRight.normalized * inputDir.x;

                if (moveWorld.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveWorld);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void HandleAnimation()
    {
        if (explorationAnim != null)
        {
            float speed = moveInput.sqrMagnitude;
            explorationAnim.SetSpeed(speed);
        }
    }

    private void OnAimStarted(InputAction.CallbackContext context)
    {
        isAiming = true;
        if (cursorController != null)
            cursorController.SetAimState(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        isAiming = false;
        if (cursorController != null)
            cursorController.SetAimState(false);
    }

    public void TryJump()
    {
    }

    private void EndJump()
    {
        isJumping = false;
        if (explorationAnim != null)
            explorationAnim.SetJumping(false);
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (explorationAnim != null)
            explorationAnim.SetCrouching(isCrouching);
    }

    private void TrySlideOrCrouch()
    {
        if (controller.isGrounded && !isSliding && canSlide && moveInput.sqrMagnitude > 0.5f)
        {
            isSliding = true;
            canSlide = false;

            if (explorationAnim != null)
                explorationAnim.SetSliding(true);

            Invoke(nameof(EndSlide), 1f);
            return;
        }

        ToggleCrouch();
    }

    private void EndSlide()
    {
        isSliding = false;
        if (explorationAnim != null)
            explorationAnim.SetSliding(false);

        Invoke(nameof(ResetSlideCooldown), 2f);
    }

    private void ResetSlideCooldown() => canSlide = true;

    private void ApplyGravity()
    {
        if (!controller.enabled) return;

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 gravityMove = new Vector3(0, verticalVelocity, 0);
        controller.Move(gravityMove * Time.deltaTime);
    }

    private void UpdateAnimatorStates()
    {
        float speed = moveInput.sqrMagnitude;
        if (explorationAnim != null)
        {
            explorationAnim.SetSpeed(speed);
            explorationAnim.SetJumping(isJumping);
            explorationAnim.SetSliding(isSliding);
        }
    }
}
