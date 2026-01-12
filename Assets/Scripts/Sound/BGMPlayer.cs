using System;
using UnityEngine;
using UnityEngine.Video;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmClips; // 0 : boss Start / 1 : boss fight / 2 : bossFight Loop / 3 : boss End
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
        canBGM = true;
        ChangeBGM(0);
    }
    private void Update()
    {
        if (!canBGM) return;
        if (audioSource == null) return;
        if (audioSource.loop) return;

        // 재생 시작 감지
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
