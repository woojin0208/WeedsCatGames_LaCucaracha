using UnityEngine;
using UnityEngine.Video;

// 배경음 재생과 순차 전환을 관리한다.
public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool[] loopBGMCounts;
    [SerializeField] private bool canBGM = true;

    private bool wasPlaying;
    private int currentBGMIndex;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += HandleVideoEnded;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HandleVideoEnded;
        }
    }

    private void Update()
    {
        if (!canBGM) return;
        if (audioSource == null) return;
        if (audioSource.loop) return;

        if (!wasPlaying && audioSource.isPlaying)
        {
            wasPlaying = true;
        }

        if (!wasPlaying || audioSource.isPlaying) return;

        wasPlaying = false;
        PlayBGM(currentBGMIndex + 1);
    }

    public void StartBGM(int bgmIndex = 0)
    {
        canBGM = true;
        wasPlaying = false;

        PlayBGM(bgmIndex);
    }

    // 기존 Animation/Event/Inspector 호출 호환용 메서드다.
    public void ChangeBGM(int bgmIndex = -1)
    {
        int nextIndex = bgmIndex >= 0 ? bgmIndex : currentBGMIndex + 1;
        PlayBGM(nextIndex);
    }

    private void HandleVideoEnded(VideoPlayer endedVideoPlayer)
    {
        StartBGM(0);
    }

    private void PlayBGM(int bgmIndex)
    {
        if (!CanPlayBGM(bgmIndex)) return;

        currentBGMIndex = bgmIndex;
        audioSource.clip = bgmClips[currentBGMIndex];
        audioSource.loop = IsLoopBGM(currentBGMIndex);
        audioSource.Play();
    }

    private bool CanPlayBGM(int bgmIndex)
    {
        if (audioSource == null)
        {
            Debug.LogError("[BGMPlayer] AudioSource가 할당되지 않았습니다.", this);
            return false;
        }

        if (bgmClips == null || bgmClips.Length == 0)
        {
            Debug.LogWarning("[BGMPlayer] BGM Clip이 비어 있습니다.", this);
            return false;
        }

        if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
        {
            return false;
        }

        if (bgmClips[bgmIndex] == null)
        {
            Debug.LogWarning($"[BGMPlayer] BGM Clip이 비어 있습니다. index: {bgmIndex}", this);
            return false;
        }

        return true;
    }

    private bool IsLoopBGM(int bgmIndex)
    {
        if (loopBGMCounts == null) return false;
        if (bgmIndex < 0 || bgmIndex >= loopBGMCounts.Length) return false;

        return loopBGMCounts[bgmIndex];
    }
}