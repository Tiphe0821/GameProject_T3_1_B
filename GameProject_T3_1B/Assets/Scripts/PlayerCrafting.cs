using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 2f;      // PlayerPickup의 detectionRadius와 같은 값으로 맞출 것

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.eKey.wasPressedThisFrame) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);

        CraftingStation closestStation = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            // 주울 아이템이나 문이 근처에 있으면 PlayerPickup에게 양보한다
            if (col.GetComponentInParent<PickupItem>() != null) return;
            if (col.GetComponentInParent<Door>() != null) return;

            CraftingStation station = col.GetComponentInParent<CraftingStation>();
            if (station == null) continue;

            float distance = Vector3.Distance(transform.position, station.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestStation = station;
            }
        }

        if (closestStation != null)
            closestStation.TryCraft();
    }
}