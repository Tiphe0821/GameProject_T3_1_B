using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AgentData
{
    public string agentName;
    public Sprite agentIcon;       
    
    [Header("Agent Stats")]
    public float maxHealth;        
    public float maxOxygen;        
    public float moveSpeed;        
}

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; }

    [Header("Agent Settings")]
    public List<AgentData> allAgents = new List<AgentData>();
    public int currentAgentIndex { get; private set; } = 0; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AgentData GetCurrentAgent()
    {
        if (allAgents.Count > 0 && currentAgentIndex >= 0 && currentAgentIndex < allAgents.Count)
            return allAgents[currentAgentIndex];
        return null;
    }

    public List<int> GetStandbyAgentIndices()
    {
        List<int> standby = new List<int>();
        for (int i = 0; i < allAgents.Count; i++)
        {
            if (i != currentAgentIndex)
            {
                standby.Add(i);
            }
        }
        return standby;
    }

    public void SwitchAgent(int newIndex)
    {
        if (newIndex >= 0 && newIndex < allAgents.Count)
        {
            currentAgentIndex = newIndex;
            AgentData newAgent = allAgents[newIndex];

            if (PlayerStatus.Instance != null)
            {
                // PlayerStatus.Instance.maxHealth = newAgent.maxHealth;
                Debug.Log($"[AgentManager] {newAgent.agentName}(으)로 직업 변경 완료!");
            }
        }
    }
}