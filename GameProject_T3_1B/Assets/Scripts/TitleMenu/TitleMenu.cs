using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [Header("옵션 창 연결")]
    public GameObject optionsPanel;

    private void Start()
    {
        // 처음에는 옵션 창을 숨깁니다.
        CloseOptions();
    }

    // 시작 버튼: TestScene으로 이동
    public void StartGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    // 옵션 버튼: 옵션 창 열기
    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    // 옵션 창의 닫기 버튼
    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    // 나가기 버튼
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티에서 테스트할 때는 실행을 멈춥니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 완성된 게임에서는 프로그램을 종료합니다.
        Application.Quit();
#endif
    }
}