using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// 효과음을 즉시 재생한다.
public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] vfxClips;
    [SerializeField] private AudioSource audioSource;

    public void StartVFX(int count)
    {
        audioSource.Stop();

        audioSource.clip = vfxClips[count];
        audioSource.Play();
    }
}