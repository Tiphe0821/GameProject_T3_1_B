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
    [SerializeField] private float targetRadius = 250f;
    [SerializeField] private float targetOrbitSpeed = 40f;
    [SerializeField] private float openDuration = 2f;

    private float currentAngle = 0f;
    private float currentRadius = 0f;
    private float currentOrbitSpeed = 0f;
    private float openTimer = 0f;
    private bool isOpening = false;
    private bool wasPanelOpen = false;
    private List<int> currentStandbyIndices = new List<int>();
    private int activeOuterCount = 0;

    private void OnEnable()
    {
        StartOpenAnimation();
    }

    public void StartOpenAnimation()
    {
        openTimer = 0f;
        currentRadius = 0f;
        currentOrbitSpeed = 0f;
        isOpening = true;
        RefreshUI();
    }

    private void Update()
    {
        if (centerIcon != null)
        {
            bool isPanelOpen = centerIcon.gameObject.activeInHierarchy;
            if (isPanelOpen && !wasPanelOpen)
            {
                wasPanelOpen = true;
                StartOpenAnimation();
            }
            else if (!isPanelOpen)
            {
                wasPanelOpen = false;
                return;
            }
        }

        if (outerIcons == null || outerIcons.Length == 0 || activeOuterCount == 0) return;

        if (isOpening)
        {
            openTimer += Time.deltaTime;
            float progress = openDuration > 0f ? Mathf.Clamp01(openTimer / openDuration) : 1f;

            currentRadius = Mathf.Lerp(0f, targetRadius, progress);
            currentOrbitSpeed = Mathf.Lerp(0f, targetOrbitSpeed, progress);

            if (progress >= 1f)
            {
                isOpening = false;
                currentRadius = targetRadius;
                currentOrbitSpeed = targetOrbitSpeed;
            }
        }
        else
        {
            currentRadius = targetRadius;
            currentOrbitSpeed = targetOrbitSpeed;
        }

        currentAngle += currentOrbitSpeed * Time.deltaTime;
        float angleStep = 360f / activeOuterCount;

        Vector2 centerPos = centerIcon != null ? centerIcon.rectTransform.anchoredPosition : Vector2.zero;
        int activeIndex = 0;

        for (int i = 0; i < outerIcons.Length; i++)
        {
            if (outerIcons[i] == null || !outerIcons[i].gameObject.activeSelf) continue;

            float angle = currentAngle + (angleStep * activeIndex);
            float rad = angle * Mathf.Deg2Rad;

            float x = centerPos.x + Mathf.Cos(rad) * currentRadius;
            float y = centerPos.y + Mathf.Sin(rad) * currentRadius;

            outerIcons[i].rectTransform.anchoredPosition = new Vector2(x, y);
            activeIndex++;
        }
    }

    public void RefreshUI()
    {
        if (AgentManager.Instance == null) return;

        AgentData currentAgent = AgentManager.Instance.GetCurrentAgent();
        if (currentAgent != null && centerIcon != null)
        {
            centerIcon.sprite = currentAgent.agentIcon;
            centerIcon.gameObject.SetActive(true);
        }

        currentStandbyIndices = AgentManager.Instance.GetStandbyAgentIndices();
        activeOuterCount = Mathf.Min(currentStandbyIndices.Count, outerIcons != null ? outerIcons.Length : 0);

        if (outerIcons == null) return;

        Vector2 centerPos = centerIcon != null ? centerIcon.rectTransform.anchoredPosition : Vector2.zero;

        for (int i = 0; i < outerIcons.Length; i++)
        {
            if (i < activeOuterCount)
            {
                int agentIndex = currentStandbyIndices[i];
                AgentData standbyData = AgentManager.Instance.allAgents[agentIndex];

                if (outerIcons[i] != null)
                {
                    outerIcons[i].sprite = standbyData.agentIcon;
                    if (isOpening)
                    {
                        outerIcons[i].rectTransform.anchoredPosition = centerPos;
                    }
                    outerIcons[i].gameObject.SetActive(true);
                }

                if (outerButtons != null && i < outerButtons.Length && outerButtons[i] != null)
                {
                    outerButtons[i].onClick.RemoveAllListeners();
                    outerButtons[i].onClick.AddListener(() => OnOuterIconClicked(agentIndex));
                }
            }
            else
            {
                if (outerIcons[i] != null)
                {
                    outerIcons[i].gameObject.SetActive(false);
                }

                if (outerButtons != null && i < outerButtons.Length && outerButtons[i] != null)
                {
                    outerButtons[i].onClick.RemoveAllListeners();
                }
            }
        }
    }

    private void OnOuterIconClicked(int agentIndex)
    {
        AgentManager.Instance.SwitchAgent(agentIndex);
        RefreshUI();
    }
}