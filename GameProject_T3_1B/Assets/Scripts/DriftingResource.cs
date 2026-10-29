using UnityEngine;

public class DriftingResource : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 moveDirection = new Vector3(-1f, 0f, 0f);  // 흘러가는 방향
    [SerializeField] private float moveSpeed = 3f;                              // 이동 속도
    [SerializeField] private float rotationSpeed = 45f;                         // 회전 속도 (우주 느낌 연출)
    [SerializeField] private float lifeTime = 20f;                              // 일정 시간 후 자동 소멸 (메모리 관리)

    private void Start()
    {
        // 지정된 시간이 지나면 자동으로 파괴 (맵 밖으로 나가서 쌓이는 것 방지)
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        //앞으로 흘러가게 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        //빙글빙글 돌리는 표류 느낌 주기
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    // 플레이어가 획득했을 때 호출할 함수
    public void Collect()
    {
        // 여기에 아이템 획득 효과음이나 인벤토리 추가 로직 연결 (아직 미완이니 수정 해야 함.)
        Destroy(gameObject);
    }
}