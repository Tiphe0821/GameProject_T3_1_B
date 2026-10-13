using UnityEngine;
using GameFramework.Gameplay;
using GameFramework.Data;

public class PickupItem : MonoBehaviour
{
    public bool needDelay = false;      // 나중에 오브젝트 상호작용과 나눌 예정

    public ItemData itemData;

    public void Collect()
    {
        Debug.Log("아이템 획득");

        if (!needDelay)
        {
            InventoryManager.Instance.AddItem(itemData.id, 1);
        }

        Destroy(gameObject);
    }
}
