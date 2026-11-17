using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemInteraction : MonoBehaviour
{
    [Header("들고 있는 아이템")]
    [SerializeField] private string heldItemId = "";          // 핫바 완성 전 임시: 직접 입력해서 테스트. 핫바가 생기면 SetHeldItem으로 갱신

    [Header("아이템 효과 스크립트")]
    [SerializeField] private PlayerOxygen oxygen;
    [SerializeField] private PlayerImmunityBoost immunity;

    [Header("줍기와 충돌 방지")]
    [SerializeField] private float detectionRadius = 2f;      // PlayerPickup의 detectionRadius와 같은 값으로 맞출 것

    public string HeldItemId => heldItemId;

    private void Awake()
    {
        if (oxygen == null) oxygen = GetComponent<PlayerOxygen>();
        if (immunity == null) immunity = GetComponent<PlayerImmunityBoost>();
    }

    // 핫바에서 선택 슬롯이 바뀔 때 호출해 준다 (빈 칸이면 null 또는 "")
    public void SetHeldItem(string itemId)
    {
        heldItemId = itemId;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.eKey.wasPressedThisFrame) return;

        // 근처에 줍거나 열 수 있는 대상이 있으면 PlayerPickup에게 양보한다
        if (HasInteractableNearby()) return;

        UseHeldItem();
    }

    private void UseHeldItem()
    {
        if (string.IsNullOrEmpty(heldItemId)) return;

        // 한 번에 하나만 사용되도록, 성공하면 바로 종료한다
        if (oxygen != null && oxygen.TryUse(heldItemId)) return;
        if (immunity != null && immunity.TryUse(heldItemId)) return;
    }

    private bool HasInteractableNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider col in colliders)
        {
            if (col.GetComponentInParent<PickupItem>() != null) return true;
            if (col.GetComponentInParent<Door>() != null) return true;
        }

        return false;
    }
}