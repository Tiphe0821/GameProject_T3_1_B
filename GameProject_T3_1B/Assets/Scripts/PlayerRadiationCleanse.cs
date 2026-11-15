using UnityEngine;
using UnityEngine.InputSystem;
using GameFramework.Gameplay;

public class PlayerRadiationCleanse : MonoBehaviour
{
    [Header("방사선 제거 아이템")]
    [SerializeField] private string radiationItemId = "";     // 방사선 제거 아이템의 ItemData.id
    [SerializeField] private float reduceAmount = 40f;        // 사용 시 줄어드는 방사선 수치

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.gKey.wasPressedThisFrame)
        {
            CleanseRadiation();
        }
    }

    private void CleanseRadiation()
    {
        if (string.IsNullOrEmpty(radiationItemId)) return;

        PlayerStatus status = PlayerStatus.Instance;

        // 방사선 수치가 0이면 아이템을 소모하지 않는다
        if (status.CurrentRadiation <= 0f) return;

        if (InventoryManager.Instance.GetCount(radiationItemId) > 0)
        {
            InventoryManager.Instance.RemoveItem(radiationItemId);
            status.ModifyRadiation(-reduceAmount);   // 음수로 넘겨 수치를 낮춘다
        }
    }
}