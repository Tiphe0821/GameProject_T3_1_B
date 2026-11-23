using UnityEngine;

public class MainMonitor : MonoBehaviour
{
    [Header("UI 세팅")]
    [SerializeField] private GameObject monitorPanel;
    [SerializeField] private float interactiveDistance = 5.0f;

    private Transform playerTransform;
    private AgentOrbitUI orbitUI;

    private void Start()
    {
        FindPlayer();

        if (monitorPanel != null)
        {
            orbitUI = monitorPanel.GetComponentInChildren<AgentOrbitUI>(true);
            if (orbitUI == null)
            {
                orbitUI = monitorPanel.GetComponentInParent<AgentOrbitUI>();
            }
            monitorPanel.SetActive(false);
        }
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
            if (monitorPanel != null && !monitorPanel.activeSelf)
            {
                monitorPanel.SetActive(true);
                if (orbitUI != null)
                {
                    orbitUI.StartOpenAnimation();
                }
            }
        }
        else
        {
            if (monitorPanel != null && monitorPanel.activeSelf)
            {
                monitorPanel.SetActive(false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactiveDistance);
    }
}