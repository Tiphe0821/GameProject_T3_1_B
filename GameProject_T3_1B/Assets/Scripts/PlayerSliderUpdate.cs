using UnityEngine;
using UnityEngine.UI;

public class PlayerSliderUpdate : MonoBehaviour
{
    public Slider hpSlider;
    public Slider radiationSlider;
    public Slider oxygenSlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHpBar()
    {
        if (hpSlider != null)
            hpSlider.value = PlayerStatus.Instance.CurrentHealth/PlayerStatus.Instance.MaxHealth;
    }

    public void UpdateOxygenBar()
    {
        if(oxygenSlider != null)
            oxygenSlider.value = PlayerStatus.Instance.CurrentOxygen / PlayerStatus.Instance.MaxOxygen;
    }

    public void UpdateRadiationBar()
    {
        if(radiationSlider != null)
            radiationSlider.value = PlayerStatus.Instance.CurrentRadiation / PlayerStatus.Instance.MaxRadiation;
    }
}
