using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 0.2f;

    float lastFireTime = -999f;

    public PlayerMelee playerMelee;

    private void Awake()
    {
        playerMelee = GetComponent<PlayerMelee>();
    }

    public void OnAttack(InputValue value)
    {
        if (HotbarUI.IsBlockingGameplayClick) return;   // [추가] 핫바를 클릭하거나 드래그하는 동안은 공격하지 않는다
        if (playerMelee.canMeleeAttack) return;
        if (Time.timeScale == 0f) return;
        if (Time.time < lastFireTime + fireCooldown) return;
        lastFireTime = Time.time;

        GameObject bullet = Instantiate(bulletPrefab,
            firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}