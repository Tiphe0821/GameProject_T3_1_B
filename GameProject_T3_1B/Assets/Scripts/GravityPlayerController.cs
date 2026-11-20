using UnityEngine;
using UnityEngine.InputSystem;

// PlayerState 열거형은 PlayerController.cs 안에 이미 있어서 여기서는 다시 만들지 않는다.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class GravityPlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform cameraTransform;

    [Header("이동 설정")]

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("점프 / 중력 설정")]

    [SerializeField] private float jumpHeight = 2f;             // 점프 높이 (월드 단위)
    [SerializeField] private float gravityMultiplier = 2f;      // 중력 배율 (1 = 기본, 1보다 작으면 우주처럼 둥실, 클수록 빨리 떨어진다)
    [SerializeField] private float groundCheckDistance = 0.15f; // 발 밑 바닥 감지 거리
    [SerializeField] private LayerMask groundLayers = ~0;       // 바닥으로 인식할 레이어 (기본: 전부)

    private static readonly int SpeedHash = Animator.StringToHash("speed");
    private static readonly int GroundedHash = Animator.StringToHash("isGrounded");

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private PlayerState currentState = PlayerState.Normal;

    private Vector3 moveDirection;
    private bool isRunning;
    private bool jumpRequested;
    private bool isGrounded;                // 실제 바닥 판정 (점프 가능 여부 확인용)
    private bool isJumpMotion;              // 점프키로 점프한 상태인지 (점프 모션 재생용)
    private bool hasGroundedParam;          // Animator에 isGrounded 파라미터가 있는지
    private float groundCheckLockTimer;     // 점프 직후 잠깐 바닥 판정을 쉬는 시간

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // 리지드바디 기본 설정
        rb.useGravity = true;                                       // 리지드바디 중력 사용
        rb.constraints = RigidbodyConstraints.FreezeRotation;       // 캐릭터가 넘어지지 않게 회전 고정
        rb.interpolation = RigidbodyInterpolation.Interpolate;      // 움직임을 부드럽게
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Inspector에 프리팹 원본이 들어가 있어도 문제없도록,
        // 항상 씬에 있는 자식 오브젝트의 Animator를 찾아서 사용한다.
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("GravityPlayerController: 자식에서 Animator를 찾지 못했어요! 모델(Astraveler_Hu)이 Player 아래에 있는지 확인해주세요.");
        }
        else
        {
            // Animator에 isGrounded 파라미터가 있을 때만 값을 보낸다 (없어도 경고가 나지 않게)
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.nameHash == GroundedHash)
                {
                    hasGroundedParam = true;
                    break;
                }
            }
        }

        if (cameraTransform == null)
        {
            Debug.LogError("GravityPlayerController: cameraTransform이 비어 있어요! 인스펙터에서 카메라를 연결해주세요.");
        }
    }

    private void OnInteract(InputValue value)
    {

    }

    // 입력은 Update에서 받는다
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (currentState == PlayerState.Normal)
        {
            HandleInput(keyboard);
        }
        else
        {
            // Normal 상태가 아니라면 이동 입력을 받지 않는다
            moveDirection = Vector3.zero;
            isRunning = false;
            jumpRequested = false;
        }

        UpdateAnimation();
    }

    // 물리 이동은 FixedUpdate에서 한다
    private void FixedUpdate()
    {
        CheckGround();

        Vector3 velocity = rb.linearVelocity;

        // 수평 이동 (위/아래 속도는 중력과 점프가 담당한다)
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 horizontal = moveDirection * currentSpeed;
        velocity.x = horizontal.x;
        velocity.z = horizontal.z;

        // 점프: 바닥에 있을 때만
        if (jumpRequested && isGrounded)
        {
            // 중력이 약할수록 같은 높이에 필요한 힘이 줄어든다
            float gravityForce = Mathf.Max(0.01f, -Physics.gravity.y * gravityMultiplier);
            velocity.y = Mathf.Sqrt(2f * jumpHeight * gravityForce);

            // 점프 직후에는 아직 발이 땅 근처라서 바닥으로 판정되는 걸 막는다
            isGrounded = false;
            groundCheckLockTimer = 0.1f;

            // 점프키로 실제 점프했을 때만 점프 모션을 켠다
            isJumpMotion = true;
        }
        jumpRequested = false;

        rb.linearVelocity = velocity;

        // 중력 배율 적용: 1보다 크면 더 세게, 1보다 작으면 더 약하게 (우주 느낌)
        if (!Mathf.Approximately(gravityMultiplier, 1f))
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }

        // 회전은 수평 방향만 사용
        Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleInput(Keyboard keyboard)
    {
        if (cameraTransform == null) return;

        // 1. 입력 구현 (상하 이동 Space / Ctrl 은 삭제했다)
        Vector3 input = Vector3.zero;

        if (keyboard.aKey.isPressed)
            input.x -= 1f;
        if (keyboard.dKey.isPressed)
            input.x += 1f;
        if (keyboard.sKey.isPressed)
            input.z -= 1f;
        if (keyboard.wKey.isPressed)
            input.z += 1f;

        input = Vector3.ClampMagnitude(input, 1f);

        // 카메라가 바라보는 방향 기준으로 수평 방향만 뽑기
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * input.z + cameraRight * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        isRunning = keyboard.leftShiftKey.isPressed;

        // 점프 입력 (Space)
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void CheckGround()
    {
        // 점프 직후에는 잠깐 바닥 판정을 쉰다
        if (groundCheckLockTimer > 0f)
        {
            groundCheckLockTimer -= Time.fixedDeltaTime;
            isGrounded = false;
            return;
        }

        // 캡슐 중심에서 아래로 레이를 쏜다 (자기 자신의 콜라이더 안에서 시작하면 자기 자신은 맞지 않는다)
        Bounds bounds = capsule.bounds;
        float distance = bounds.extents.y + groundCheckDistance;

        isGrounded = Physics.Raycast(
            bounds.center,
            Vector3.down,
            distance,
            groundLayers,
            QueryTriggerInteraction.Ignore);

        // 점프 후 착지하면 점프 모션을 끈다
        if (isGrounded)
        {
            isJumpMotion = false;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        //8. Idle, Walk, Run 애니메이션
        float animationSpeed = 0f;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            animationSpeed = isRunning ? 1f : 0.5f;
        }

        animator.SetFloat(SpeedHash, animationSpeed, 0.1f, Time.deltaTime);

        // 점프 모션: 점프키로 점프했을 때만 false(점프 모션), 그 외에는 true(지상 모션)
        // 언덕/계단/낭떠러지처럼 그냥 공중에 뜬 경우에는 점프 모션이 나오지 않는다
        if (hasGroundedParam)
        {
            animator.SetBool(GroundedHash, !isJumpMotion);
        }
    }

    public void ChangeState(PlayerState newState)
    {
        currentState = newState;

        Debug.Log("현재 상태 : " + currentState);
    }

}