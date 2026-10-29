using UnityEngine;
using UnityEngine.SceneManagement;

public class TetherSceneDoor : MonoBehaviour
{
    [Header("Scene Setting")]
    [SerializeField] private string targetSceneName = "TestScene";
    [SerializeField] private float interactiveDistance = 3.0f;

    private void Update()
    {
        if (PlayerStatus.Instance != null)
        {
            float distance = Vector3.Distance(transform.position, PlayerStatus.Instance.transform.position);

            if (distance <= interactiveDistance)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    SceneManager.LoadScene(targetSceneName);
                }
            }
        }
    }
}