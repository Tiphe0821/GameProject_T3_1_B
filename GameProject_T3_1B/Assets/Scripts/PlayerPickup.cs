using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerPickup : MonoBehaviour
{
    // 플레이어 컨트롤러: 씬에 있는 쪽을 자동으로 찾아서 쓴다
    private PlayerController playerController;                  // CharacterController 방식 플레이어
    private GravityPlayerController gravityPlayerController;    // Rigidbody 방식 플레이어

    [SerializeField] private Animator animator;                     // 플레이어 애니메이션 재생 & 플레이어가 줍는 동안 움직이지 못하게 하는 용도
    [SerializeField] private InteractionUI interactionUI;

    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;
    [SerializeField] private string NextSceneName;

    private PickupItem targetItem;                                  // 문 말고 다른 상호작용 오브젝트가 추가될 수 있음
    private Door targetDoor;

    private bool isPickingUp;

    private void Awake()
    {
        // 둘 중 씬에 있는 컨트롤러를 찾는다 (없는 쪽은 null)
        playerController = FindAnyObjectByType<PlayerController>();
        gravityPlayerController = FindAnyObjectByType<GravityPlayerController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerController == null && gravityPlayerController == null)
        {
            Debug.LogWarning("PlayerPickup: PlayerController / GravityPlayerController를 둘 다 찾지 못했어요! 줍는 동안 이동 제한이 동작하지 않아요.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickingUp) return;

        // Player 주변에서 가장 가까운 아이템을 찾는다
        targetItem = FindClosestItem();
        targetDoor = FindClosestDoor();

        ShowGuide();

        if (targetItem == null) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 가까운 아이템이 있을 때 E키로 줍는다
        // 마우스로 줍는 건 아직 안 하고 있음.. 일단 이렇게 해놓고 나중에 바꾸는 걸로

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(NextSceneName);
    }

    // 어떤 컨트롤러를 쓰든 상태를 바꿔 준다 (컨트롤러가 없으면 아무것도 하지 않는다)
    private void SetPlayerState(PlayerState newState)
    {
        if (playerController != null)
        {
            playerController.ChangeState(newState);
        }
        else if (gravityPlayerController != null)
        {
            gravityPlayerController.ChangeState(newState);
        }
    }

    private void Interact()
    {
        if (targetDoor != null)
        {
            ChangeScene();
        }
        if (targetItem != null)
        {
            StartCoroutine(PickupAnimation());
        }
    }

    private void ShowGuide()
    {
        if (targetItem != null)
        {
            interactionUI.ShowGuide("[E] " + targetItem.itemData.name + " 줍기");
        }
    }

    private PickupItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders)
        {
            PickupItem item = itemCollider.GetComponentInParent<PickupItem>();

            if (item == null) continue;
            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

    private Door FindClosestDoor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders)
        {
            Door door = itemCollider.GetComponentInParent<Door>();

            if (door == null) continue;
            float distance = Vector3.Distance(transform.position, door.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDoor = door;
            }
        }

        return closestDoor;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        // 지금 찾은 아이템을 저장한다
        PickupItem itemToCollect = targetItem;

        bool stateChanged = false;

        // 중간에 에러가 나거나 끊겨도 finally에서 반드시 원래 상태로 돌려놓는다
        try
        {
            if (itemToCollect == null) yield break;

            /*
            // 아이템 방향으로 캐릭터를 돌려준다 - 아직은 굳이 필요 없을 것 같음.
            Vector3 itemDirection = itemToCollect.transform.position - transform.position;
            itemDirection.y = 0f;

            if (itemDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(itemDirection);
            }
            */

            if (itemToCollect.needDelay)
            {
                // 줍기 상태로 변경하여 이동을 막는다 - 나중에 주울 때 모션을 한다면
                SetPlayerState(PlayerState.Interact);
                stateChanged = true;

                // 줍기 모션으로 전환한다
                //animator.CrossFade("PickUp01", 0.1f);

                // 손이 아이템에 닿는 순간까지 기다린다
                yield return new WaitForSeconds(pickupMoment);

                // 실제로 아이템을 획득한다
                if (itemToCollect != null)
                {
                    itemToCollect.Collect();
                }

                // 남은 애니메이션 시간을 기다린다
                yield return new WaitForSeconds(Mathf.Max(0f, pickupDuration - pickupMoment));

                // 이동 BlendTree로 돌아간다 (지금은 사용 X)
                //animator.CrossFade("Blend Tree", 0.1f);
            }
            else
            {
                itemToCollect.Collect();
            }
        }
        finally
        {
            // 노멀 상태로 돌아가 이동을 허용한다
            if (stateChanged)
            {
                SetPlayerState(PlayerState.Normal);
            }

            targetItem = null;
            isPickingUp = false;
        }
    }

}