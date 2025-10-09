using UnityEngine;

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
