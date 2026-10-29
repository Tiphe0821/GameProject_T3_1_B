using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneDoor : MonoBehaviour
{
    [Header("Scene Setting")]
    [SerializeField] private string targetSceneName = "TetherScene"; // 이동할 씬 이름
    [SerializeField] private float interactiveDistance = 3.0f;       // 상호작용 가능 거리 (3)

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

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactiveDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }
}