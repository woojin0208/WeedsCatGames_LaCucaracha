using UnityEngine;
using UnityEngine.Video;

public class FirstViewCutScene : MonoBehaviour
{
    UIManager um;
    [SerializeField] VideoPlayer vPlayer;

    private void OnEnable()
    {
        if (vPlayer != null)
            vPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        if (vPlayer != null)
            vPlayer.loopPointReached -= OnVideoEnd;
    }

    private void Start()
    {
        um = UIManager.Instance;

        if (um.canViewVideo)
        {
            vPlayer.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            // º¼ ¶§
            um.gameObject.SetActive(false);
        }
        
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        um.gameObject.SetActive(true);
        um.canViewVideo = true;

        vPlayer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
