using UnityEngine;

public class MeterialSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    [SerializeField] private GameObject[] resourcePrefabs; // 소환할 자원 프리팹들 (프리팹들 1단계 자원 4개)

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 4.0f;     // 자원이 생성되는 주기 (초)
    [SerializeField] private float spawnRadius = 25f;      // 스폰 범위 (함수 주변 넓이)
    [SerializeField] private bool isSpawning = true;         // 스폰 활성화 여부

    private float timer;

    private void Update()
    {
        if (!isSpawning) return;

        timer += Time.deltaTime;

        // 정해진 주기가 되면 자원 소환
        if (timer >= spawnInterval)
        {
            SpawnMaterial();
            timer = 0f;
        }
    }

    private void SpawnMaterial()
    {
        if (resourcePrefabs == null || resourcePrefabs.Length == 0)
        {
            Debug.LogWarning("스폰할 자원 프리팹이 등록되지 않았습니다!");
            return;
        }

        // 등록된 자원 중 무작위로 하나 선택
        GameObject selectedPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];

        //스포너 주변 무작위 위치 계산 (범위 내)
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // 자원 오브젝트 생성 (이때 이동 스크립트가 알아서 움직임)
        if (selectedPrefab != null)
        {
            Instantiate(selectedPrefab, spawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        }
    }
}