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

    [Header("턱 오르기 설정")]

    [SerializeField] private float stepMaxHeight = 0.4f;        // 올라갈 수 있는 턱 최대 높이
    [SerializeField] private float stepMinHeight = 0.05f;       // 이보다 낮은 이음새는 무시
    [SerializeField] private float stepCheckDistance = 0.3f;    // 최소 감지 거리 (속도가 빠르면 자동으로 더 길어진다)

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

    private readonly RaycastHit[] castHits = new RaycastHit[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // 리지드바디 기본 설정
        rb.useGravity = true;                                       // 리지드바디 중력 사용
        rb.constraints = RigidbodyConstraints.FreezeRotation;       // 캐릭터가 넘어지지 않게 회전 고정
        rb.interpolation = RigidbodyInterpolation.Interpolate;      // 움직임을 부드럽게
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 마찰 0: 문턱 모서리에 걸리지 않게
        PhysicsMaterial noFriction = new PhysicsMaterial("NoFriction");
        noFriction.dynamicFriction = 0f;
        noFriction.staticFriction = 0f;
        noFriction.frictionCombine = PhysicsMaterialCombine.Minimum;
        capsule.material = noFriction;

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

        // 점프키로 점프한 게 아니면 위로 가는 속도를 없앤다 (턱에서 튕기는 것 방지)
        if (!isJumpMotion && velocity.y > 0f)
        {
            velocity.y = 0f;
        }

        rb.linearVelocity = velocity;

        // 중력 배율 적용: 1보다 크면 더 세게, 1보다 작으면 더 약하게 (우주 느낌)
        if (!Mathf.Approximately(gravityMultiplier, 1f))
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }

        TryStepUp();

        // 회전은 수평 방향만 사용
        Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // 앞에 낮은 턱이 있으면 턱 높이를 재서 부딪히기 전에 한 번에 올려준다
    private void TryStepUp()
    {
        if (!isGrounded || isJumpMotion) return;

        Vector3 dir = new Vector3(moveDirection.x, 0f, moveDirection.z);
        if (dir.sqrMagnitude < 0.001f) return;
        dir.Normalize();

        Bounds b = capsule.bounds;
        float feetY = b.min.y;
        float radius = Mathf.Max(b.extents.x, b.extents.z);

        // 한 번에 움직이는 거리보다 멀리 본다 (속도가 빠를수록 더 멀리)
        float speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        float dist = radius + Mathf.Max(stepCheckDistance, speed * Time.fixedDeltaTime * 1.5f);

        Vector3 low = new Vector3(b.center.x, feetY + stepMinHeight, b.center.z);
        Vector3 high = new Vector3(b.center.x, feetY + stepMaxHeight, b.center.z);

        RaycastHit lowHit;
        RaycastHit highHit;
        RaycastHit topHit;

        // 발 근처는 막혀 있고
        if (!CastIgnoreSelf(low, dir, dist, out lowHit)) return;

        // 완만한 경사는 그냥 올라가면 되니까 무시
        if (lowHit.normal.y > 0.7f) return;

        // 최대 높이 위쪽은 비어 있어야 올라갈 수 있는 턱이다
        if (CastIgnoreSelf(high, dir, dist, out highHit)) return;

        // 턱 윗면 높이 재기: 턱 바로 안쪽 위에서 아래로 쏜다
        Vector3 topOrigin = lowHit.point + dir * 0.05f;
        topOrigin.y = feetY + stepMaxHeight;

        if (!CastIgnoreSelf(topOrigin, Vector3.down, stepMaxHeight, out topHit)) return;

        float rise = topHit.point.y - feetY;

        if (rise > stepMinHeight && rise <= stepMaxHeight)
        {
            rb.position += Vector3.up * (rise + 0.01f);
        }
    }

    // 자기 자신(자식 포함)은 빼고 가장 가까운 것을 찾는다
    private bool CastIgnoreSelf(Vector3 origin, Vector3 dir, float dist, out RaycastHit nearest)
    {
        nearest = default(RaycastHit);
        bool found = false;
        float best = float.MaxValue;

        int count = Physics.RaycastNonAlloc(origin, dir, castHits, dist, groundLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            if (castHits[i].collider.transform.IsChildOf(transform)) continue;

            if (castHits[i].distance < best)
            {
                best = castHits[i].distance;
                nearest = castHits[i];
                found = true;
            }
        }

        return found;
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