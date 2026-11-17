using UnityEngine;
using UnityEngine.InputSystem;
using GameFramework.Gameplay;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float decreaseAmount = -2.0f;
    [SerializeField] private float decreaseFrequency = 0.6f;
    [SerializeField] private float increaseAmount = 60f;

    [Header("산소 아이템")]
    [SerializeField] private string oxygenItemId = "Dummy";   // 산소 아이템의 ItemData.id

    private float lastDecreasedTime = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Keyboard keyboard = Keyboard.current;

        // [변경] F키 충전은 제거. 이제 PlayerItemInteraction이 E키로 TryUse를 호출한다.

        if (lastDecreasedTime + decreaseFrequency < Time.time)
        {
            PlayerStatus.Instance.ModifyOxygen(decreaseAmount);
            lastDecreasedTime = Time.time;
        }


    }

    // [추가] 들고 있는 아이템이 산소 아이템일 때만 사용한다. 사용했으면 true
    public bool TryUse(string heldItemId)
    {
        if (heldItemId != oxygenItemId) return false;
        return ChargeOxygen();
    }

    private bool ChargeOxygen()
    {
        if (0 < InventoryManager.Instance.GetCount(oxygenItemId))
        {
            InventoryManager.Instance.RemoveItem(oxygenItemId);
            PlayerStatus.Instance.ModifyOxygen(increaseAmount);
            return true;
        }
        return false;
    }
}