using UnityEngine;
using UnityEngine.Video;

public class StartVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [SerializeField]
    private GameObject startImage;

    [SerializeField]
    private float timer = 1.5f;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        //Invoke(nameof(PlayVideo), 0.5f);
        Invoke(nameof(StartImage), 0.2f);
        Invoke(nameof(StartGame), 3f);
    }

    private void StartImage()
    {
        startImage.SetActive(false);
    }
    private void PlayVideo() => videoPlayer.Play();

    private void StartGame() => GameManager.Instance.TryLoadScene("Tutorial");
}
