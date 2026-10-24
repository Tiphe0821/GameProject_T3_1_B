using UnityEngine;
using UnityEngine.UI;

public class CheatEngineManager : MonoBehaviour
{

    //  활성화 상태 - 활성화 상태라면 전용 UI - 키 가이드 표시

    private GameObject CheatPanel;

    private Button maxHealthButton;             // 체력 최대치로 회복
    private Button maxOxygenButton;             // 산소 수치 최대로 회복
    private Button maxRadiationButton;          // 면역력 최대치로 회복

    
    void Start()
    {
        if (maxHealthButton != null)
            maxHealthButton.onClick.AddListener(OnClickMaxHealth);

        if (maxOxygenButton != null)
            maxOxygenButton.onClick.AddListener(OnClickMaxOxygen);

        if (maxRadiationButton != null)
            maxRadiationButton.onClick.AddListener(OnClickMaxRadiation);
    }

    public void OnClickMaxHealth()
    {
        if (PlayerStatus.Instance != null)
        {
            float missingHealth = PlayerStatus.Instance.MaxHealth - PlayerStatus.Instance.CurrentHealth;
            PlayerStatus.Instance.ModifyHealth(missingHealth);
        }
    }
    public void OnClickMaxOxygen()
    {
        if (PlayerStatus.Instance != null)
        {
            float missingOxygen = PlayerStatus.Instance.MaxOxygen - PlayerStatus.Instance.CurrentOxygen;
            PlayerStatus.Instance.ModifyOxygen(missingOxygen);
        }
    }
    public void OnClickMaxRadiation()
    {
        if (PlayerStatus.Instance != null)
        {
            float missingRadiation = PlayerStatus.Instance.MaxRadiation - PlayerStatus.Instance.CurrentRadiation;
            PlayerStatus.Instance.ModifyRadiation(missingRadiation);
        }
    }

    public void StopStatusChange()
    {
        // 플레이어 스테이터스 매니져한테 스테이터스를 바꾸지 못하도록 호출하기?
    }

    
}
