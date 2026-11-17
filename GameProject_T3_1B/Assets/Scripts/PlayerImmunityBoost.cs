using UnityEngine;
using UnityEngine.InputSystem;
using GameFramework.Gameplay;

public class PlayerImmunityBoost : MonoBehaviour
{
    [Header("면역력 증가 아이템")]
    [SerializeField] private string immunityItemId = "";      // 면역력 증가 아이템의 ItemData.id
    [SerializeField] private float increaseAmount = 40f;      // 사용 시 올라가는 면역력 수치

    // [변경] F키 입력(Update)은 제거. 이제 PlayerItemInteraction이 E키로 TryUse를 호출한다.

    // [추가] 들고 있는 아이템이 면역력 아이템일 때만 사용한다. 사용했으면 true
    public bool TryUse(string heldItemId)
    {
        if (string.IsNullOrEmpty(immunityItemId)) return false;
        if (heldItemId != immunityItemId) return false;
        return BoostImmunity();
    }

    private bool BoostImmunity()
    {
        PlayerStatus status = PlayerStatus.Instance;

        // 면역력(방사선 수치)이 이미 가득 차 있으면 아이템을 소모하지 않는다
        if (status.CurrentRadiation >= status.MaxRadiation) return false;

        if (InventoryManager.Instance.GetCount(immunityItemId) > 0)
        {
            InventoryManager.Instance.RemoveItem(immunityItemId);
            status.ModifyRadiation(increaseAmount);   // 양수로 넘겨 수치를 올린다
            return true;
        }
        return false;
    }

    /*
    // ====================================================================
    // 아래부터 추가: 면역력 관련 설치물과의 상호작용 (E키)
    // 위의 기본 코드는 수정하지 않았고, LateUpdate로 따로 동작한다.
    // ====================================================================

    [Header("면역력 설치물 상호작용 (E키)")]
    [SerializeField] private LayerMask stationLayer;          // 설치물이 속한 레이어. 설정 전(Nothing)에는 동작하지 않는다
    [SerializeField] private float stationDetectionRadius = 2f;
    [SerializeField] private float stationIncreaseAmount = 40f; // TODO: 임시값. 설치물이 추가되면 수정할 것

    void LateUpdate()
    {
        HandleStationInteraction();
    }

    private void HandleStationInteraction()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.eKey.wasPressedThisFrame) return;

        // 근처에 설치물이 없으면 아무 일도 일어나지 않는다
        Collider[] stations = Physics.OverlapSphere(transform.position, stationDetectionRadius, stationLayer, QueryTriggerInteraction.Collide);
        if (stations.Length == 0) return;

        PlayerStatus status = PlayerStatus.Instance;
        if (status.CurrentRadiation >= status.MaxRadiation) return;   // 면역력이 이미 가득 찼으면 동작하지 않는다

        // TODO: 설치물 종류가 정해지면 설치물 스크립트를 가져와 거기서 수치를 받아오도록 변경
        status.ModifyRadiation(stationIncreaseAmount);   // 양수로 넘겨 수치를 올린다
    }
    */
}