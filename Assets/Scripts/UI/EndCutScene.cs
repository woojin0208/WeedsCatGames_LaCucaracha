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
        if (GameManager.Instance.Stage2CutScene == true)
        {
            foreach (GameObject obj in disableObjs) obj.SetActive(false);

            gameObject.SetActive(false);
        }
    
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += End;
    }
    public void End(VideoPlayer vp)
    {
        GameManager.Instance.Stage2CutScene = true;
        foreach (GameObject obj in disableObjs) obj.SetActive(false);

        gameObject.SetActive(false);
    }
}