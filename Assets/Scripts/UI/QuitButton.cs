using UnityEngine;

// 게임 종료 버튼 동작을 처리한다.
public class QuitButton : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    public void OnQuitButtonClick(bool isQuit)
    {
        if (isQuit)
            UIManager.Instance.ClickExitGame();
        else
            this.gameObject.SetActive(false);
    }
}