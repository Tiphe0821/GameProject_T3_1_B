using UnityEngine;
using UnityEngine.InputSystem;
using GameFramework.Gameplay;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float decreaseAmount = -2.0f;
    [SerializeField] private float decreaseFrequency = 0.6f;
    [SerializeField] private float increaseAmount = 60f;

    private float lastDecreasedTime = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Keyboard keyboard = Keyboard.current;

        // 가까운 아이템이 있을 떄 E키로 줍는다 
        // 마우스로 그 아이템을 겨눴을 때 하고싶은데.. 당장은 이렇게 해놓자. 나중에 바꾸는 걸로

        if (keyboard.fKey.wasPressedThisFrame)
        {
            ChargeOxygen();
        }

        if (lastDecreasedTime + decreaseFrequency < Time.time)
        {
            PlayerStatus.Instance.ModifyOxygen(decreaseAmount);
            lastDecreasedTime = Time.time;  
        }

        
    }

    private void ChargeOxygen()
    {
        if( 0 < InventoryManager.Instance.GetCount("Dummy"))
        {
            InventoryManager.Instance.RemoveItem("Dummy");
            PlayerStatus.Instance.ModifyOxygen(increaseAmount);
        }
    }
}
