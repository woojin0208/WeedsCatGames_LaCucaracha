using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 미니맵 UI를 제어한다.
public class Minimap : MonoBehaviour
{
    [SerializeField]
    private GameObject[] maps;

    [SerializeField]
    private Vector2[] playerPosition;

    [SerializeField]
    private GameObject playerUI;

    private void OnEnable()
    {
        Time.timeScale = 0f;

        string currentMap = SceneManager.GetActiveScene().name;
        string numbers = "";
        Debug.Log(currentMap);
        for (int i = 0; i < currentMap.Length; i++)
        {
            Debug.Log(currentMap);
            if (int.TryParse(currentMap[i].ToString(), out int num))
            {
                numbers += num;
                Debug.Log(numbers);
            }
        }
        if (int.TryParse(numbers, out int mapNumber))
        {
            mapNumber = mapNumber > 1 ? 1 : 0;
            
            RectTransform playerRect = playerUI.GetComponent<RectTransform>();
            playerRect.anchoredPosition = playerPosition[mapNumber];

            Button mapButton = maps[mapNumber].GetComponent<Button>();
            mapButton.interactable = false;
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void OnClickMap(int num)
    {
        DialogueManager.Instance.CloseDialogue();
        GameManager.Instance.TryLoadScene($"stage{num}");
        UIManager.Instance.OpenMinimap();
    }

    public void CloseMap() => this.gameObject.SetActive(false);

}