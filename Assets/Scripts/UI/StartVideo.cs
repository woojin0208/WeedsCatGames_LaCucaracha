using UnityEngine;
using UnityEngine.Video;

// 시작 연출 영상을 재생한다.
public class StartVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [SerializeField]
    private GameObject startImage;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnPrepared(VideoPlayer videoPlayer)
    {
        startImage.SetActive(false);
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer _)
    {
        GameManager.Instance.TryLoadScene("Tutorial");
    }
}