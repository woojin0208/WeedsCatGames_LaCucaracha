using UnityEngine;
using UnityEngine.Video;

public class FirstViewCutScene : MonoBehaviour
{
    UIManager um;
    [SerializeField] VideoPlayer vPlayer;
    [SerializeField] private AnchoredBossController bossController;
    [SerializeField] private GameObject player;

    [SerializeField] BossUI bossUI;
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
        player.transform.position = new Vector2(-6, -3);
        bossController.StartBattle();
        bossUI.StartBossBattle();

        vPlayer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
