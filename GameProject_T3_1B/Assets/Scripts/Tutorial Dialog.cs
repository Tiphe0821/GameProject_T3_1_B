
using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialDialog : MonoBehaviour
{
    [Header("튜토리얼 UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("튜토리얼 설정")]
    [SerializeField] private string[] tutorialMessages;
    [SerializeField] private float tutorialInterval = 5.0f;

    private Coroutine tutorialRoutine;

    // 튜토리얼 진행 여부
    public bool IsTutorialPlaying { get; private set; }

    // 현재 출력 중인 문장 번호
    public int CurrentMessageIndex { get; private set; }

    void Start()
    {
        // 처음에는 대화창을 숨긴다.
        dialoguePanel.SetActive(false);

        // 튜토리얼 자동 시작
        StartTutorial();
    }

    // 튜토리얼 시작
    public void StartTutorial()
    {
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
        }

        tutorialRoutine = StartCoroutine(TutorialRoutine());
    }

    // 튜토리얼 종료
    public void StopTutorial()
    {
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
            tutorialRoutine = null;
        }

        IsTutorialPlaying = false;
        dialoguePanel.SetActive(false);
    }

    // 튜토리얼 대화 진행
    private IEnumerator TutorialRoutine()
    {
        IsTutorialPlaying = true;
        CurrentMessageIndex = 0;

        dialoguePanel.SetActive(true);

        // 등록된 문장을 순서대로 출력
        for (int i = 0; i < tutorialMessages.Length; i++)
        {
            CurrentMessageIndex = i;
            dialogueText.text = tutorialMessages[i];

            // 다음 문장까지 대기
            yield return new WaitForSeconds(tutorialInterval);
        }

        // 모든 문장이 끝나도 대화창을 유지한다.
        tutorialRoutine = null;
    }
}
