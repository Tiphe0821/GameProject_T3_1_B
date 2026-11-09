using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AgentOrbitUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image centerIcon;
    [SerializeField] private Image[] outerIcons;
    [SerializeField] private Button[] outerButtons;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 150f;
    [SerializeField] private float orbitSpeed = 30f;

    private float currentAngle = 0f;
    private List<int> currentStandbyIndices = new List<int>();

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (outerIcons == null || outerIcons.Length == 0) return;

        currentAngle += orbitSpeed * Time.deltaTime;
        float angleStep = 360f / outerIcons.Length;

        Vector2 centerPos = Vector2.zero;

        for (int i = 0; i < outerIcons.Length; i++)
        {
            if (outerIcons[i] == null || !outerIcons[i].gameObject.activeSelf) continue;

            float angle = currentAngle + (angleStep * i);
            float rad = angle * Mathf.Deg2Rad;

            float x = centerPos.x + Mathf.Cos(rad) * orbitRadius;
            float y = centerPos.y + Mathf.Sin(rad) * orbitRadius;

            outerIcons[i].rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    private void RefreshUI()
    {
        if (AgentManager.Instance == null) return;

        AgentData currentAgent = AgentManager.Instance.GetCurrentAgent();
        if (currentAgent != null)
        {
            centerIcon.sprite = currentAgent.agentIcon;
            centerIcon.gameObject.SetActive(true);
        }

        currentStandbyIndices = AgentManager.Instance.GetStandbyAgentIndices();

        for (int i = 0; i < outerIcons.Length; i++)
        {
            if (i < currentStandbyIndices.Count)
            {
                int agentIndex = currentStandbyIndices[i];
                AgentData standbyData = AgentManager.Instance.allAgents[agentIndex];

                outerIcons[i].sprite = standbyData.agentIcon;
                outerIcons[i].gameObject.SetActive(true);

                outerButtons[i].onClick.RemoveAllListeners();
                outerButtons[i].onClick.AddListener(() => OnOuterIconClicked(agentIndex));
            }
            else
            {
                outerIcons[i].gameObject.SetActive(false);
                outerButtons[i].onClick.RemoveAllListeners();
            }
        }
    }

    private void OnOuterIconClicked(int agentIndex)
    {
        AgentManager.Instance.SwitchAgent(agentIndex);
        RefreshUI();
    }
}