using UnityEngine;
using UnityEngine.UI;

public class CheatEngineManager : MonoBehaviour
{
    [Header("UI 패널")]
    [SerializeField] private GameObject cheatPanel;  // 인스펙터에서 연결할 수 있도록 serializeField 추가

    [Header("치트 버튼")]
    [SerializeField] private Button maxHealthButton;             // 체력 최대치로 회복
    [SerializeField] private Button maxOxygenButton;             // 산소 수치 최대로 회복
    [SerializeField] private Button maxRadiationButton;          // 면역력 최대치로 회복
    [SerializeField] private Button activateImmunityButton;          // 무적 활성화 키

    void Start()
    {
        // 버튼 이벤트 리스너 등록
        if (maxHealthButton != null)
            maxHealthButton.onClick.AddListener(OnClickMaxHealth);

        if (maxOxygenButton != null)
            maxOxygenButton.onClick.AddListener(OnClickMaxOxygen);

        if (maxRadiationButton != null)
            maxRadiationButton.onClick.AddListener(OnClickMaxRadiation);

        if(activateImmunityButton != null)
            activateImmunityButton.onClick.AddListener(StopStatusChange);

        // 게임 시작 시 치트 패널은 기본적으로 꺼두기
        if (cheatPanel != null)
            cheatPanel.SetActive(false);
    }

    void Update()
    {
        // 패널 토글 기능 추가 (~ 키나 F1 키를 누르면 치트 패널이 켜지고 꺼짐)
        if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F1))
        {
            ToggleCheatPanel();
        }
    }

    // 치트 패널 활성화/비활성화 전환 함수
    private void ToggleCheatPanel()
    {
        if (cheatPanel != null)
        {
            bool isActive = cheatPanel.activeSelf;
            cheatPanel.SetActive(!isActive);
        }
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
        PlayerStatus.Instance.OnOffImmunity();
    }
}