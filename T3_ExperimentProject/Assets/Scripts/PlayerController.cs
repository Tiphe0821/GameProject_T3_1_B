using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Normal,
    Pickup
}

public class PlayerController : MonoBehaviour
{
    // [SerializeField] private Animator animator;

    [SerializeField] private Transform cameraTransform;

    [Header("이동 설정")]

    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 6f;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private PlayerState currentState = PlayerState.Normal;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnInteract(InputValue value)
    {

    }

    // 눈앞에 들어온 물체 인식 -> 아이템 상호작용 (허준 교수님 코드 참고해보자)


    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        // 상태 상관없이 적용되는 중력 // 잠시 꺼보자.
        //ApplyGravity();

        // Normal 상태가 아니라면 이동 입력을 받지 않는다
        if (currentState != PlayerState.Normal) return;

        HandleMovement(keyboard);

    }

    private void HandleMovement(Keyboard keyboard)      // On 대신 쓰이는 것
    {
        // 1. 입력 구현
        Vector3 input = Vector3.zero;

        if (keyboard.aKey.isPressed)
            input.x -= 1f;
        if (keyboard.dKey.isPressed)
            input.x += 1f;
        if (keyboard.sKey.isPressed)
            input.z -= 1f;
        if (keyboard.wKey.isPressed)
            input.z += 1f;
        if (keyboard.leftCtrlKey.isPressed)
            input.y -= 1f;
        if(keyboard.spaceKey.isPressed)
            input.y += 1f;
            

        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        Vector3 cameraUp = cameraTransform.up;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraUp.x = 0;
        cameraUp.z = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();
        cameraUp.Normalize();

        Vector3 moveDirection = cameraForward * input.z + cameraRight * input.x + cameraUp* input.y;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        /*
        bool isRunning = keyboard.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        */
        float currentSpeed = walkSpeed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.001)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }



        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);


        //float animationSpeed = 0f;
        
        /*
        if (moveDirection.sqrMagnitude > 0.001)
        {
            animationSpeed = isRunning ? 1f : 0.5f;
        }
        */

        // animator.SetFloat("Speed", animationSpeed, 0.1f, Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    /*
    public void ChangeState(PlayerState newState)
    {
        currentState = newState;

        if (currentState != PlayerState.Normal)
        {
            animator.SetFloat("speed", 0);

        }

        Debug.Log("현재 상태 : " + currentState);
    }
    */
}