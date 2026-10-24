using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 관련 변수 리스트")]
    public int maxHp = 3;
    public int currentHp;

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        //currentHp = currentHp - damage; (윗 줄과 같은 스크립트)

        Debug.Log(name + " HP: " + currentHp);

        if (currentHp <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}


