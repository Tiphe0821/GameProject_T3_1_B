using UnityEngine;

public class PlayerRadiationZone : MonoBehaviour
{

    public bool isRadiation = false;

    public float lastDecreasedTime = -999f;
    public float decreaseAmount = 5f;
    public float decreaseFrequency = 0.7f;

    private void Awake()
    {
        isRadiation = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isRadiation=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isRadiation = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRadiation) return;
        if (lastDecreasedTime + decreaseFrequency < Time.time)
        {
            PlayerStatus.Instance.ModifyRadiation(decreaseAmount);
            lastDecreasedTime = Time.time;
        }
    }
}
