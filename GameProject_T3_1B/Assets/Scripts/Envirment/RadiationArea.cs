using UnityEngine;

public class RadiationArea : MonoBehaviour
{


    [SerializeField] private float radiationDamage = 1f;
    [SerializeField] private float radiationFrequency = 0.7f;

    private float lastDamageTime = -999f;
    private bool inRadiation;
    

    void Start()
    {
        inRadiation = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            inRadiation = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            inRadiation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(inRadiation && lastDamageTime + radiationFrequency < Time.time)
        {
            PlayerStatus.Instance.ModifyRadiation(-1f);
            lastDamageTime = Time.time;
        }
    }
}
