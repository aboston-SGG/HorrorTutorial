using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float maxMoveSpeed = 2.2f;
    [SerializeField, Min(0f)] private float acceleration = 6f;
    [SerializeField, Min(0f)] private float deceleration = 8f;
    [SerializeField, Min(0f)] private float rotationSpeed = 540f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;

    private const float GroundedVerticalSpeed = -2f;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Using OnEnable instead of Start to subscribe to events, it ensures that subscription happens before any Update calls.
    private void OnEnable()
    {
        if(inputReader == null)
        {
            Debug.LogError("Assign a PlayerInputReader asset.", this);
            enabled = false;
            return;
        }

        inputReader.MoveEvent += OnMove;
        inputReader.EnableInput();
    }

    // Using OnDisable to unsubscribe from events and reset movement variables, ensuring that the player doesn't continue moving when the script is disabled.
    private void OnDisable()
    {
        if(inputReader != null)
        {
            inputReader.MoveEvent -= OnMove;
            inputReader.DisableInput();
        }

        moveInput = Vector2.zero;
        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHorizontalMovement();
        ApplyGravity();

        Vector3 frameVelocity = horizontalVelocity;
        frameVelocity += Vector3.up * verticalVelocity;
        controller.Move(frameVelocity * Time.deltaTime);
        RotateTowardsMovement();
    }

    // OnMove is called when the MoveEvent is invoked, it updates the moveInput variable with the new input value.
    private void OnMove(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    // UpdateHorizontalMovement calculates the target horizontal velocity based on the moveInput and smoothly transitions towards it using acceleration or deceleration.
    private void UpdateHorizontalMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 targetVelocity = direction * maxMoveSpeed;

        bool hasMoveInput = moveInput.sqrMagnitude > 0f;

        float changeRate = hasMoveInput ? acceleration : deceleration;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, changeRate * Time.deltaTime);
    }

    // ApplyGravity modifies the verticalVelocity based on whether the player is grounded or in the air, applying gravity when airborne and resetting to a small downward speed when grounded.
    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = GroundedVerticalSpeed;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    // RotateTowardsMovement rotates the player to face the direction of movement, using Quaternion.LookRotation to determine the target rotation and Quaternion.RotateTowards to smoothly rotate towards it.
    private void RotateTowardsMovement()
    {
        if(horizontalVelocity.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
