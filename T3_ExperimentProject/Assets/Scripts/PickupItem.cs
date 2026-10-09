using UnityEngine;
using GameFramework.Core;
using GameFramework.Services;
using GameFramework.Gameplay;

public class PickupItem : MonoBehaviour
{
    public bool needDelay = false;

    private void OnEnable()
    {
        
    }

    public void Collect()
    {
        Debug.Log("æ∆¿Ã≈€ »πµÊ");
        if (!needDelay)
        {
            InventoryManager.Instance.AddItem("temp", 1);
        }

        Destroy(gameObject);
    }
}
