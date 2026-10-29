using UnityEngine;

public class TetherController : MonoBehaviour
{
    [Header("Target References")]
    [SerializeField] private Transform shipTransform;   // 함선 (기준점)
    [SerializeField] private Transform playerTransform; // 플레이어
    [SerializeField] private LineRenderer lineRenderer; // 라인 렌더러 (줄 시각화. 아직은 스프링 처럼 구현되진 않았으나, 기말에 수정해서 현실적이게 제작할 예정)

    [Header("Tether Settings")]
    [SerializeField] private float maxLength = 15f;     // 줄의 최대 길이 (인스펙터에서 조절 가능. 나중에 짧거나 길면 수정할 예정.)

    private void Start()
    {
        // 라인 렌더러 점 개수를 2개(시작점: 함선, 끝점: 플레이어)로 설정
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
    }

    private void LateUpdate()
    {
        if (shipTransform == null || playerTransform == null) return;

        // 1. 함선과 플레이어 사이의 거리 계산
        float distance = Vector3.Distance(shipTransform.position, playerTransform.position);

        // 2. 최대 길이를 초과했을 경우 처리
        if (distance > maxLength)
        {
            // 플레이어에서 함선 방향으로 향하는 단위 벡터 계산
            Vector3 directionToShip = (shipTransform.position - playerTransform.position).normalized;

            // 함선 방향으로 5만큼 강제로 끌어오기
            playerTransform.position += directionToShip * 5f;
        }

        // 3. 라인 렌더러 위치 업데이트 (줄 그리기)
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, shipTransform.position);   // 0번 점: 함선 (트랜스폼/게임오브젝트)
            lineRenderer.SetPosition(1, playerTransform.position); // 1번 점: 플레이어(그냥 플레이어)
        }
    }
}