using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float decreaseAmount = -2.0f;
    [SerializeField] private float decreaseFrequency = 0.6f;

    private float lastDecreasedTime = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastDecreasedTime + decreaseFrequency < Time.time)
        {
            PlayerStatus.Instance.ModifyOxygen(decreaseAmount);
            lastDecreasedTime = Time.time;  
        }
    }
}
