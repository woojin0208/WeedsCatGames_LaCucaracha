using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

// 엔딩 컷신 연출을 제어한다.
public class EndCutScene : MonoBehaviour
{
    [SerializeField] private GameObject[] disableObjs;

    private VideoPlayer vp;

    private void OnEnable()
    {
        SetInputEventSubscription(true);

        if (GameManager.Instance.Stage2CutScene == true)
        {
            InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
            foreach (GameObject obj in disableObjs) obj.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        InputStateManager.Instance?.ChangeState(InputStateType.CutScene);
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += End;
    }

    private void OnDisable()
    {
        if (vp != null)
        {
            vp.loopPointReached -= End;
        }

        SetInputEventSubscription(false);
    }

    public void End(VideoPlayer vp)
    {
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
        GameManager.Instance.Stage2CutScene = true;
        foreach (GameObject obj in disableObjs) obj.SetActive(false);

        gameObject.SetActive(false);
    }

    private void HandleCutsceneSkipRequested()
    {
        if (!gameObject.activeInHierarchy) return;
        End(vp);
    }

    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        manager.CutsceneSkipRequested -= HandleCutsceneSkipRequested;
        if (isSubscribe)
        {
            manager.CutsceneSkipRequested += HandleCutsceneSkipRequested;
        }
    }
}
