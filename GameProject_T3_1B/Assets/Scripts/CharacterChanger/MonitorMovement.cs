using UnityEngine;

public class MonitorMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Bearing Rotation")]
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float minDistance = 0.01f;

    private Transform playerTransform;

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < minDistance * minDistance) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 targetEuler = targetRotation.eulerAngles;
        Vector3 currentEuler = transform.rotation.eulerAngles;

        float smoothY = Mathf.LerpAngle(currentEuler.y, targetEuler.y, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentEuler.x, smoothY, currentEuler.z);
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }
}