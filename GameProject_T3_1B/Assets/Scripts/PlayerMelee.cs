using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMelee : MonoBehaviour
{
    public bool canMeleeAttack = false;

    public GameObject meleeAttackRange;

    public float fadeTime = 0.3f;
    public float lastAttackTime = -999f;
    public float attackCooldown = 0.5f;


    public void OnAttack(InputValue inputValue)
    {
        if (HotbarUI.IsBlockingGameplayClick) return;   // [추가] 핫바를 클릭하거나 드래그하는 동안은 공격하지 않는다
        if (!canMeleeAttack) return;
        if (Time.timeScale == 0f) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        meleeAttackRange.gameObject.SetActive(true);

    }

    private void Update()
    {

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // [변경] 숫자키 1~9는 핫바가 쓰므로, 무기 전환 키를 Z(원거리) / X(근접)로 옮김
        if (keyboard.zKey.wasPressedThisFrame)
        {
            canMeleeAttack = false;
        }
        else if (keyboard.xKey.wasPressedThisFrame)
        {
            canMeleeAttack = true;
        }

        if (meleeAttackRange == null) return;

        if (Time.time > lastAttackTime + fadeTime)
        {
            meleeAttackRange.gameObject.SetActive(false);
        }
    }
}