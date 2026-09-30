using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public bool needDelay = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
    {
        Debug.Log("æ∆¿Ã≈€ »πµÊ");
        Destroy(gameObject);
    }
}
