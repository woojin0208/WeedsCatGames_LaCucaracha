using System;
using UnityEngine;
using UnityEngine.Video;

// 배경음 재생과 순차 전환을 관리한다.
public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private VideoPlayer vp;
    [SerializeField] private AudioSource audioSource;
    public event Action<int> onAudioEnd;

    private bool _wasPlaying = false;

    private int count = 0;

    [SerializeField] private bool[] loopBGMCounts;

    [SerializeField] private bool canBGM = true;
    private void Start()
    {
        onAudioEnd += ChangeBGM;

        if (vp != null) vp.loopPointReached += StartBGM;
    }

    private void StartBGM(VideoPlayer vp)
    {
        StartBGM(0);
    }

    public void StartBGM(int bgmIndex = 0)
    {
        canBGM = true;
        _wasPlaying = false;
        count = bgmIndex;

        ChangeBGM(bgmIndex);
    }
    private void Update()
    {
        if (!canBGM) return;
        if (audioSource == null) return;
        if (audioSource.loop) return;

        // 한 번 재생이 시작된 뒤 종료되었을 때만 다음 BGM으로 넘어간다.
        if (!_wasPlaying && audioSource.isPlaying)
            _wasPlaying = true;

        if (_wasPlaying && !audioSource.isPlaying)
        {
            _wasPlaying = false;
            count++;
            onAudioEnd?.Invoke(count);
        }
    }

    public void ChangeBGM(int count = -1)
    {
        if (count == -1)
        {
            this.count++;
            count = this.count;
        }

        if (count >= bgmClips.Length) return;
        audioSource.clip = bgmClips[count];

        audioSource.loop = loopBGMCounts[count];

        audioSource.Play();

    }
}
