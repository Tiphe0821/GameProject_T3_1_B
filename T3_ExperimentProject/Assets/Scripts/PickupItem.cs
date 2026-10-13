using UnityEngine;
using GameFramework.Core;
using GameFramework.Services;
using GameFramework.Gameplay;
using GameFramework.Data;

public class PickupItem : MonoBehaviour
{
    public bool needDelay = false;

    public ItemData itemData;

    public void Collect()
    {
        Debug.Log("æ∆¿Ã≈€ »πµÊ");

        if (!needDelay)
        {
            InventoryManager.Instance.AddItem(itemData.id, 1);
        }

        Destroy(gameObject);
    }
}
