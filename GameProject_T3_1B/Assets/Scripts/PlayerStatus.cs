using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    [Header("스탯 세팅")]
    //체력
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    //산소 수치
    [SerializeField] private float maxOxygen = 100f;
    private float currentOxygen;

    //방사'선' 수치 (절대로 방사'능'이 방사"능"이 방사!능!이 아님) 방사능은 물질이고, 방사선은 입자<<<<<<<<<<
    [SerializeField] private float maxRadiation = 100f;
    private float currentRadiation;

    // =>기호로 둔 이유는 계속 수치가 바뀌기 때문임. 그렇기 때문에 추후 다른 분께서 스크립트를 수정하게 된다면 절!!대로 기호 변경이 없길 바랍니다
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    
    public float CurrentOxygen => currentOxygen;
    public float MaxOxygen => maxOxygen;

    public float CurrentRadiation => currentRadiation;
    public float MaxRadiation => maxRadiation;


    // (현재 값, 최대 값)으로 설정 해두었음.
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent<float, float> onOxygenChanged;     
    public UnityEvent<float, float> onRadiationChanged;  

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        currentRadiation = 0f;

        NotifyUI();
    }

    private void Update()
    {
        // ModifyOxygen(-1f * Time.deltaTime);
    }

    public void ModifyHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            HandleDeath();
        }
    }

    public void ModifyOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(currentOxygen + amount, 0f, maxOxygen);
        onOxygenChanged?.Invoke(currentOxygen, maxOxygen);
    }

    public void ModifyRadiation(float amount)
    {
        currentRadiation = Mathf.Clamp(currentRadiation + amount, 0f, maxRadiation);
        onRadiationChanged?.Invoke(currentRadiation, maxRadiation);
    }

    private void NotifyUI()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onOxygenChanged?.Invoke(currentOxygen, maxOxygen);
        onRadiationChanged?.Invoke(currentRadiation, maxRadiation);
    }

    private void HandleDeath()
    {
        Debug.Log("플레이어 사망 처리");
    }
}